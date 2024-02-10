using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController I = null;
    // ステート
    private enum State{Idle, Move, Attack, Hit, Die}
    [SerializeField, ReadOnly]
    private State state = State.Idle;
    private State nextState = State.Idle;
    private bool isEnter = false;

    // 物理コンポーネント
    private Rigidbody2D rb = null;
    // アニメーションコンポーネント
    private Animator anim = null;
    // PlayerInput
    private PlayerInput input = null;
    [SerializeField] // スプライト
    private SpriteRenderer sprite = null;
    [SerializeField] // 向きオブジェクト
    private Transform rotation = null;
    [SerializeField] // 敵エリア判定
    private CollisionDetector enemyAreaDetector = null;
    [SerializeField] // イベントエリア判定
    private CollisionDetector eventAreaDetector = null;
    [SerializeField] // 攻撃
    private AttackController[] attackControllers = null;
    [SerializeField] // 歩きスピード
    private float walkSpeed = 0;
    [SerializeField] // ダッシュスピード
    private float dashSpeed = 0;

    // キャラクター
    private Player player => GameManager.I.Data.Player;
    // 向き
    private Vector2 dir = Vector2.zero;
    [SerializeField] // エリア内のターゲット
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField] // イベントディテクター
    private EventController eventController = null;
    [SerializeField] // 攻撃番号
    private int attackNumber = 0;
    [SerializeField] // 時間カウント
    private float countTime = 0;
    // 強制アイドルフラグ
    public bool isIdle = false;


    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();

        enemyAreaDetector.onTriggerEnter.AddListener(delegate(Collider2D other){
            targets.Add(other.gameObject);
        });
        enemyAreaDetector.onTriggerExit.AddListener(delegate(Collider2D other){
            targets.Remove(other.gameObject);
        });

        eventAreaDetector.onTriggerEnter.AddListener(delegate(Collider2D other){
            eventController = other.GetComponent<EventController>();
        });
        eventAreaDetector.onTriggerExit.AddListener(delegate(Collider2D other){
            if(other.gameObject == eventController.gameObject) eventController = null;
        });

        for(int i = 0; i < attackControllers.Length; i++){
            attackControllers[i].Initialize(player.Atk);
        }

        DungeonUI.I?.InitHpSlider(player.Hp);
    }

    public void OnAttackEnd()
    {
        nextState = State.Idle;
    }

    public void OnDamage(int damage)
    {
        player.UpdateHp(-damage);
        DungeonUI.I.UpdateHpSlider(player.CurrentHp);

        if(player.CurrentHp > 0) nextState = State.Hit;
        else nextState = State.Die;
    }


    private void FixedUpdate()
    {
        if(isIdle) nextState = State.Idle;
        else if(input.actions["Do"].WasPressedThisFrame()) eventController?.Do();

        if(state == nextState){
            if(state == State.Idle | state == State.Move){
                if(input.actions["Attack"] != null){
                    if(input.actions["Attack"].WasPressedThisFrame()){
                        attackNumber = 0;
                        nextState = State.Attack;
                    }
                }
                if(input.actions["Skill"] != null){
                    if(input.actions["Skill"].WasPressedThisFrame()){
                        attackNumber = 1;
                        nextState = State.Attack;
                    }
                }
            }
        }


        switch(state)
        {
            case State.Idle:
                if(!isEnter){
                    isEnter = true;
                }

                if(input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0 & !isIdle)
                    nextState = State.Move;

                if(nextState != state){
                    state = nextState;
                    isEnter = false;
                }
                break;

            case State.Move:
                if(!isEnter){
                    isEnter = true;

                    anim.SetFloat("speed", 1f);
                }

                // 向き取得
                if(input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0)
                    dir = input.actions["Move"].ReadValue<Vector2>().normalized;
                // 移動
                if(input.actions["Dash"].IsPressed()){
                    rb.velocity = dir * dashSpeed;
                    anim.speed = 2f;
                }
                else{
                    rb.velocity = dir * walkSpeed;
                    anim.speed = 1f;
                }
                // 向き
                anim.SetFloat("x", dir.x);
                anim.SetFloat("y", dir.y);
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, dir);
                Vector3 vector3 = quaternion.eulerAngles;
                vector3.y = 0;
                rotation.rotation = Quaternion.Euler(vector3);

                if(input.actions["Move"].ReadValue<Vector2>().normalized.magnitude == 0)
                    nextState = State.Idle;

                if(nextState != state){
                    state = nextState;
                    isEnter = false;

                    rb.velocity = Vector2.zero;
                    anim.SetFloat("speed", 0f);
                    anim.speed = 1f;
                }
                break;

            case State.Attack:
                if(!isEnter){
                    isEnter = true;

                    // 近くに敵がいたらその方を向く
                    if(targets.Count > 0){
                        GameObject target = null;
                        float dis = 100;
                        for(int i = 0; i < targets.Count; i++){
                            if(Vector3.Distance(targets[i].transform.position, transform.position) < dis){
                                target = targets[i];
                                dis = Vector3.Distance(targets[i].transform.position, transform.position);
                            }
                        }
                        dir = (target.transform.position - transform.position).normalized;
                        Quaternion quat = Quaternion.FromToRotation(Vector3.up, dir);
                        Vector3 vec3 = quat.eulerAngles;
                        vec3.y = 0;
                        rotation.rotation = Quaternion.Euler(vec3);
                        anim.SetFloat("x", dir.normalized.x);
                        anim.SetFloat("y", dir.normalized.y);
                    }

                    if(attackControllers[attackNumber].IsThrow){
                        GameObject obj = Instantiate(attackControllers[attackNumber].gameObject, 
                            attackControllers[attackNumber].transform.position,
                            attackControllers[attackNumber].transform.rotation, transform.parent);
                        obj.SetActive(true);
                    }

                    anim.SetFloat("atkNum", attackNumber);
                    anim.SetTrigger("isAtk");
                }

                if(nextState != state){
                    state = nextState;
                    isEnter = false;
                }
                break;

            case State.Hit:
                if(!isEnter){
                    isEnter = true;

                    GetComponent<Collider2D>().enabled = false;
                    sprite.color = new Color(1,0,0,1);
                    countTime = 0;
                }

                if(countTime > 0.2f) nextState = State.Idle;

                if(nextState != state){
                    state = nextState;
                    isEnter = false;

                    GetComponent<Collider2D>().enabled = true;
                    sprite.color = new Color(1,1,1,1);
                }
                break;

            case State.Die:
                if(!isEnter){
                    isEnter = true;

                    GetComponent<Collider2D>().enabled = false;
                }

                sprite.color = new Color(1,1,1, sprite.color.a - Time.fixedDeltaTime);

                break;
        }

        countTime += Time.fixedDeltaTime;
    }
}
