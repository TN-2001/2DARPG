using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : StateMachine<EnemyController>
{
    private NavMeshAgent agent = null;
    private Animator anim = null;

    [SerializeField]
    private Transform rotation = null;
    [SerializeField]
    private CollisionDetector serchDetector = null;
    [SerializeField]
    private float chaseDistance = 0;
    [SerializeField]
    private float attackDistance = 0;
    [SerializeField]
    private float walkSpeed = 0;
    [SerializeField]
    private float dashSpeed = 0;
    [SerializeField]
    private float idleTime = 0; 
    [SerializeField]
    private float walkTime = 0;

    private Vector2 dir = Vector2.zero;
    private GameObject target = null;
    private float distance = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        ChangeState(new MoveState(this));
    }

    private void FixedUpdate()
    {
        OnUpdate();
    }

    private class MoveState : State<EnemyController>
    {
        public MoveState(EnemyController _m) : base(_m){}

        private enum State
        {
            Idle,
            Walk
        }
        private State state = State.Idle;
        private bool isEnter = false;
        private float countTime = 0;

        public override void OnEnter()
        {
            m.target = null;
            m.distance = 0;
        }

        public override void OnUpdate()
        {
            if(m.serchDetector.isCollision)
            {
                m.ChangeState(new ChaseState(m));
            }

            switch(state)
            {
                case State.Idle:
                {
                    if(!isEnter)
                    {
                        isEnter = true;
                        countTime = 0;
                        m.anim.SetFloat("speed", 0);
                    }

                    if(countTime >= m.idleTime)
                    {
                        state = State.Walk;
                        isEnter = false;
                    }

                    countTime += Time.fixedDeltaTime;

                    break;
                }
                case State.Walk:
                {
                    if(!isEnter)
                    {
                        isEnter = true;
                        countTime = 0;
                        m.dir = Random.insideUnitCircle.normalized;
                        m.anim.SetFloat("speed", 0.5f);
                    }

                    if(countTime >= m.walkTime)
                    {
                        state = State.Idle;
                        isEnter = false;
                    }

                    m.transform.position += 
                        m.transform.TransformDirection(m.dir) * Time.fixedDeltaTime * m.walkSpeed;
                    m.anim.SetFloat("x", m.dir.normalized.x);
                    m.anim.SetFloat("y", m.dir.normalized.y);
                    m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);

                    countTime += Time.fixedDeltaTime;

                    break;
                }
            }
        }

        public override void OnExit()
        {
        }
    }

    private class ChaseState : State<EnemyController>
    {
        public ChaseState(EnemyController _m) : base(_m){}

        private Vector2 prePos = Vector2.zero;

        public override void OnEnter()
        {
            m.agent.isStopped = false;
            if(!m.target)
            {
                m.target = m.serchDetector.collisionObject;
            }
            prePos = m.transform.position;
        }

        public override void OnUpdate()
        {
            m.distance = Vector3.Distance(m.target.transform.position, m.transform.position);

            if(m.chaseDistance < m.distance)
            {
                m.ChangeState(new MoveState(m));
            }
            else if(m.attackDistance > m.distance)
            {
                m.ChangeState(new WaitState(m));
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

    private class WaitState : State<EnemyController>
    {
        public WaitState(EnemyController _m) : base(_m){}

        public override void OnUpdate()
        {
            m.distance = Vector3.Distance(m.target.transform.position, m.transform.position);

            if(m.attackDistance + 0.1 < m.distance)
            {
                m.ChangeState(new ChaseState(m));
            }
        }
    }
}
