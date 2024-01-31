using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController I = null;
    // ステート
    private enum State{Idle, Move, Attack, Die}
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
    [SerializeField, ReadOnly] // エリア内のターゲット
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField, ReadOnly] // イベントディテクター
    private EventController eventController = null;
    [SerializeField, ReadOnly] // 攻撃番号
    private int attackNumber = 0;
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

        DungeonUI.I?.InitializeHpSlider(player.Hp);
    }

    public void OnAttackEnd()
    {
        nextState = State.Idle;
    }

    public void OnDamage(int damage)
    {
        player.UpdateHp(-damage);
        DungeonUI.I.UpdateHpSlider(player.CurrentHp);
    }

    private void FixedUpdate()
    {
        if(state != State.Attack)
        {
            if(input.actions["Attack"].IsPressed()){
                attackNumber = 0;
                nextState = State.Attack;
            }
            else if(input.actions["Skill"].IsPressed()){
                attackNumber = 1;
                nextState = State.Attack;
            }
        }

        if(isIdle) nextState = State.Idle;
        else if(input.actions["Do"].IsPressed()) eventController.Do();


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
                }

                // 向き取得
                if(input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0)
                    dir = input.actions["Move"].ReadValue<Vector2>().normalized;
                // 移動
                if(input.actions["Dash"].IsPressed()){
                    rb.velocity = dir * dashSpeed;
                    anim.SetFloat("speed", 1f);
                }
                else{
                    rb.velocity = dir * walkSpeed;
                    anim.SetFloat("speed", 0.5f);
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
                }
                break;

            case State.Attack:
                if(!isEnter){
                    isEnter = true;

                    if(targets.Count > 0)
                    {
                        GameObject target = null;
                        float dis = 100;
                        for(int i = 0; i < targets.Count; i++)
                        {
                            if(Vector3.Distance(targets[i].transform.position, transform.position) < dis)
                            {
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

                    if(attackControllers[attackNumber].IsThrow)
                    {
                        GameObject obj = Instantiate(attackControllers[attackNumber].gameObject, 
                            attackControllers[attackNumber].transform.position,
                            attackControllers[attackNumber].transform.rotation);
                        obj.transform.SetParent(transform.parent);
                        obj.SetActive(true);
                    }

                    anim.SetFloat("attackNumber", attackNumber + 1);
                    anim.SetTrigger("isAttack");
                }

                if(nextState != state){
                    state = nextState;
                    isEnter = false;
                }
                break;

            case State.Die:
                if(!isEnter){
                    isEnter = true;
                }

                if(nextState != state){
                    state = nextState;
                    isEnter = false;
                }
                break;
        }
    }
}
