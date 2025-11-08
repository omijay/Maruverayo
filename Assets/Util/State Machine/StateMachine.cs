using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    public State<T> CurrentState { get; private set; }

    T _owner;
    public StateMachine(T Owner) 
    { 
        _owner = Owner;
    }
    
    public void ChangeState(State<T> newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter(_owner);
    }
    public void Execute()
    {
        CurrentState?.Execute();
    }
}
