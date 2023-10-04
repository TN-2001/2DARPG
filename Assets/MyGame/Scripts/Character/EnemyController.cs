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
    private float walkSpeed = 0;
    [SerializeField]
    private float dashSpeed = 0;
    [SerializeField]
    private float idleTime = 0; 
    [SerializeField]
    private float walkTime = 0;

    private Vector2 prePos = Vector2.zero;
    private Vector2 dir = Vector2.zero;
    private GameObject target = null;

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

    private void Update()
    {
        dir = new Vector2(agent.nextPosition.x - prePos.x, agent.nextPosition.y - prePos.y); 
    }

    private void LateUpdate()
    {
        prePos = transform.position;
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
        private float waitTime = 0;

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
                        waitTime = 0;
                    }

                    if(waitTime >= m.idleTime)
                    {
                        state = State.Walk;
                        isEnter = false;
                    }

                    waitTime += Time.fixedDeltaTime;

                    break;
                }
                case State.Walk:
                {
                    break;
                }
            }
        }
    }

    private class ChaseState : State<EnemyController>
    {
        public ChaseState(EnemyController _m) : base(_m){}

        public override void OnEnter()
        {
            m.target = m.serchDetector.collisionObject;
        }

        public override void OnUpdate()
        {
            m.agent.speed = m.dashSpeed;
            m.agent.SetDestination(m.target.transform.position);
            m.anim.SetFloat("speed", 1);
            m.anim.SetFloat("x", m.dir.normalized.x);
            m.anim.SetFloat("y", m.dir.normalized.y);
            m.rotation.rotation = Quaternion.FromToRotation(Vector3.up, m.dir);
        }
    }
}
