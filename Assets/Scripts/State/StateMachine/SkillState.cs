using UnityEngine;
using System.Collections;

public class SkillState : BaseState 
{

    private int _SkillIndex;
    private float _timer;

    public SkillState(StateManager stateManager, int skillIndex)
        : base(stateManager)
    {
        _SkillIndex = skillIndex;
    }

    public override void OnEnter()
    {
        base.OnEnter();
       
        if (_stateManager.controller.actionStat != Controller.ActionStat.Skill)
        {
            _stateManager.controller.OnSkillCast(_SkillIndex);            
        }
    
        _timer = Time.time;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

   
        if (_stateManager.controller.actionStat != Controller.ActionStat.Skill || Time.time - _timer > 3)
        {
            _stateManager.OnChangeState(new ActionState(_stateManager));
        }   
    }

    public override void OnExit()
    {
        base.OnExit();
    }


   
}
