using UnityEngine;
using System.Collections;
using System.Linq;
using DEngine.Common.GameLogic;

public class IdleState : BaseState {

    private float _timerRunAway;

    public IdleState(StateManager stateManager)
        : base(stateManager)
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();
        _stateManager.controller.OnIdle();
        _timerRunAway = Time.time;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_stateManager.controller.role.Base.Type == DEngine.Common.GameLogic.RoleType.Hostage)
        {
            HandleUpdateHostage();
            return;
        }

        if (_stateManager.controller.typeCharacter == Controller.TypeChatacter.Healer)
        {
            HandleUpdateHealer();
        }
        else
        {        
            HandleUpdateMeleeRanger();
        }

    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void HandleUpdateHostage()
    {
        if (Time.time - _timerRunAway >= 5)
        {
            _timerRunAway = Time.time;
            _stateManager.OnChangeState(new RunAwayState(_stateManager));
        }
    }
    private void HandleUpdateHealer()
    {
        Controller controllerTarget = _stateManager.FindAllyMiniumHP();      
       
        if (controllerTarget != null)
        {
            _stateManager.controller.target = controllerTarget.gameObject;
            _stateManager.OnChangeState(new ActionState(_stateManager));
        }
    }
    private void HandleUpdateMeleeRanger()
    {
        Controller controllerTarget;

        if (_stateManager.controller.isMob)
            controllerTarget = _stateManager.FindEnemyNearest();
        else
            controllerTarget = _stateManager.FindEnemyStrategy();

        if (controllerTarget != null)
        {
            _stateManager.controller.target = controllerTarget.gameObject;
            _stateManager.OnChangeState(new ActionState(_stateManager));
        }
    }

  

   
}
