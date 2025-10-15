using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class NestedStateMachine<TMachineStateEnum, TStateEnum> : StateMachine<TMachineStateEnum>, IState<TStateEnum>  where TMachineStateEnum : Enum where TStateEnum : Enum
{
    // IState
    public abstract TStateEnum Tag { get; }
    public StateMachine<TStateEnum> Machine { get; set;}

    public abstract void Enter();

    public abstract void Exit();
    
    public sealed override void ChangeState(IState<TMachineStateEnum> newState)
    {
        if (newState != State)
        {
            Machine.ChangeState(this);

            MachineExit();
            State = newState;
            MachineEnter();
        }
    }
}

public abstract class StateMachine<TStateEnum> where TStateEnum : Enum
{
    protected abstract IEnumerable<IState<TStateEnum>> States { get; }

    private IState<TStateEnum> state;

    public IState<TStateEnum> State
    {
        get => state;
        set
        {
            state = value;

            if (state is not null)
            {
                Debug.Log(state.Tag);
            }
            else
            {
                Debug.Log(null);
            }
        }
    }

    private Dictionary<TStateEnum, IState<TStateEnum>> stateTable = new Dictionary<TStateEnum, IState<TStateEnum>>();
    
    protected StateMachine()
    {
        foreach (var state in States)
        {
            state.Machine = this;
            stateTable.Add(state.Tag, state);
        }
    }

    public void ChangeState(TStateEnum newStateTag)
    {
        stateTable.TryGetValue(newStateTag, out var state);
        ChangeState(state);
    }
    
    public virtual void ChangeState(IState<TStateEnum> newState)
    {
        if (newState != state)
        {
            MachineExit();
            State = newState;
            MachineEnter();
        }
    }

    public void MachineEnter()
    {
        if (state != null)
        {
            state.Enter();
            if (state is StateMachine<TStateEnum> stateMachine)
            {
                stateMachine.MachineEnter();
            }
        }
    }

    public void MachineExit()
    {
        if (state != null)
        {
            if (state is StateMachine<TStateEnum> stateMachine)
            {
                stateMachine.MachineExit();
            }
            state.Exit();
        }
    }

    private void MachineUpdate(float deltaTime)
    {
        if (state != null)
        {
            state.Update(deltaTime);
        }
    }

    protected abstract void InternalUpdate(float deltaTime);

    public void Update(float deltaTime)
    {
        InternalUpdate(deltaTime);
        MachineUpdate(deltaTime);
    }
}

public interface IState<T> where T: Enum
{
    public T Tag {get;}
    public StateMachine<T> Machine {set;}
    public void Enter();
    public void Exit();
    public void Update(float deltaTime);
}

