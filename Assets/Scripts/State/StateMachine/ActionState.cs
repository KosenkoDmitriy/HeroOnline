using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;

public class ActionState : BaseState
{

  
    private float _timerTargetHealer;

    public ActionState(StateManager stateManager)
        : base(stateManager)
    {
    }

    //call action on controller
    public override void OnEnter()
    {
        base.OnEnter();

        if (_stateManager.controller.target != null)
        {
            _stateManager.controller.OnAction(_stateManager.controller.target);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        HandleUpdate();

    }

    public override void OnExit()
    {
        base.OnExit();
    }


    private void HandleUpdate()
    {
        if (_stateManager.controller == null) return;

        if (_stateManager.controller.autoSkill)
            if (PlaySkill()) return;

        //if (_stateManager.controller.hp / _stateManager.controller.maxhp <= 0.3f
        //      && Time.time - _stateManager.timerLastRun > 10
        //      && _stateManager.controller.typeCharacter != Controller.TypeChatacter.Melee
        //      && _stateManager.hasHealer())
        //{
        //    _stateManager.OnChangeState(new RunAwayState(_stateManager));
        //    return;
        //}

        if (_stateManager.controller.target == null)
        {
            _stateManager.OnChangeState(new IdleState(_stateManager));
            return;
        }

        if (_stateManager.controller.target.GetComponent<Controller>().actionStat == Controller.ActionStat.Dead)
        {
            _stateManager.OnChangeState(new IdleState(_stateManager));
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
    
    private void HandleUpdateHealer()
    {      

        Controller controllerTarget = _stateManager.FindAllyMiniumHP();              

        if (controllerTarget != null)
        {                       
            _stateManager.controller.OnAction(controllerTarget.gameObject);
        }
        else
        {
            _stateManager.OnChangeState(new IdleState(_stateManager));
        }
    }

    private void HandleUpdateMeleeRanger()
    {
        if (_stateManager.controller.target == null
            || _stateManager.controller.actionStat == Controller.ActionStat.Idle)
        {
            _stateManager.OnChangeState(new IdleState(_stateManager));
            return;
        }


        if (_stateManager.controller.isMob)
        {
            Controller enemyTargetToMe = _stateManager.FindEnnemyTargetToMe();
            if (enemyTargetToMe != null)
            {
                //send message to server (SubCode.Action) , this role wants to impact target
                _stateManager.controller.OnAction(enemyTargetToMe.gameObject);
            }
        }
       
    }

    private bool PlaySkill()
    {
        if (_stateManager.controller.actionStat != Controller.ActionStat.Action)
        {
            return false;
        }

        List<RoleSkill> roleSkills = _stateManager.controller.role.RoleSkills.Where(p => p.GameSkill.SkillType != (int)SkillType.Aura).ToList();

        if (roleSkills.Count <= 1) return false;


        if (_stateManager.controller.actionStat != Controller.ActionStat.Skill)
        {
            if (Time.time - _stateManager.timerSkillLast > 1)
            {

                for (int i = 1; i < roleSkills.Count; i++)
                {
                    RoleSkill roleSkill = roleSkills[i];
                    if (GameManager.autoSkillConfig.CheckUseSkill(_stateManager.controller, roleSkill))
                    {
                        int skillIndex = _stateManager.controller.role.RoleSkills.IndexOf(roleSkill);
                        _stateManager.OnChangeState(new SkillState(_stateManager, skillIndex));
                        _stateManager.timerSkillLast = Time.time;
                        return true;
                    }
                }
                             
            }
        }

        return false;
    }

}
