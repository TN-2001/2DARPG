using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<T> : MonoBehaviour where T : StateMachine<T>
{
    // State変数
    private State<T> currentState = null;
    private State<T> nextState = null;
    [SerializeField]
    private string state = "";

    // Updateのタイプ
    protected enum Type
    {
        Update,
        FixedUpdate,
    }
    protected virtual Type type => Type.Update;

    // Stateの遷移時に呼ぶ
    protected void ChangeState(State<T> _nextState)
    {
        // 次のStateを保存
        nextState = _nextState;
        state = nextState.ToString();
        state = state.Substring(state.IndexOf("+") + 1);
    }

    // Stateを実行
    private void OnUpdate()
    {
        // Stateを実行
        currentState?.OnUpdate();

        if(nextState != null)
        {
            currentState?.OnExit();

            currentState = nextState;
            currentState.OnEnter();
            nextState = null;
        }
    }

    private void Update()
    {
        if(type == Type.Update)
        {
            OnUpdate();
        }
    }

    private void FixedUpdate()
    {
        if(type == Type.FixedUpdate)
        {
            OnUpdate();
        }
    }
}

public abstract class State<T> where T : StateMachine<T>
{
    // TはStateMachineの方
    protected T m = null;

    // 初期化
    public State(T _m) 
    { 
        m = _m; 
    }

    // 実行される関数
    public virtual void OnEnter(){}
    public virtual void OnUpdate(){}
    public virtual void OnExit(){}
}
