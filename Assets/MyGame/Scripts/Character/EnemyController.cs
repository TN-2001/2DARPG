using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : StateMachine<EnemyController>
{
    private NavMeshAgent agent = null;
    private Animator anim = null;

    [SerializeField]
    private GameObject target = null;
    [SerializeField]
    private float walkSpeed = 0;
    [SerializeField]
    private float dashSpeed = 0;

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

        public override void OnUpdate()
        {
            m.agent.speed = m.walkSpeed;
            m.agent.SetDestination(m.target.transform.position);
            Vector2 dir = m.agent.nextPosition - m.transform.position;
            m.anim.SetFloat("speed", 0.5f);
            m.anim.SetFloat("x", dir.normalized.x);
            m.anim.SetFloat("y", dir.normalized.y);
        }
    }
}
