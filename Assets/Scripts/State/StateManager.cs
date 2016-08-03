using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DEngine.Common.GameLogic;

public class StateManager : MonoBehaviour {

    public IState curState { get; set; }
    public Controller controller { get; set; }
    public Controller oldTarget { get; set; }

    public List<Controller> allySet { get; set; }
    public List<Controller> enemySet { get; set; }

    public string tagAlly { get; set; }
    public string tagEnemy { get; set; }


    private float _timer { get; set; }
    private float _timerStart { get; set; }
    
    public float timerSkillLast { get; set; }
    public float timerLastRun { get; set; }
    public float timerLastTargetHealer { get; set; }

    void Awake()
    {
        //OnInit();
    }

    

	void Start () {
        _timer = Time.time;
        _timerStart = Time.time;
        timerLastRun = Time.time;
        timerSkillLast = Time.time;
        timerLastTargetHealer = Random.Range(-10, -30);
	}


    void Update()
    {
        if (!controller.isControl()  || controller.isLockAuto) return;

        if (controller.isMob)
        {
            if (Time.time - _timerStart < 1) return;
        }

        if (GameplayManager.battleStatus != GameplayManager.BattleStatus.Playing && !controller.isMobTutorial) return;

        if (controller.actionStat == Controller.ActionStat.Move 
            || controller.actionStat == Controller.ActionStat.hit 
            || controller.actionStat == Controller.ActionStat.Skill) return;

        if (!GetComponent<Animation>().IsPlaying(controller.action.name)
            || (GetComponent<Animation>().IsPlaying(controller.action.name) && GetComponent<Animation>()[controller.action.name].normalizedTime >= 0.8f))
        {
            if (curState != null && Time.time - _timer > 0.1f)
            {
                // Debug.Log("Update");

                //call update of current state
                if (controller != null && controller.actionStat != Controller.ActionStat.Dead && controller.actionStat != Controller.ActionStat.Move)
                    curState.OnUpdate();

                _timer = Time.time;
            }
        }
    }
    //Init state 
    public void OnInit()
    {
        controller = GetComponent<Controller>();

        if (tag == GameManager.EnemyTagName)
        {
            tagAlly = GameManager.EnemyTagName;
            tagEnemy = GameManager.PlayerTagName;

            allySet = (from item in GameplayManager.Instance.heroEnemySet select item.controller).ToList();
            enemySet = (from item in GameplayManager.Instance.heroPlayerSet select item.controller).ToList();
        }
        else
        {
            tagEnemy = GameManager.EnemyTagName;
            tagAlly = GameManager.PlayerTagName;

            allySet = (from item in GameplayManager.Instance.heroPlayerSet select item.controller).ToList();
            enemySet = (from item in GameplayManager.Instance.heroEnemySet select item.controller).ToList();
        }

        OnChangeState(new IdleState(this));

    }

    public void OnChangeState(IState newState)
    {
        if (curState != null)
            curState.OnExit();

        curState = newState;

        curState.OnEnter();
    }

    public void Reset()
    {
        OnChangeState(new IdleState(this));
    }

