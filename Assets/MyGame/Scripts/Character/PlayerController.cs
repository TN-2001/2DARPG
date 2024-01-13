using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : StateMachine<PlayerController>, IBattlerController
{
    protected override Type type => Type.FixedUpdate;

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

    // 物理コンポーネント
    private Rigidbody2D rb = null;
    // アニメーションコンポーネント
    private Animator anim = null;
    // PlayerInput
    private PlayerInput input = null;
    [SerializeField, ReadOnly] // キャラクター
    private Player player = null;
    // 向き
    private Vector2 dir = Vector2.zero;
    [SerializeField, ReadOnly] // エリア内のターゲット
    private List<GameObject> targets = new List<GameObject>();
    [SerializeField, ReadOnly] // イベントディテクター
    private EventController eventController = null;
    [SerializeField, ReadOnly] // 攻撃番号
    private int attackNumber = 0;
    // 攻撃終了フラグ
    private bool isAttackEnd = false;


    public void OnAttackEnd()
    {
        isAttackEnd = true;
    }

    public void OnDamage(int damage)
    {
        player.OnDamage(damage);
        DungeonUI.I.UpdateHpSlider(player.CurrentHp);
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();

        player = new Player(GameManager.I.DataBase.PlayerData);

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

        for(int i = 0; i < attackControllers.Length; i++)
        {
            attackControllers[i].Initialize(player.Atk);
        }

        DungeonUI.I?.InitializeHpSlider(player.Hp);

        GameManager.I.CameraController.Initialize(transform);

        ChangeState(new IdleState(this));
    }

    private class DoState : State<PlayerController>
    {
        public DoState(PlayerController _m) : base(_m){}

        public override List<(bool, State<PlayerController>)> StateList => new List<(bool, State<PlayerController>)>()
        {
            (GameManager.I.state == GameManager.State.Player, new IdleState(m)),
        };

        public override void OnEnter()
        {
            GameManager.I.state = GameManager.State.UI;
            m.eventController.Do();
        }
    }

    private class IdleState : State<PlayerController>
    {
        public IdleState(PlayerController _m) : base(_m){}

        public override List<(bool, State<PlayerController>)> StateList => new List<(bool, State<PlayerController>)>()
        {
            (m.input.actions["Do"].IsPressed() & m.eventController, new DoState(m)),
            (m.player.CurrentHp == 0, new DieState(m)),
            (m.input.actions["Attack"].IsPressed() || m.input.actions["Skill"].IsPressed(), new AttackState(m)),
            (m.input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0, new MoveState(m))
        };

        public override void OnUpdate()
        {
            if(m.input.actions["Attack"].IsPressed())
            {
                m.attackNumber = 0;
            }
            else if(m.input.actions["Skill"].IsPressed())
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
            (m.input.actions["Do"].IsPressed() & m.eventController, new DoState(m)),
            (m.player.CurrentHp == 0, new DieState(m)),
            (m.input.actions["Attack"].IsPressed() || m.input.actions["Skill"].IsPressed(), new AttackState(m)),
            (m.input.actions["Move"].ReadValue<Vector2>().normalized.magnitude == 0, new IdleState(m))
        };

        public override void OnUpdate()
        {
            if(m.input.actions["Attack"].IsPressed())
            {
                m.attackNumber = 0;
            }
            else if(m.input.actions["Skill"].IsPressed())
            {
                m.attackNumber = 1;
            }

            // 向き取得
            if(m.input.actions["Move"].ReadValue<Vector2>().normalized.magnitude > 0)
            {
                m.dir = m.input.actions["Move"].ReadValue<Vector2>().normalized;
            }
            // 移動
            if(m.input.actions["Dash"].IsPressed())
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
            (m.input.actions["Do"].IsPressed() & m.eventController, new DoState(m)),
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

    private class DieState : State<PlayerController>
    {
        public DieState(PlayerController _m) : base(_m){}
    }
}
