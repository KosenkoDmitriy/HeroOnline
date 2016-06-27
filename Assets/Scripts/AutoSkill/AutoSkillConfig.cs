using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;

public class AutoSkillConfig
{
    public struct Rule
    {
        public TargetType targetType;
        public AttribType attribType;
        public Condition.ConditionType condition;
        public float value;
    }

    public Dictionary<int,List<Rule>> skills;

    private string _pathEffectInfo;

    public AutoSkillConfig()
    {
        skills = new Dictionary<int, List<Rule>>();
        _pathEffectInfo = "Text/AutoSkill";
        LoadFileConfig();
    }

    private void LoadFileConfig()
    {
        TextAsset text = Resources.Load(_pathEffectInfo) as TextAsset;
        string[] lines = text.text.Split('\n');

        foreach (string line in lines)
        {
            string[] s = line.Split('\t');
            if (s.Length >= 2)
            {
                List<Rule> rules = new List<Rule>();
                int id = int.Parse(s[0]);
                if (id == 0) continue;
                Rule r1 = new Rule();
                r1.targetType = (TargetType)int.Parse(s[2]);
                r1.attribType = (AttribType)int.Parse(s[3]);
                r1.condition = (Condition.ConditionType)int.Parse(s[4]);
                r1.value = float.Parse(s[5]);

                Rule r2 = new Rule();
                r2.targetType = (TargetType)int.Parse(s[6]);
                r2.attribType = (AttribType)int.Parse(s[7]);
                r2.condition = (Condition.ConditionType)int.Parse(s[8]);
                r2.value = float.Parse(s[9]);

                if (r1.targetType != TargetType.None)
                    rules.Add(r1);

                if (r2.targetType != TargetType.None)
                    rules.Add(r2);

                skills[id] = rules;
            }
        }


        Debug.Log("LoadFileConfig " + lines.Length);

    }

    //chi danh cho AI cua Boss
    public bool CheckUseSkill(Controller con, RoleSkill skill)
    {

        Debug.Log("con : " + con + " con.value_CoolDown_Skill_1 " + con.value_CoolDown_Skill_1);

        int skillIndex = con.role.RoleSkills.IndexOf(skill);
        float cooldown = 0;
        if (skillIndex == 1)
            cooldown = Mathf.Min(0, con.value_CoolDown_Skill_1);
        else
            cooldown = Mathf.Min(0, con.value_CoolDown_Skill_2);

        UIBattleManager.ErrorCodeSkill errorCode = con.LockedSkill(skillIndex);
        if (errorCode != UIBattleManager.ErrorCodeSkill.Success) return false;

        List<Rule> rules = skills[skill.SkillId];

        if (rules.Count > 0)
        {
            foreach (Rule rule in rules)
            {
                switch (rule.targetType)
                {                    
                    case TargetType.Self://= 1,
                        return CheckUseSkill_Self(con, skill, rule);
                    case TargetType.AllyOne://= 2,  
                        return CheckUseSkill_AllyOne(con, skill, rule);
                    case TargetType.AllyGroup://= 3,
                        return CheckUseSkill_AllyGroup(con, skill, rule);
                    case TargetType.EnemyOne://= 4,
                        return CheckUseSkill_EnemyOne(con, skill, rule);
                    case TargetType.EnemyGroup://= 5,
                        break;
                    case TargetType.AreaEffect://= 6,
                        break;
                    case TargetType.AreaAroundSelf://= 7,
                        return CheckUseSkill_AreaAroundSelf(con, skill, rule);
                }
            }
        }
        else
        {
            if (cooldown <= 0) return true;
        }

        return false;
    }

    #region private methods
    private bool CheckUseSkill_Self(Controller con, RoleSkill skill, Rule rule)
    {
        switch (rule.attribType)
        {
            case AttribType.HPPercent://6
                if (Condition.Compair((float)con.role.State.CurHP / (float)con.role.State.MaxHP, rule.condition, rule.value/100.0f)) return true;
                break;
        }
        return false;
    }
    private bool CheckUseSkill_AllyOne(Controller con, RoleSkill skill, Rule rule)
    {
       
        switch (rule.attribType)
        {
            case AttribType.HPPercent://6                
                {
                    Controller target = GetAlly(con, rule);
                    if (target != null)
                    {
                        con.target = target.gameObject;
                        return true;
                    }
                }
                break;
        }
        return false;
    }
    private bool CheckUseSkill_AllyGroup(Controller con, RoleSkill skill, Rule rule)
    {
        HeroSet allySet = GameplayManager.Instance.getAllySet(con);
        int count = 0;
        foreach (Hero hero in allySet)
        {
            if (rule.attribType == AttribType.HPPercent)//6
            {
                float hpPercent = (float)hero.controller.role.State.CurHP /  (float)hero.controller.role.State.MaxHP;
                if (Condition.Compair(hpPercent, rule.condition, rule.value / 100.0f))
                    count++;
            }
        }
        if (count > 1)
        {
            return true;
        }
        return false;
    }
    private bool CheckUseSkill_AreaAroundSelf(Controller con, RoleSkill skill, Rule rule)
    {
        HeroSet enemySet = GameplayManager.Instance.getEnemySet(con);
        int count = 0;
        foreach (Hero hero in enemySet)
        {

            float distance = Vector3.Distance(con.transform.position, hero.gameObject.transform.position);

            if (distance <= skill.GameSkill.EffectRange)
                count++;

            if (Condition.Compair(count, rule.condition, rule.value))
                return true;
        }      
        return false;
    }
    private bool CheckUseSkill_EnemyOne(Controller con, RoleSkill skill, Rule rule)
    {
         HeroSet enemySet = GameplayManager.Instance.getEnemySet(con);
         if (enemySet.Count(p => p.controller.actionStat != Controller.ActionStat.Dead) > 0)
             return true;
        return false;
    }
    private Controller GetAlly(Controller con, Rule rule)
    {
        Controller result = null;
        switch (rule.attribType)
        {
            case AttribType.HPPercent://6               
                {
                    Controller minHPController = con.stateManager.FindAllyMiniumHP();
                    if (minHPController != null)
                    {
                        float percentHP = (float)minHPController.role.State.CurHP / (float)minHPController.role.State.MaxHP;
                        if (Condition.Compair(percentHP, rule.condition, rule.value / 100.0f))
                            result = minHPController;
                    }
                }
                break;
                
        }    
        return result;

    }
    #endregion
}
