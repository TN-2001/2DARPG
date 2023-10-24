using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine<PlayerController>
{
    [SerializeField] // 向きオブジェクト
    private Transform rotation = null;
    [SerializeField] // エリア判定
    private CollisionDetector areaDetector = null;
    [SerializeField] // 攻撃判定
    private CollisionDetector[] attackDetectors = null;
    [SerializeField] // 歩きスピード
    private float walkSpeed = 0;
    [SerializeField] // ダッシュスピード
    private float dashSpeed = 0;

    // 物理コンポーネント
    private Rigidbody2D rb = null;
    // アニメーションコンポーネント
    private Animator anim = null;
    [SerializeField] // キャラクター
    private Character character = null;
    // 向き
    private Vector2 dir = Vector2.zero;
    [SerializeField] // エリア内のターゲット
    private List<GameObject> targets = new List<GameObject>();
    // 攻撃番号
    private int attackNumber = 0;
    // 攻撃終了フラグ
    private bool attackEndFlag = false;

    // Updateのタイプ
    protected override Type type => Type.FixedUpdate;


    private void OnEnterArea(Collider2D other)
    {
        targets.Add(other.gameObject);
    }

    private void OnExitArea(Collider2D other)
    {
        targets.Remove(other.gameObject);
    }

    private void OnAttackHit(Collider2D other)
    {
        other.GetComponent<EnemyController>().OnDamage(character.GetAttack(attackNumber));
    }

    private void OnAttackEnd()
    {
        attackEndFlag = true;
    }

    public void OnDamage(int damage)
    {
        character.OnDamage(damage);
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        character = new Character(GameManager.I.DataBase.CharacterDatas[0]);

        areaDetector.onTriggerEnter.AddListener(OnEnterArea);
        areaDetector.onTriggerExit.AddListener(OnExitArea);

        ChangeState(new IdleState(this));
    }

    private class IdleState : State<PlayerController>
    {
        public IdleState(PlayerController _m) : base(_m){}

        public override void OnUpdate()
        {
            if(GameManager.I.Input.actions["Attack"].WasPerformedThisFrame())
            {
                m.ChangeState(new AttackState(m));
                return;
            }
            else if(GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0
                & GameManager.I.Input.actions["Dash"].IsPressed())
            {
                m.ChangeState(new DashState(m));
            }
            else if(GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0)
            {
                m.ChangeState(new WalkState(m));
                return;
            }
        }
    }

    private class WalkState : State<PlayerController>
    {
        public WalkState(PlayerController _m) : base(_m){}

        public override void OnEnter()
        {
            m.anim.SetFloat("speed", 0.5f);
        }

        public override void OnUpdate()
        {
            if(GameManager.I.Input.actions["Attack"].WasPerformedThisFrame())
            {
                m.ChangeState(new AttackState(m));
                return;
            }
            else if(GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude == 0)
            {
                m.ChangeState(new IdleState(m));
                return;
            }
            else if(GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0
                & GameManager.I.Input.actions["Dash"].IsPressed())
            {
                m.ChangeState(new DashState(m));
            }

            m.dir = GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized;
            m.rb.velocity = m.dir * m.walkSpeed;
            m.anim.SetFloat("x", m.dir.x);
            m.anim.SetFloat("y", m.dir.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
        }

        public override void OnExit()
        {
            m.rb.velocity = Vector2.zero;
            m.anim.SetFloat("speed", 0);
        }
    }

    private class DashState : State<PlayerController>
    {
        public DashState(PlayerController _m) : base(_m){}

        public override void OnEnter()
        {
            m.anim.SetFloat("speed", 1);
        }

        public override void OnUpdate()
        {
            if(GameManager.I.Input.actions["Attack"].WasPerformedThisFrame())
            {
                m.ChangeState(new AttackState(m));
                return;
            }
            else if(GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude == 0)
            {
                m.ChangeState(new IdleState(m));
                return;
            }
            else if(GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0
                & !GameManager.I.Input.actions["Dash"].IsPressed())
            {
                m.ChangeState(new WalkState(m));
                return;
            }

            m.dir = GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized;
            m.rb.velocity = m.dir * m.dashSpeed;
            m.anim.SetFloat("x", m.dir.x);
            m.anim.SetFloat("y", m.dir.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
        }

        public override void OnExit()
        {
            m.rb.velocity = Vector2.zero;
            m.anim.SetFloat("speed", 0);
        }
    }

    private class AttackState : State<PlayerController>
    {
        public AttackState(PlayerController _m) : base(_m){}

        private GameObject target = null;

        public override void OnEnter()
        {
            if(m.targets.Count > 0)
            {
                float dis = 100;
                for(int i = 0; i < m.targets.Count; i++)
                {
                    if(Vector3.Distance(m.targets[i].transform.position, m.transform.position) < dis)
                    {
                        target = m.targets[i];
                        dis = Vector3.Distance(m.targets[i].transform.position, m.transform.position);
                    }
                }
                m.dir = (target.transform.position - m.transform.position).normalized;
                m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
                m.anim.SetFloat("x", m.dir.normalized.x);
                m.anim.SetFloat("y", m.dir.normalized.y);
            }
            m.attackDetectors[m.attackNumber].onTriggerEnter.AddListener(m.OnAttackHit);
            m.attackDetectors[m.attackNumber].gameObject.SetActive(true);
            m.anim.SetFloat("attackNumber", m.attackNumber + 1);
            m.anim.SetTrigger("isAttack");
        }

        public override void OnUpdate()
        {
            if(m.attackEndFlag)
            {
                m.ChangeState(new IdleState(m));
                return;
            }
        }

        public override void OnExit()
        {
            m.attackEndFlag = false;
            m.anim.SetFloat("attackNumber", 0);
            m.attackDetectors[m.attackNumber].gameObject.SetActive(false);
            m.attackDetectors[m.attackNumber].onTriggerEnter.RemoveAllListeners();
        }
    }
}
