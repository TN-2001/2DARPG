using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : StateMachine<EnemyController>
{
    protected override Type type => Type.FixedUpdate;

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

    // 物理コンポーネント
    private Rigidbody2D rb = null;
    // アニメーションコンポーネント
    private Animator anim = null;
    [SerializeField, ReadOnly] // キャラクター
    private Enemy enemy = null;
    // 向き
    private Vector2 dir = Vector2.zero;
    [SerializeField, ReadOnly] // エリア内のターゲット
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField, ReadOnly] // 攻撃番号
    private int attackNumber = 0;
    // 攻撃終了フラグ
    private bool isAttackEnd = false;
    [SerializeField, ReadOnly] // ターゲット
    private GameObject target = null;
    // ターゲットとの距離
    private float distance = 0;
    // クール時間カウント
    private float countCoolTime = 0;
    // 可能な攻撃番号
    private List<int> numbers = new List<int>();


    public void Init(Enemy enemy)
    {
        this.enemy = enemy;
    }

    private void OnEnterArea(Collider2D other)
    {
        targets.Add(other.gameObject);
    }

    private void OnExitArea(Collider2D other)
    {
        targets.Remove(other.gameObject);
        
        if(other.gameObject == target)
        {
            if(targets.Count > 0)
            {
                float dis = 100;
                for(int i = 0; i < targets.Count; i++)
                {
                    if(Vector3.Distance(targets[i].transform.position, transform.position) < dis)
                    {
                        target = targets[i];
                        dis = Vector3.Distance(targets[i].transform.position, transform.position);
                    }
                }
            }
            else
            {
                target = null;
            }
        }
    }

    private void OnEncount(Collider2D other)
    {
        target = other.gameObject;
    }
    
    public void OnAttackEnd()
    {
        isAttackEnd = true;
    }

    public void OnDamage(int damage)
    {
        int dam = enemy.UpdateHp(-damage);
        DungeonUI.I.InitializeDamageText(dam, transform);

        if(!target)
        {
            if(targets.Count > 0)
            {
                float dis = 100;
                for(int i = 0; i < targets.Count; i++)
                {
                    if(Vector3.Distance(targets[i].transform.position, transform.position) < dis)
                    {
                        target = targets[i];
                        dis = Vector3.Distance(targets[i].transform.position, transform.position);
                    }
                }
            }
        }
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        areaDetector.onTriggerEnter.AddListener(OnEnterArea);
        areaDetector.onTriggerExit.AddListener(OnExitArea);
        serchDetector.onTriggerEnter.AddListener(OnEncount);
        for(int i = 0; i < attackControllers.Length; i++)
        {
            attackControllers[i].Initialize(enemy.Atk);
        }

        ChangeState(new IdleState(this));
    }

    private class IdleState : State<EnemyController>
    {
        public IdleState(EnemyController m) : base(m){}

        public override List<(bool, State<EnemyController>)> StateList => new List<(bool, State<EnemyController>)>()
        {
            (m.enemy.CurrentHp == 0, new DieState(m)),
            (m.target, new ReadyState(m)),
            (countTime > m.idleTime, new WalkState(m))
        };

        // 時間カウント
        private float countTime = 0;

        public override void OnUpdate()
        {
            countTime += Time.fixedDeltaTime;
        }
    }

    private class WalkState : State<EnemyController>
    {
        public WalkState(EnemyController m) : base(m){}

        public override List<(bool, State<EnemyController>)> StateList => new List<(bool, State<EnemyController>)>()
        {
            (m.enemy.CurrentHp == 0, new DieState(m)),
            (m.target, new ReadyState(m)),
            (countTime > m.walkTime, new IdleState(m))
        };

        // 時間カウント
        private float countTime = 0;

        public override void OnEnter()
        {
            // 初期化
            m.rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            m.anim.SetFloat("speed", 0.5f);
            // 向き決め
            m.dir = Random.insideUnitCircle.normalized;
        }

        public override void OnUpdate()
        {
            countTime += Time.fixedDeltaTime;

            m.rb.velocity = m.dir * m.walkSpeed;
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
        }

        public override void OnExit()
        {
            m.rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            m.anim.SetFloat("speed", 0);
        }
    }

    private class ReadyState : State<EnemyController>
    {
        public ReadyState(EnemyController m) : base(m){}

        public override List<(bool, State<EnemyController>)> StateList => new List<(bool, State<EnemyController>)>()
        {
            (m.enemy.CurrentHp == 0, new DieState(m)),
            (!m.target, new IdleState(m)),
            (m.countCoolTime <= m.coolTime & m.distance > m.closeDistance, new ChaseState(m)),
            (m.countCoolTime > m.coolTime & m.numbers.Count > 0, new AttackState(m))
        };

        public override void OnUpdate()
        {
            m.countCoolTime += Time.fixedDeltaTime;

            if(m.target)
            {
                m.distance = Vector3.Distance(m.target.transform.position, m.transform.position);
                m.numbers.Clear();
                for(int i = 0; i < m.attackControllers.Length; i++)
                {
                    if(m.distance <= m.attackControllers[i].Area)
                    {
                        m.numbers.Add(i);
                    }
                }
                if(m.numbers.Count > 0)
                {
                    m.attackNumber = m.numbers[Random.Range(0, m.numbers.Count)];
                }
            }

            if(m.target)
            {
                m.dir = (m.target.transform.position - m.transform.position).normalized;
            }
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
        }
    }

    private class ChaseState : State<EnemyController>
    {
        public ChaseState(EnemyController m) : base(m){}

        public override List<(bool, State<EnemyController>)> StateList => new List<(bool, State<EnemyController>)>()
        {
            (m.enemy.CurrentHp == 0, new DieState(m)),
            (!m.target, new IdleState(m)),
            (m.countCoolTime <= m.coolTime & m.distance <= m.closeDistance, new ReadyState(m)),
            (m.countCoolTime > m.coolTime & m.numbers.Count > 0, new AttackState(m))
        };

        public override void OnEnter()
        {
            m.rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            m.anim.SetFloat("speed", 1);
        }

        public override void OnUpdate()
        {
            m.countCoolTime += Time.fixedDeltaTime;

            if(m.target)
            {
                m.distance = Vector3.Distance(m.target.transform.position, m.transform.position);
                m.numbers.Clear();
                for(int i = 0; i < m.attackControllers.Length; i++)
                {
                    if(m.distance <= m.attackControllers[i].Area)
                    {
                        m.numbers.Add(i);
                    }
                }
                if(m.numbers.Count > 0)
                {
                    m.attackNumber = m.numbers[Random.Range(0, m.numbers.Count)];
                }
            }

            if(m.target)
            {
                m.dir = (m.target.transform.position - m.transform.position).normalized;
            }
            m.rb.velocity = m.dir * m.dashSpeed;
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
        }

        public override void OnExit()
        {
            m.rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            m.anim.SetFloat("speed", 0);
        }
    }

    private class AttackState : State<EnemyController>
    {
        public AttackState(EnemyController m) : base(m){}

        public override List<(bool, State<EnemyController>)> StateList => new List<(bool, State<EnemyController>)>()
        {
            (m.enemy.CurrentHp == 0, new DieState(m)),
            (m.isAttackEnd, new ReadyState(m))
        };

        public override void OnEnter()
        {
            m.dir = (m.target.transform.position - m.transform.position).normalized;
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);

            if(m.attackControllers[m.attackNumber].IsThrow)
            {
                GameObject obj = Instantiate(m.attackControllers[m.attackNumber].gameObject, 
                    m.attackControllers[m.attackNumber].transform.position,
                    m.attackControllers[m.attackNumber].transform.rotation);
                obj.transform.SetParent(m.transform.parent);
                obj.SetActive(true);
            }
            else
            {
                m.attackControllers[m.attackNumber].gameObject.SetActive(true);
            }

            m.anim.SetFloat("attackNumber", m.attackNumber + 1);
            m.anim.SetTrigger("isAttack");
        }

        public override void OnExit()
        {
            m.isAttackEnd = false;
            m.anim.SetFloat("attackNumber", 0);
            if(!m.attackControllers[m.attackNumber].IsThrow)
            {
                m.attackControllers[m.attackNumber].gameObject.SetActive(false);
            }
            m.countCoolTime = 0;
        }
    }

    private class DieState : State<EnemyController>
    {
        public DieState(EnemyController m) : base(m){}

        public override void OnEnter()
        {
            GameManager.I.Data.UpdateEnemyData(m.enemy.Data);
            Destroy(m.gameObject);
        }
    }
}