    #region Mehthods Sate
    public int FindEnemyAlive()
    {
        return enemySet.Count(p => p != null && p.actionStat != Controller.ActionStat.Dead);
    }
    public List<Controller> FindAllySetHPLess(float hpPercent)
    {       
        return allySet.Where(p => p!=null && p.actionStat != Controller.ActionStat.Dead &&  p.hp / p.maxhp < hpPercent ).ToList();
    }
    public int GetSkillIndex(TargetType targetType)
    {
        int SkillID = 0;
        switch (targetType)
        {
            case TargetType.AllyGroup:
                SkillID = 2;
                break;
            case TargetType.AllyOne:
                SkillID = 1;
                break;
            case TargetType.AreaEffect:
                SkillID = 1;
                break;
            case TargetType.EnemyGroup:
                SkillID = 2;
                break;
            case TargetType.EnemyOne:
                SkillID = 1;
                break;
            case TargetType.Self:
                SkillID = 1;
                break;
        }
        return SkillID;
    }
    public Controller FindEnnemyTargetToMe()
    {
        return controller.activeEnemy;      
    }
    public Controller FindAllyMiniumHP()
    {
        float minHP = float.MaxValue;
        Controller controllerTarget = null;
        foreach (Controller con in allySet)
        {
            if (con != null)
            {
                if (con.actionStat != Controller.ActionStat.Dead                 
                    && con.hp < con.maxhp)
                {
                    float hp = con.hp / con.maxhp;
                    if (hp < minHP && hp < 1.0f)
                    {
                        minHP = hp;
                        controllerTarget = con;
                    }
                }
            }
        }
        return controllerTarget;
    }
    public Controller FindAllyNearest()
    {
        //return null;
        float minDistace = float.MaxValue;
        Controller controllerTarget = null;
        foreach (Controller con in allySet)
        {
            if (con != null)
            {
                if (con.actionStat != Controller.ActionStat.Dead  && con.name !=controller.name)
                {              
                    float distance = Vector3.Distance(controller.transform.position, con.transform.position);
                    if (distance <= minDistace)
                    {                     

                        if (distance < minDistace)
                        {
                            minDistace = distance;
                            controllerTarget = con;
                        }
                    }
                }
            }
        }
        return controllerTarget;
    }
    public Controller FindEnemyNearest(bool attackHealer = false, float maxRange = float.MaxValue)
    {
        //return null;
        float minDistace = float.MaxValue;
        Controller controllerTarget = null;
        foreach (Controller con in enemySet)
        {
            if (con != null)
            {
                if (con.actionStat != Controller.ActionStat.Dead)
                {
                    if (controller.isMob)
                    {
                        if (con.role.Base.Type == DEngine.Common.GameLogic.RoleType.Hostage)
                        {
                            //if (Random.Range(0, 2) == 1)
                            //{
                            return con;
                            //}
                        }
                    }

                    float distance = Vector3.Distance(controller.transform.position, con.transform.position);
                    if (distance <= maxRange)
                    {                    
                        if (con.typeCharacter == Controller.TypeChatacter.Healer && !controller.isMob)
                            if (!attackHealer)
                                distance += 1000;
                          

                        if (distance < minDistace)
                        {
                            minDistace = distance;
                            controllerTarget = con;
                        }
                    }
                }
            }
        }
        return controllerTarget;
    }
    public bool hasHealer()
    {
        foreach (Controller ally in allySet)
        {
            if (ally != null)
                if (ally.typeCharacter == Controller.TypeChatacter.Healer)
                    return true;
        }
        return false;
    }
    public Controller FindEnemyTargetToHealer()
    {              
        foreach (Controller con in enemySet)
        {
            if (con != null)
            {
                if (con.actionStat != Controller.ActionStat.Dead)
                {
                    GameObject target = con.target;
                    if (target != null)
                    {
                        if (target.GetComponent<Controller>().typeCharacter == Controller.TypeChatacter.Healer
                           && target.tag == controller.tag
                            )
                            return con;
                    }
                }
            }
        }
        return null;
    }
    public Controller FindHealerOfEnemy()
    {
        foreach (Controller con in enemySet)
        {
            if (con != null)
            {
                if (con.actionStat != Controller.ActionStat.Dead)
                {
                    if (con.typeCharacter == Controller.TypeChatacter.Healer)
                        return con;
                }
            }
        }
        return null;
    }

    #region Strategy
    public Controller FindEnemyStrategy()
    {
        if (!controller.isConnectedServer())
            return FindEnemyNearest();

        //Debug.Log(gameObject.name + " Strategy: " + (Strategy)controller.role.Base.AIMode);

        switch ((Strategy)controller.role.Base.AIMode)
        {
            case Strategy.MaxHP:
                return FindEnemyMaxHPValue();
            case Strategy.MinHP:
                return FindEnemyMinHPValue();

            case Strategy.MaxPDefence:
                return FindMaxPDefence();
           // case Strategy.MaxMDefence:
           //     return FindMaxMDefence();
            case Strategy.MinPDefence:
                return FindMinPDefence();
           // case Strategy.MinMDefence:
           //     return FindMinMDefence();

            case Strategy.MaxPDamage:
                return FindMaxPDamage();
            //case Strategy.MaxMDamage:
            //    return FindMaxMDamage();
            case Strategy.MinPDamage:
                return FindMinPDamage();
            //case Strategy.MinMDamage:
             //   return FindMinMDamage();

            case Strategy.MaxMP:
                return FindEnemyMaxMPValue();
            case Strategy.MinMP:
                return FindEnemyMinMPValue();
            default:
                return FindEnemyNearest();
        }
    } 
    public Controller FindEnemyMaxHPValue()
    {      
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderByDescending(p => p.role.Attrib.MaxHP).FirstOrDefault();
    }
    public Controller FindEnemyMinHPValue()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderBy(p => p.role.Attrib.MaxHP).FirstOrDefault();
    }
    public Controller FindMaxPDefence()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderByDescending(p => p.role.Attrib.DefenceValue).FirstOrDefault();
    }
    /*public Controller FindMaxMDefence()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderByDescending(p => p.role.Attrib.MDefence).FirstOrDefault();
    }*/
    public Controller FindMaxPDamage()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderByDescending(p => p.role.Attrib.AttackValue).FirstOrDefault();
    }
    /*public Controller FindMaxMDamage()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderByDescending(p => p.role.Attrib.MAttack).FirstOrDefault();
    }*/
    public Controller FindMinPDefence()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderBy(p => p.role.Attrib.DefenceValue).FirstOrDefault();
    }
    /*public Controller FindMinMDefence()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderBy(p => p.role.Attrib.MDefence).FirstOrDefault();
    }*/
    public Controller FindMinPDamage()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderBy(p => p.role.Attrib.AttackValue).FirstOrDefault();
    }
    /*public Controller FindMinMDamage()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderBy(p => p.role.Attrib.MAttack).FirstOrDefault();
    }*/
    public Controller FindEnemyMaxMPValue()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderByDescending(p => p.role.Attrib.MaxMP).FirstOrDefault();
    }
    public Controller FindEnemyMinMPValue()
    {
        return enemySet.Where(p => p != null && p.actionStat != Controller.ActionStat.Dead).OrderBy(p => p.role.Attrib.MaxMP).FirstOrDefault();
    }
    #endregion

    #endregion
}
