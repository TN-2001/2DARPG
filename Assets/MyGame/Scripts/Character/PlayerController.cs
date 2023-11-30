using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine<PlayerController>, IBattlerController
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

    // 物理コンポーネント
    private Rigidbody2D rb = null;
    // アニメーションコンポーネント
    private Animator anim = null;
    [SerializeField, ReadOnly] // キャラクター
    private Player player = null;
    // 向き
    private Vector2 dir = Vector2.zero;
    [SerializeField, ReadOnly] // エリア内のターゲット
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField, ReadOnly] // 攻撃番号
    private int attackNumber = 0;
    // 攻撃終了フラグ
    private bool isAttackEnd = false;
    // ガードフラグ
    private bool isGuard = false;


    private void OnEnterArea(Collider2D other)
    {
        targets.Add(other.gameObject);
    }

    private void OnExitArea(Collider2D other)
    {
        targets.Remove(other.gameObject);
    }

    public void OnAttackEnd()
    {
        isAttackEnd = true;
    }

    public void OnDamage(int damage)
    {
        if(!isGuard)
        {
            player.OnDamage(damage);
            GameUI.I.UpdateHpSlider(player.CurrentHp);
        }
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        player = new Player(GameManager.I.DataBase.PlayerData);

        areaDetector.onTriggerEnter.AddListener(OnEnterArea);
        areaDetector.onTriggerExit.AddListener(OnExitArea);
        for(int i = 0; i < attackControllers.Length; i++)
        {
            attackControllers[i].Initialize(player.Atk);
        }

        GameUI.I.InitializeHpSlider(player.Hp);

        ChangeState(new IdleState(this));
    }

    private class IdleState : State<PlayerController>
    {
        public IdleState(PlayerController _m) : base(_m){}

        public override List<(bool, State<PlayerController>)> StateList => new List<(bool, State<PlayerController>)>()
        {
            (m.player.CurrentHp == 0, new DieState(m)),
            (GameManager.I.Input.actions["Attack"].IsPressed() || GameManager.I.Input.actions["Skill"].IsPressed(), new AttackState(m)),
            (GameManager.I.Input.actions["Guard"].IsPressed(), new GuardState(m)),
            (GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0, new MoveState(m))
        };

        public override void OnUpdate()
        {
            if(GameManager.I.Input.actions["Attack"].IsPressed())
            {
                m.attackNumber = 0;
            }
            else if(GameManager.I.Input.actions["Skill"].IsPressed())
            {
                m.attackNumber = 1;
            }
        }
    }

    private class MoveState : State<PlayerController>
    {
        public MoveState(PlayerController _m) : base(_m){}

        public override List<(bool, State<PlayerController>)> StateList => new List<(bool, State<PlayerController>)>()
        {
            (m.player.CurrentHp == 0, new DieState(m)),
            (GameManager.I.Input.actions["Attack"].IsPressed() || GameManager.I.Input.actions["Skill"].IsPressed(), new AttackState(m)),
            (GameManager.I.Input.actions["Guard"].IsPressed(), new GuardState(m)),
            (GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude == 0, new IdleState(m))
        };

        public override void OnUpdate()
        {
            if(GameManager.I.Input.actions["Attack"].IsPressed())
            {
                m.attackNumber = 0;
            }
            else if(GameManager.I.Input.actions["Skill"].IsPressed())
            {
                m.attackNumber = 1;
            }

            // 向き取得
            if(GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0)
            {
                m.dir = GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized;
            }
            // 移動
            if(GameManager.I.Input.actions["Dash"].IsPressed())
            {
                m.rb.velocity = m.dir * m.dashSpeed;
                m.anim.SetFloat("speed", 1f);
            }
            else
            {
                m.rb.velocity = m.dir * m.walkSpeed;
                m.anim.SetFloat("speed", 0.5f);
            }
            // 向き
            m.anim.SetFloat("x", m.dir.x);
            m.anim.SetFloat("y", m.dir.y);
            Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, m.dir);
            Vector3 vector3 = quaternion.eulerAngles;
            vector3.y = 0;
            m.rotation.rotation = Quaternion.Euler(vector3);
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

        public override List<(bool, State<PlayerController>)> StateList => new List<(bool, State<PlayerController>)>()
        {
            (m.player.CurrentHp == 0, new DieState(m)),
            (m.isAttackEnd, new IdleState(m))
        };

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
                Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, m.dir);
                Vector3 vector3 = quaternion.eulerAngles;
                vector3.y = 0;
                m.rotation.rotation = Quaternion.Euler(vector3);
                m.anim.SetFloat("x", m.dir.normalized.x);
                m.anim.SetFloat("y", m.dir.normalized.y);
            }

            if(m.attackControllers[m.attackNumber].IsThrow)
            {
                GameObject obj = Instantiate(m.attackControllers[m.attackNumber].gameObject, 
                    m.attackControllers[m.attackNumber].transform.position,
                    m.attackControllers[m.attackNumber].transform.rotation);
                obj.transform.SetParent(m.transform.parent);
                obj.SetActive(true);
            }

            m.anim.SetFloat("attackNumber", m.attackNumber + 1);
            m.anim.SetTrigger("isAttack");
        }

        public override void OnExit()
        {
            m.isAttackEnd = false;
            m.anim.SetFloat("attackNumber", 0);
        }
    }

    private class GuardState : State<PlayerController>
    {
        public GuardState(PlayerController _m) : base(_m){}

        public override List<(bool, State<PlayerController>)> StateList => new List<(bool, State<PlayerController>)>()
        {
            (m.player.CurrentHp == 0, new DieState(m)),
            (!GameManager.I.Input.actions["Guard"].IsPressed(), new IdleState(m))
        };

        public override void OnEnter()
        {
            m.anim.SetBool("isGuard", true);
            m.isGuard = true;
        }

        public override void OnExit()
        {
            m.anim.SetBool("isGuard", false);
            m.isGuard = false;
        }
    }

    private class DieState : State<PlayerController>
    {
        public DieState(PlayerController _m) : base(_m){}
    }
}
