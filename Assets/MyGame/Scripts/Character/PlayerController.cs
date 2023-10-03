using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine<PlayerController>
{
    private Rigidbody2D rb = null;
    private Animator anim = null;

    [SerializeField]
    private float walkSpeed = 0;
    [SerializeField]
    private float dashSpeed = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        ChangeState(new MoveState(this));
    }

    private void FixedUpdate()
    {
        OnUpdate();
    }

    private class MoveState : State<PlayerController>
    {
        public MoveState(PlayerController _m) : base(_m){}

        public override void OnUpdate()
        {
            if (GameManager.I.Input.actions["Attack"].WasPerformedThisFrame())
            {
                m.ChangeState(new PlayerController.AttackState(m));
                return;
            }

            Vector2 move = GameManager.I.Input.actions["Move"].ReadValue<Vector2>().normalized;
            
            if(GameManager.I.Input.actions["Dash"].IsPressed() & move.magnitude > 0)
            {
                Debug.Log("Dash");
                m.rb.velocity = move * m.dashSpeed * Time.fixedDeltaTime;
                m.anim.SetFloat("speed", 1);
                m.anim.SetFloat("x", move.x);
                m.anim.SetFloat("y", move.y);
            }
            else if(move.magnitude > 0)
            {
                Debug.Log("Walk");
                m.rb.velocity = move * m.walkSpeed * Time.fixedDeltaTime;
                m.anim.SetFloat("speed", 0.5f);
                m.anim.SetFloat("x", move.x);
                m.anim.SetFloat("y", move.y);
            }
            else
            {
                Debug.Log("Idle");
                m.rb.velocity = Vector2.zero;
                m.anim.SetFloat("speed", 0);
            }
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

        public override void OnUpdate()
        {
            Debug.Log("Attack");
        }
    }
}
