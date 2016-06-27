using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class SkillCollider  {

    public static void HandleSkillHit(GameObject colliderObject, Controller controllerAttack, int skillIndex, TargetType type)
    {

        SoundManager.Instance.PlaySkillEnd(controllerAttack.role.RoleSkills[skillIndex], controllerAttack);

        if (controllerAttack == null) return;
        string tagName = Helper.GetTagEnemy(controllerAttack);

        switch (type)
        {
            case TargetType.AllyGroup:
                break;
            case TargetType.AllyOne:               
                break;
            case TargetType.AreaEffect:
                {
                    foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(tagName))
                    {
                        if (controllerAttack.isConnectedServer())
                        {
                            if (Vector3.Distance(controllerAttack.transform.position, enemy.transform.position) > controllerAttack.curSkill.GameSkill.CastRange)
                            {
                                continue;
                            }
                        }
                        HandleSkillCollider(enemy, controllerAttack, skillIndex);
                    }
                }
                break;
            case TargetType.EnemyGroup:
                {
                    //Debug.Log("EnemyGroup");
                    foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(tagName))
                    {
                        HandleSkillCollider(enemy, controllerAttack, skillIndex);
                    }
                }
                break;
            case TargetType.EnemyOne:
                HandleSkillCollider(colliderObject, controllerAttack, skillIndex);
                break;
            case TargetType.Self:
                break;
        }
    }

    private static void HandleSkillCollider(GameObject colliderObject, Controller attackController, int skillIndex)
    {
        if (colliderObject == null || attackController == null) return;
        
        if (attackController.isConnectedServer())
        {
            GameplayManager.Instance.SendSkillHit(attackController.role.Id, attackController.index,
                colliderObject.GetComponent<Controller>(), attackController.role.RoleSkills[skillIndex]);
        }
        else
        {
            Controller targetController = colliderObject.GetComponent<Controller>();
            float damage = 0;

            if (skillIndex == 1)
                damage = attackController.skill_1_Value;
            else if(skillIndex == 2)
                damage = attackController.skill_2_Value;
            else
                damage = attackController.actionValue;


            float damageVal = damage - targetController.def;
            if (damageVal <= 0)
            {
                damageVal = 0;
            }
            targetController.TakingDamage();
            targetController.hp -= damageVal;
            targetController.damageGet = damageVal;
            targetController.InitTextDamage(Color.red);
            if ((targetController.hp - damageVal) <= 0)
            {
                targetController.hp = 0;
                targetController.actionStat = Controller.ActionStat.Dead;
            }
        }

    }

    

}
