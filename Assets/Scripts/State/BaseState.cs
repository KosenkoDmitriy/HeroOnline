using UnityEngine;
using System.Collections;

public abstract class BaseState : IState
{
    
    protected StateManager _stateManager { get; set; }

    public BaseState(StateManager stateManager)
        :base()
    {
        _stateManager = stateManager;
    }

    public virtual void OnEnter()
    {
       // Debug.Log("OnEnter " + GetType());
    }

    public virtual void OnUpdate()
    {
     
    }

    public virtual void OnExit()
    {
       // Debug.Log("OnExit " + GetType());
    }





}
