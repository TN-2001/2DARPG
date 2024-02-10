using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // ステート
    private enum State{Idle, Walk, Ready, Chase, Attack, Hit, Die}
    [SerializeField, ReadOnly]
    private State state = State.Idle;
    private State nextState = State.Idle;
    private bool isEnter = false;

    // 物理コンポーネント
    private Rigidbody2D rb = null;
    // アニメーションコンポーネント
    private Animator anim = null;
    [SerializeField] // スプライト
    private SpriteRenderer sprite = null;
    [SerializeField] // 向きオブジェクト
    private Transform rotation = null;
    [SerializeField] // エリア判定
    private CollisionDetector areaDetector = null;
    [SerializeField] // 攻撃
    private AttackController[] attackControllers = null;
    [SerializeField] // 歩きスピード
    private float walkSpeed = 0;
    [SerializeField] // ダッシュスピード
    private float dashSpeed = 0;
    [SerializeField] // 索敵判定
    private CollisionDetector serchDetector = null;
    [SerializeField] // 立ち時間
    private float idleTime = 0; 
    [SerializeField] // 歩き時間
    private float walkTime = 0;
    [SerializeField] // クールタイム
    private float coolTime = 0;
    [SerializeField] // 接敵距離
    private float closeDistance = 0;

    [SerializeField] // キャラクター
    private Enemy enemy = null;
    // 向き
    private Vector2 dir = Vector2.zero;
    [SerializeField] // 攻撃番号
    private int attackNumber = 0;
    [SerializeField] // ターゲット
    private GameObject target = null;
    // ターゲットとの距離
    private float distance = 0;
    [SerializeField] // 時間カウント
    private float countTime = 0;
    // 可能な攻撃番号
    private List<int> numbers = new List<int>();


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        areaDetector.onTriggerEnter.AddListener(delegate(Collider2D other){
            target = other.gameObject;
        });
        areaDetector.onTriggerExit.AddListener(delegate(Collider2D other){
            target = null;
        });
        serchDetector.onTriggerEnter.AddListener(delegate(Collider2D other){
            target = other.gameObject;
        });
    }

    public void Init(Enemy enemy)
    {
        this.enemy = enemy;
        for(int i = 0; i < attackControllers.Length; i++){
            attackControllers[i].Initialize(enemy.Atk);
        }
    }

    public void OnAttackEnd()
    {
        nextState = State.Ready;
    }

    public void OnDamage(int damage)
    {
        int dam = enemy.UpdateHp(-damage);
        DungeonUI.I.InitDamageText(-dam, transform);

        if(enemy.CurrentHp > 0) nextState = State.Hit;
        else nextState = State.Die;
    }

    private void FixedUpdate()
    {
        if(state == nextState){
            if(state == State.Idle | state == State.Walk){
                if(target){
                    countTime = 0;
                    nextState = State.Ready;
                }
            }
            else if(state == State.Ready | state == State.Chase){
                if(!target) nextState = State.Idle;
            }
        }

        if(target){
            // 敵の方向取得
            dir = (target.transform.position - transform.position).normalized;
            // 敵との距離取得
            distance = Vector3.Distance(target.transform.position, transform.position);
            // 攻撃の番号を決定
            numbers.Clear();
            for(int i = 0; i < attackControllers.Length; i++){
                if(distance <= attackControllers[i].Area) numbers.Add(i);
            }
            if(numbers.Count > 0) attackNumber = numbers[Random.Range(0, numbers.Count)];
        }


        switch(state)
        {
            case State.Idle:
                if(!isEnter){
                    isEnter = true;

                    countTime = 0;
                }

                if(nextState == state & countTime >= idleTime) nextState = State.Walk;

                if(nextState != state){
                    state = nextState;
                    isEnter = false;
                }
                break;

            case State.Walk:
                if(!isEnter){
                    isEnter = true;

                    // 初期化
                    rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                    anim.SetFloat("speed", 0.5f);
                    // 向き決め
                    dir = Random.insideUnitCircle.normalized;
                    countTime = 0;
                }

                rb.velocity = dir * walkSpeed;
                anim.SetFloat("x", dir.normalized.x);
                anim.SetFloat("y", dir.normalized.y);
                rotation.rotation = Quaternion.FromToRotation(Vector3.up, dir);

                if(nextState == state & countTime >= walkTime) nextState = State.Idle;

                if(nextState != state){
                    state = nextState;
                    isEnter = false;

                    rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                    anim.SetFloat("speed", 0);
                }
                break;

            case State.Ready:
                if(!isEnter){
                    isEnter = true;
                }

                anim.SetFloat("x", dir.normalized.x);
                anim.SetFloat("y", dir.normalized.y);
                rotation.rotation = Quaternion.FromToRotation(Vector3.up, dir);

                if(nextState == state){
                    if(countTime > coolTime) nextState = State.Attack;
                    else if(distance > closeDistance) nextState = State.Chase;
                }

                if(nextState != state){
                    state = nextState;
                    isEnter = false;
                }
                break;

            case State.Chase:
                if(!isEnter){
                    isEnter = true;

                    rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                    anim.SetFloat("speed", 1);
                }

                rb.velocity = dir * dashSpeed;
                anim.SetFloat("x", dir.normalized.x);
                anim.SetFloat("y", dir.normalized.y);
                rotation.rotation = Quaternion.FromToRotation(Vector3.up, dir);

                if(nextState == state){
                    if(countTime > coolTime) nextState = State.Attack;
                    else if(distance <= closeDistance) nextState = State.Ready;
                }

                if(nextState != state){
                    state = nextState;
                    isEnter = false;

                    rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                    anim.SetFloat("speed", 0);
                }
                break;

            case State.Attack:
                if(!isEnter){
                    isEnter = true;

                    rotation.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                    anim.SetFloat("x", dir.normalized.x);
                    anim.SetFloat("y", dir.normalized.y);

                    if(attackControllers[attackNumber].IsThrow){
                        GameObject obj = Instantiate(attackControllers[attackNumber].gameObject, 
                            attackControllers[attackNumber].transform.position,
                            attackControllers[attackNumber].transform.rotation);
                        obj.transform.SetParent(transform.parent);
                        obj.SetActive(true);
                    }
                    else{
                        attackControllers[attackNumber].gameObject.SetActive(true);
                    }
                    anim.SetFloat("atkNum", attackNumber);
                    anim.SetTrigger("isAtk");
                }

                if(nextState != state){
                    state = nextState;
                    isEnter = false;

                    anim.SetFloat("atkNum", 0);
                    if(!attackControllers[attackNumber].IsThrow){
                        attackControllers[attackNumber].gameObject.SetActive(false);
                    }
                    countTime = 0;
                }
                break;

            case State.Hit:
                if(!isEnter){
                    isEnter = true;

                    GetComponent<Collider2D>().enabled = false;
                    sprite.color = new Color(1,0,0,1);
                    countTime = 0;

                    // プレイヤーの方を向く
                    dir = (PlayerController.I.transform.position - transform.position).normalized;
                    rotation.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                    anim.SetFloat("x", dir.normalized.x);
                    anim.SetFloat("y", dir.normalized.y);
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
                if(sprite.color.a <= 0){
                    for(int i = 0; i < GameManager.I.DataBase.EnemyDataList.Count; i++){
                        if(GameManager.I.DataBase.EnemyDataList[i] == enemy.Data){
                            GameManager.I.Data.UpdateFindEnemy(i);
                            break;
                        }
                    }
                    Destroy(gameObject);
                }

                break;
        }

        countTime += Time.fixedDeltaTime;
    }
}