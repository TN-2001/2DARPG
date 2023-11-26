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

        // Stete繊維
        if(currentState != null)
        {
            for(int i = 0; i < currentState.StateList.Count; i++)
            {
                if(currentState.StateList[i].Item1)
                {
                    ChangeState(currentState.StateList[i].Item2);
                }
            }
        }

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
    // 条件分岐とステート
    public virtual List<(bool, State<T>)> StateList => new List<(bool, State<T>)>();

    // 初期化
    public State(T m) 
    { 
        this.m = m; 
    }

    // 実行される関数
    public virtual void OnEnter(){}
    public virtual void OnUpdate(){}
    public virtual void OnExit(){}
}
