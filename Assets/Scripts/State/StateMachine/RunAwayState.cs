using UnityEngine;
using System.Collections;

public class RunAwayState : BaseState 
{
    private WayPointController _waypoint;

    private float _timer;

    public RunAwayState(StateManager stateManager)
        : base(stateManager)
    {
        _stateManager = stateManager;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        if (_stateManager.controller.role.Base.Type == DEngine.Common.GameLogic.RoleType.Hostage)
            _waypoint = WayPointSet.Instance.FindWayPointRandom(_stateManager.controller);
        else
            _waypoint = WayPointSet.Instance.FindWayPointNoEnemy(_stateManager.controller);


        if (_waypoint != null && _stateManager.controller != null)
            _stateManager.controller.OnMove(_waypoint.transform.position);
        else
        {
            Debug.Log(" RunAwayState OnEnter => Null");
        }
        _stateManager.timerLastRun = Time.time;
        _timer = Time.time;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        float distace = Vector3.Distance(_stateManager.controller.transform.position, _waypoint.transform.position);

        if (distace <= 1f || Time.time - _timer >= 2)
        {
            _stateManager.OnChangeState(new IdleState(_stateManager));
        }   
    }

    public override void OnExit()
    {
        base.OnExit();
    }


   
}
