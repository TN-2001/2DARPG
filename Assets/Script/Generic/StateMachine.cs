using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> : MonoBehaviour where T : StateMachine<T>
{
    // State変数
    private State<T> currentState = null;
    private State<T> nextState = null;

    // Stateの遷移時に呼ぶ
    protected void ChangeState(State<T> _nextState)
    {
        // 次のStateを保存
        nextState = _nextState;
    }

    // Stateを実行したい場所に呼ぶ
    protected void OnUpdate()
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
}

public class State<T> where T : StateMachine<T>
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
