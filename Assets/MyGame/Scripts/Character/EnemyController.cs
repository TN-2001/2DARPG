using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : StateMachine<EnemyController>
{
    [SerializeField] // ナンバー
    private int number = 0;
    [SerializeField] // 向きオブジェクト
    private Transform rotation = null;
    [SerializeField] // エリア判定
    private CollisionDetector areaDetector = null;
    [SerializeField] // 索敵判定
    private CollisionDetector serchDetector = null;
    [SerializeField] // 攻撃
    private AttackController[] attackControllers = null;
    [SerializeField] // 歩きスピード
    private float walkSpeed = 0;
    [SerializeField] // ダッシュスピード
    private float dashSpeed = 0;
    [SerializeField] // 立ち時間
    private float idleTime = 0; 
    [SerializeField] // 歩き時間
    private float walkTime = 0;
    [SerializeField] // クールタイム
    private float coolTime = 0;
    [SerializeField] // 接敵距離
    private float closeDistance = 0;

    // AIコンポーネント
    private NavMeshAgent agent = null;
    // アニメーションコンポーネント
    private Animator anim = null;
    [SerializeField, ReadOnly] // キャラクター
    private Character character = null;
    // 向き
    private Vector2 dir = Vector2.zero;
    [SerializeField, ReadOnly] // エリア内のターゲット
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField, ReadOnly] // ターゲット
    private GameObject target = null;
    // ターゲットとの距離
    private float distance = 0;
    // 時間カウント
    private float countTime = 0;
    // クール時間カウント
    private float countCoolTime = 0;
    // 可能な攻撃番号
    private List<int> numbers = new List<int>();
    [SerializeField, ReadOnly] // 攻撃番号
    private int attackNumber = 0;
    // 攻撃終了フラグ
    private bool isAttackEnd = false;

    // Updateのタイプ
    protected override Type type => Type.FixedUpdate;


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

    private void OnAttackEnd()
    {
        isAttackEnd = true;
    }

    public void OnDamage(int damage)
    {
        int dam = character.OnDamage(damage);
        GameUI.I.InitializeDamageText(dam, transform);

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
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        character = new Character(GameManager.I.DataBase.CharacterDatas[number]);

        areaDetector.onTriggerEnter.AddListener(OnEnterArea);
        areaDetector.onTriggerExit.AddListener(OnExitArea);
        serchDetector.onTriggerEnter.AddListener(OnEncount);
        for(int i = 0; i < attackControllers.Length; i++)
        {
            attackControllers[i].Initialize(character.Atk);
        }

        ChangeState(new IdleState(this));
    }

    private class IdleState : State<EnemyController>
    {
        public IdleState(EnemyController _m) : base(_m){}

        public override void OnEnter()
        {
            m.countTime = 0;
        }

        public override void OnUpdate()
        {
            m.countTime += Time.fixedDeltaTime;

            if(m.target)
            {
                m.ChangeState(new ReadyState(m));
                return;
            }
            else if(m.countTime > m.idleTime)
            {
                m.ChangeState(new WalkState(m));
                return;
            }
        }
    }

    private class WalkState : State<EnemyController>
    {
        public WalkState(EnemyController _m) : base(_m){}

        public override void OnEnter()
        {
            m.dir = Random.insideUnitCircle.normalized;
            m.anim.SetFloat("speed", 0.5f);

            m.countTime = 0;
        }

        public override void OnUpdate()
        {
            m.countTime += Time.fixedDeltaTime;

            if(m.target)
            {
                m.ChangeState(new ReadyState(m));
                return;
            }
            else if(m.countTime >= m.walkTime)
            {
                m.ChangeState(new IdleState(m));
                return;
            }

            m.transform.position += m.transform.TransformDirection(m.dir) * Time.fixedDeltaTime * m.walkSpeed;
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
        }

        public override void OnExit()
        {
            m.anim.SetFloat("speed", 0);
        }
    }

    private class ReadyState : State<EnemyController>
    {
        public ReadyState(EnemyController _m) : base(_m){}

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
            }

            if(!m.target)
            {
                m.ChangeState(new IdleState(m));
                return;
            }
            else if(m.countCoolTime <= m.coolTime & m.distance > m.closeDistance)
            {
                m.ChangeState(new ChaseState(m));
                return;
            }
            else if(m.countCoolTime > m.coolTime & m.numbers.Count > 0)
            {
                m.attackNumber = m.numbers[Random.Range(0, m.numbers.Count)];
                m.ChangeState(new AttackState(m));
                return;
            }

            m.dir = (m.target.transform.position - m.transform.position).normalized;
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
        }
    }

    private class ChaseState : State<EnemyController>
    {
        public ChaseState(EnemyController _m) : base(_m){}

        // 前の位置
        private Vector2 prePos = Vector2.zero;

        public override void OnEnter()
        {
            prePos = m.transform.position;

            m.anim.SetFloat("speed", 1);
            m.agent.isStopped = false;
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
            }

            if(!m.target)
            {
                m.ChangeState(new IdleState(m));
                return;
            }
            else if(m.countCoolTime <= m.coolTime & m.distance <= m.closeDistance)
            {
                m.ChangeState(new ReadyState(m));
                return;
            }
            else if(m.countCoolTime > m.coolTime & m.numbers.Count > 0)
            {
                m.attackNumber = m.numbers[Random.Range(0, m.numbers.Count)];
                m.ChangeState(new AttackState(m));
                return;
            }

            m.agent.speed = m.dashSpeed;
            m.agent.SetDestination(m.target.transform.position);
            if(prePos.x != m.transform.position.x | prePos.y != m.transform.position.y)
            {
                m.dir = new Vector2(m.transform.position.x - prePos.x, m.transform.position.y - prePos.y).normalized;
            }
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

        public override void OnUpdate()
        {
            if(m.isAttackEnd)
            {
                m.ChangeState(new ReadyState(m));
                return;
            }
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
}
