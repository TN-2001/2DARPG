using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : StateMachine<EnemyController>
{
    [SerializeField] // 向きオブジェクト
    private Transform rotation = null;
    [SerializeField] // 索敵判定
    private CollisionDetector serchDetector = null;
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
    [SerializeField] // 攻撃
    private AttackController[] attackControllers = null;
=======
    [SerializeField] // 攻撃判定
    private CollisionDetector attackDetector = null;
>>>>>>> parent of 5a5e18f (データベースからキャラクターのデータを使用可能に、攻撃時に敵の方を向けるように)
=======
    [SerializeField] // 攻撃判定
    private CollisionDetector[] attackDetectors = null;
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
    [SerializeField] // 攻撃判定
    private CollisionDetector[] attackDetectors = null;
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
    [SerializeField] // 攻撃判定
    private CollisionDetector[] attackDetectors = null;
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
    [SerializeField] // 歩きスピード
    private float walkSpeed = 0;
    [SerializeField] // ダッシュスピード
    private float dashSpeed = 0;
    [SerializeField] // 追いかけ距離
    private float chaseDistance = 0;
    [SerializeField] // 攻撃距離
    private float attackDistance = 0;
    [SerializeField] // 立ち時間
    private float idleTime = 0; 
    [SerializeField] // 歩き時間
    private float walkTime = 0;
    [SerializeField] // hp
    private int hp = 0;

    // AIコンポーネント
    private NavMeshAgent agent = null;
    // アニメーションコンポーネント
    private Animator anim = null;
    // 向き
    private Vector2 dir = Vector2.zero;
    // 敵
    private GameObject target = null;
    // 敵との距離
    private float distance = 0;
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
    [SerializeField] // 攻撃番号
=======
    // 攻撃番号
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
    // 攻撃番号
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
    // 攻撃番号
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
    private int attackNumber = 0;
=======
>>>>>>> parent of 5a5e18f (データベースからキャラクターのデータを使用可能に、攻撃時に敵の方を向けるように)
    // 攻撃終了フラグ
    private bool attackEndFlag = false;

    // Updateのタイプ
    protected override Type type => Type.FixedUpdate;


    private void OnEncount(Collider2D other)
    {
        target = other.gameObject;
    }

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
    private void OnAttackHit(Collider2D other)
    {
        other.GetComponent<PlayerController>().OnDamage(1);
    }

>>>>>>> parent of 5a5e18f (データベースからキャラクターのデータを使用可能に、攻撃時に敵の方を向けるように)
=======
=======
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
    private void OnAttackHit(Collider2D other)
    {
        other.GetComponent<PlayerController>().OnDamage(character.GetAttack(attackNumber));
    }

<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
    private void OnAttackEnd()
    {
        attackEndFlag = true;
    }

    public void OnDamage(int damage)
    {
        hp -= damage;
    }


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        serchDetector.onTriggerEnter.AddListener(OnEncount);
        attackDetector.onTriggerEnter.AddListener(OnAttackHit);

        ChangeState(new IdleState(this));
    }

    private class IdleState : State<EnemyController>
    {
        public IdleState(EnemyController _m) : base(_m){}

        private float countTime = 0;

        public override void OnUpdate()
        {
            if(m.target)
            {
                m.ChangeState(new ChaseState(m));
                return;
            }
            else if(countTime >= m.idleTime)
            {
                m.ChangeState(new WalkState(m));
                return;
            }

            countTime += Time.fixedDeltaTime;
        }
    }

    private class WalkState : State<EnemyController>
    {
        public WalkState(EnemyController _m) : base(_m){}

        private float countTime = 0;

        public override void OnEnter()
        {
            m.dir = Random.insideUnitCircle.normalized;
            m.anim.SetFloat("speed", 0.5f);
        }

        public override void OnUpdate()
        {
            if(m.target)
            {
                m.ChangeState(new ChaseState(m));
                return;
            }
            else if(countTime >= m.walkTime)
            {
                m.ChangeState(new IdleState(m));
                return;
            }

            m.transform.position += m.transform.TransformDirection(m.dir) * Time.fixedDeltaTime * m.walkSpeed;
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);

            countTime += Time.fixedDeltaTime;
        }

        public override void OnExit()
        {
            m.anim.SetFloat("speed", 0);
        }
    }

    private class ChaseState : State<EnemyController>
    {
        public ChaseState(EnemyController _m) : base(_m){}

        private Vector2 prePos = Vector2.zero;

        public override void OnEnter()
        {
            m.agent.isStopped = false;
            prePos = m.transform.position;
        }

        public override void OnUpdate()
        {
            m.distance = Vector3.Distance(m.target.transform.position, m.transform.position);

            if(m.chaseDistance < m.distance)
            {
                m.ChangeState(new IdleState(m));
                m.target = null;
                return;
            }
            else if(m.attackDistance > m.distance)
            {
                m.ChangeState(new AttackState(m));
                return;
            }

            m.agent.speed = m.dashSpeed;
            m.agent.SetDestination(m.target.transform.position);
            if(prePos.x != m.transform.position.x | prePos.y != m.transform.position.y)
            {
                m.dir = new Vector2(m.transform.position.x - prePos.x, m.transform.position.y - prePos.y);
            }
            m.anim.SetFloat("speed", 1);
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);

            prePos = m.transform.position;
        }

        public override void OnExit()
        {
            m.anim.SetFloat("speed", 0);
            m.agent.isStopped = true;
        }
    }

    private class AttackState : State<EnemyController>
    {
        public AttackState(EnemyController _m) : base(_m){}

        public override void OnEnter()
        {
<<<<<<< HEAD
            m.dir = (m.target.transform.position - m.transform.position).normalized;
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
            m.attackDetectors[m.attackNumber].onTriggerEnter.AddListener(m.OnAttackHit);
            m.attackDetectors[m.attackNumber].gameObject.SetActive(true);
            m.anim.SetFloat("attackNumber", m.attackNumber + 1);
=======
            m.anim.SetFloat("attackNumber", 1);
>>>>>>> parent of 5a5e18f (データベースからキャラクターのデータを使用可能に、攻撃時に敵の方を向けるように)
            m.anim.SetTrigger("isAttack");
            m.attackDetector.gameObject.SetActive(true);
        }

        public override void OnUpdate()
        {
            m.distance = Vector3.Distance(m.target.transform.position, m.transform.position);

            if(m.attackEndFlag)
            {
                m.ChangeState(new ChaseState(m));
                return;
            }
        }

        public override void OnExit()
        {
            m.attackEndFlag = false;
            m.anim.SetFloat("attackNumber", 0);
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            if(m.character.AttackDatas[m.attackNumber]._Type != AttackData.Type.Throw)
            {
                m.attackControllers[m.attackNumber].gameObject.SetActive(false);
            }
=======
            m.attackDetector.gameObject.SetActive(false);
>>>>>>> parent of 5a5e18f (データベースからキャラクターのデータを使用可能に、攻撃時に敵の方を向けるように)
=======
            m.attackDetectors[m.attackNumber].gameObject.SetActive(false);
            m.attackDetectors[m.attackNumber].onTriggerEnter.RemoveAllListeners();
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
            m.attackDetectors[m.attackNumber].gameObject.SetActive(false);
            m.attackDetectors[m.attackNumber].onTriggerEnter.RemoveAllListeners();
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
=======
            m.attackDetectors[m.attackNumber].gameObject.SetActive(false);
            m.attackDetectors[m.attackNumber].onTriggerEnter.RemoveAllListeners();
>>>>>>> parent of 838f5c7 (AttackControllerの作成、遠距離攻撃を可能に)
        }
    }
}
