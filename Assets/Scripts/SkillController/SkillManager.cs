using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System;
using System.Reflection;
using System.Linq;


//public enum SkillType
//{
//    None,
//    Active,
//    Passive,
//    Aura
//}

public class SkillManager : MonoBehaviour
{
    private static SkillManager _instance;
    public static SkillManager Instance { get { return _instance; } }

    public GameplayManager gamePlayManager;
    public GameObject rangeOfSkillPrefab;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        //Debug.Log("Clear SkillManager");
    }

    public void DoSkill(Controller controller, int skillIndex)
    {
        RoleSkill roleSkill = controller.role.RoleSkills[skillIndex];

        string methodName = string.Format("DoSkill{0:D2}", roleSkill.SkillId);
        StartCoroutine(methodName, controller);
    }

    public void DoSkill(Controller controller, RoleSkill skill)
    {
        string methodName = string.Format("DoSkill{0:D2}", skill.SkillId);
        StartCoroutine(methodName, controller);
    }

    private GameObject GetSkillPrefab(int skillID, int subID = -1)
    {
        string path = "";
        if (subID == -1)
            path = string.Format("Prefabs/Skills/Skill_{0:D3}", skillID);
        else
            path = string.Format("Prefabs/Skills/Skill_{0:D3}_{1:D2}", skillID, subID);

        return Resources.Load(path) as GameObject;
    }

    #region Active Skill
    IEnumerator DoSkill01(Controller controller)//Attack
    {
        SoundManager.Instance.PlayAction(controller);
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 01);
        if (controller.target != null)
        {
            //Instantiate(FX, controller.target.transform.position, controller.target.transform.rotation);

            gamePlayManager.SendSkillHit(controller.role.Id, controller.index,
                controller.target.GetComponent<Controller>(), skill);
        }
        yield return null;
    }
    private void Shoot(Controller controller, RoleSkill skill )
    {
        SoundManager.Instance.PlayAction(controller);
        GameObject enemy = controller.target;
        int skillIndex = controller.role.RoleSkills.IndexOf(skill);
        if (enemy != null)
        {

            GameObject spellObj = (GameObject)Instantiate(GetSkillPrefab(skill.SkillId), controller.pointSpell.position, controller.pointSpell.rotation) as GameObject;

            EffectSettings setting = spellObj.GetComponent<EffectSettings>();

            if (setting != null)
            {
                setting.Target = enemy;
                setting.gameSkill = controller.curSkill;
                setting.controller = controller;
                setting.skillIndex = skillIndex;
                setting.audioEffect = null;
            }
            else
            {
                Spell_Bullet spellBullet = spellObj.GetComponent<Spell_Bullet>();
                spellBullet.damage = 0;
                spellBullet.SkillID = 0;
                spellBullet.controllerAttack = controller;
                spellBullet.speed = 15;
                if (controller.target != null)
                {
                    spellBullet.target = enemy.transform;
                }
                else
                {
                    //Debug.Log("Destroy " + spellObj);
                    Destroy(spellObj);
                }
            }
        }
    }
    IEnumerator DoSkill02(Controller controller)//Shoot
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 2);
        Shoot(controller, skill);
        yield return null;
    }
    IEnumerator DoSkill03(Controller controller)
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 3);
        Shoot(controller, skill);
        yield return null;
    }
    IEnumerator DoSkill04(Controller controller)//Heal
    {
        SoundManager.Instance.PlayAction(controller);
        if (controller.target != null)
        {
            Controller playerTarget = controller.target.GetComponent<Controller>();

            RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 4);

            int subSkillID = 1;
            switch ((RoleType)controller.roleID)
            {
                case RoleType.Shaolin: subSkillID = 2; break;
                case RoleType.WoodElf: subSkillID = 3; break;
            }
            GameObject prefab = GetSkillPrefab(skill.SkillId, subSkillID);

            Instantiate(prefab, playerTarget.transform.position, playerTarget.transform.rotation);

            
            if (controller != null && controller.target != null)
            {
                gamePlayManager.SendSkillHit(controller.role.Id,
                    controller.index, controller.target.GetComponent<Controller>(), skill);
            }

        }
        yield return null;
    }
    IEnumerator DoSkill05(Controller controller)//Group Heal
    {

        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 5);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        int subSkillID = 1;
        switch ((RoleType)controller.roleID)
        {
            case RoleType.Hecate: subSkillID = 2; break;
        }

        GameObject prefab = GetSkillPrefab(skill.SkillId, subSkillID);

        foreach (GameObject player in GameObject.FindGameObjectsWithTag(controller.tag))
        {
            if (player == null) continue;
            Controller playerTarget = player.GetComponent<Controller>();

            if (playerTarget == null) continue;
            Instantiate(prefab, playerTarget.transform.position, playerTarget.transform.rotation);

            gamePlayManager.SendSkillHit(controller.role.Id, controller.index, playerTarget, skill);
            
            yield return null;
        }

    }
    IEnumerator DoSkill06(Controller controller)//Mega Heal
    {

        if (controller.target != null)
        {
            Controller playerTarget = controller.target.GetComponent<Controller>();

            RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 6);

            SoundManager.Instance.PlaySkillBegin(skill, controller);

            int subSkillID = 1;
            switch ((RoleType)controller.roleID)
            {
                case RoleType.WoodElf: subSkillID = 2; break;
            }
            GameObject prefab = GetSkillPrefab(skill.SkillId, subSkillID);

            Instantiate(prefab, playerTarget.transform.position, playerTarget.transform.rotation);


            if (controller != null && controller.target != null)
            {
                gamePlayManager.SendSkillHit(controller.role.Id,
                    controller.index, controller.target.GetComponent<Controller>(), skill);
            }

        }
        yield return null;
    }
    IEnumerator DoSkill07(Controller controller)//Regen
    {
        if (controller.target != null)
        {
            Controller playerTarget = controller.target.GetComponent<Controller>();

            RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 7);

            SoundManager.Instance.PlaySkillBegin(skill, controller);
       
            GameObject prefab = GetSkillPrefab(skill.SkillId);

            GameObject go = Instantiate(prefab, playerTarget.transform.position, playerTarget.transform.rotation) as GameObject;
            
            if (controller != null && controller.target != null)
            {
                gamePlayManager.SendSkillHit(controller.role.Id,
                    controller.index, controller.target.GetComponent<Controller>(), skill);
            }

            go.GetComponent<DestroyForTime>().time = skill.GameSkill.Duration;

        }
        yield return null;
    }
    IEnumerator DoSkill08(Controller controller)//Stoneskin
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 8);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject go = GameObject.Instantiate(GetSkillPrefab(skill.SkillId), controller.transform.position, controller.transform.rotation) as GameObject;
        go.transform.parent = controller.transform;

        float scale = 2.7f / controller.transform.localScale.y;

        go.transform.localScale = new Vector3(scale, scale, scale);

        gamePlayManager.SendSkillHit(controller.role.Id, controller.index, controller, skill);
        go.GetComponent<DestroyForTime>().time = skill.GameSkill.Duration;

        yield return null;
    }    
    IEnumerator DoSkill09(Controller controller)//Frenzy
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 9);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);
        GameObject go = Instantiate(prefab, controller.transform.position, controller.transform.rotation) as GameObject;
        go.transform.parent = controller.transform;

        float sacle = 1.0f / controller.gameObject.transform.localScale.y;
        go.transform.localScale = new Vector3(sacle, sacle, sacle);
        go.transform.localPosition = new Vector3(0, 2f / controller.gameObject.transform.localScale.y, 0);

        gamePlayManager.SendSkillHit(controller.role.Id, controller.index, controller, skill);

        go.GetComponent<DestroyForTime>().time = skill.GameSkill.Duration;


        yield return null;
    }
    IEnumerator DoSkill10(Controller controller)//Heavy Punch
    {
        if (controller.target != null)
        {
            if (controller.target.GetComponent<Controller>().actionStat != Controller.ActionStat.Dead)
            {
                RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 10);

                SoundManager.Instance.PlaySkillBegin(skill, controller);

                GameObject prefab = GetSkillPrefab(skill.SkillId);
                Instantiate(prefab, controller.target.transform.position, controller.target.transform.rotation);

                gamePlayManager.SendSkillHit(controller.role.Id, controller.index, controller.target.GetComponent<Controller>(), skill);
            }

        }
        yield return null;
    }
    IEnumerator DoSkill11(Controller controller)//Slam
    {
        string tagName = Helper.GetTagEnemy(controller);

        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 11);
        
        GameObject prefab = GetSkillPrefab(skill.SkillId);

        CreateRangeOfSkill(controller.transform.position, skill.GameSkill.EffectRange);

        bool isplayskill = false;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(tagName))
        {
              float distance = Vector3.Distance(enemy.transform.position, controller.transform.position);

              if (distance <= skill.GameSkill.EffectRange)
              {
                  isplayskill = true;
                  GameObject spellObj = (GameObject)Instantiate(prefab, prefab.transform.position, Quaternion.identity) as GameObject;

                  EffectSettings setting = spellObj.GetComponent<EffectSettings>();
                  int skillIndex = controller.role.RoleSkills.IndexOf(skill);
                  Debug.Log("skillIndex " + skillIndex);

                  if (setting != null)
                  {
                      setting.Target = enemy;
                      setting.gameSkill = skill;
                      setting.skillIndex = skillIndex;
                      setting.controller = controller;
                      //setting.damage = controller.skill_1_Value;
                      setting.audioEffect = null;
                  }

              }
        }

        if (isplayskill)
            SoundManager.Instance.PlaySkillBegin(skill, controller);

        yield return null;
    }
    IEnumerator DoSkill12(Controller controller)
    {
        yield return null;
    }
    IEnumerator DoSkill13(Controller controller)//Fireball
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 13);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);
        int skillIndex = controller.role.RoleSkills.IndexOf(skill);
        GameObject enemy = controller.target;

        if (enemy != null)
        {
            GameObject spellObj = (GameObject)Instantiate(prefab, controller.pointSpell.position, controller.pointSpell.rotation) as GameObject;
            EffectSettings setting = spellObj.GetComponent<EffectSettings>();
            if (setting != null)
            {
                setting.Target = enemy;
                setting.gameSkill = skill;
                setting.controller = controller;
                setting.skillIndex = skillIndex;
                setting.audioEffect = null;
            }
        }
        yield return null;
    }
    IEnumerator DoSkill14(Controller controller)
    {
        yield return null;
    }
    IEnumerator DoSkill15(Controller controller)
    {
        yield return null;
    }
    IEnumerator DoSkill16(Controller controller)
    {
        yield return null;
    }
    IEnumerator DoSkill17(Controller controller)//Frost Nova
    {

        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 17);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);
        int skillIndex = controller.role.RoleSkills.IndexOf(skill);

        string tagName = Helper.GetTagEnemy(controller);
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(tagName))
        {
            if (enemy != null)
            {
                //  float distance = Vector3.Distance(enemy.transform.position, controller.transform.position);

                // if (distance <= controller.role.RoleSkills[1].GameSkill.CastRange)
                
                GameObject spellObj = (GameObject)Instantiate(prefab, controller.pointSpell.position, controller.pointSpell.rotation) as GameObject;
                EffectSettings setting = spellObj.GetComponent<EffectSettings>();
                if (setting != null)
                {
                    setting.Target = enemy;
                    setting.gameSkill = skill;
                    setting.controller = controller;
                    setting.skillIndex = skillIndex;
                    setting.audioEffect = null;
                }               
            }
        }

        yield return null;
    }
    IEnumerator DoSkill18(Controller controller)
    {
        yield return null;
    }
    IEnumerator DoSkill19(Controller controller)
    {
        yield return null;
    }
    IEnumerator DoSkill20(Controller controller)
    {
        yield return null;
    }
    IEnumerator DoSkill21(Controller controller)//Stun shot
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 21);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);
        int skillIndex = controller.role.RoleSkills.IndexOf(skill);
        GameObject enemy = controller.target;

        if (enemy != null)
        {
            GameObject spellObj = (GameObject)Instantiate(prefab, controller.pointSpell.position, controller.pointSpell.rotation) as GameObject;
            EffectSettings setting = spellObj.GetComponent<EffectSettings>();
            if (setting != null)
            {
                setting.Target = enemy;
                setting.gameSkill = skill;
                setting.controller = controller;
                setting.skillIndex = skillIndex;
                setting.audioEffect = null;
            }

        }

        yield return null;
    }
    IEnumerator DoSkill22(Controller controller)//Calm shot
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 22);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);
        int skillIndex = controller.role.RoleSkills.IndexOf(skill);
        GameObject enemy = controller.target;

        if (enemy != null)
        {
            GameObject spellObj = (GameObject)Instantiate(prefab, controller.pointSpell.position, controller.pointSpell.rotation) as GameObject;
            EffectSettings setting = spellObj.GetComponent<EffectSettings>();
            if (setting != null)
            {
                setting.Target = enemy;
                setting.gameSkill = skill;
                setting.controller = controller;
                setting.skillIndex = skillIndex;
                setting.audioEffect = null;
            }

        }
        yield return null;
    }
    IEnumerator DoSkill23(Controller controller)//God Power
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 23);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);
        int skillIndex = controller.role.RoleSkills.IndexOf(skill);
        GameObject enemy = controller.target;

        if (enemy != null)
        {
            GameObject spellObj = (GameObject)Instantiate(prefab, enemy.transform.position, Quaternion.identity) as GameObject;

            yield return new WaitForSeconds(0.1f);
            gamePlayManager.SendSkillHit(controller.role.Id,
                   controller.index, enemy.GetComponent<Controller>(), skill);
        }
        yield return null;
    }
    IEnumerator DoSkill24(Controller controller)//Lightning Call
    {
        GameObject target = controller.target;
        if (target == null)
        {
            Controller con = controller.stateManager.FindEnemyNearest(true);
            if (con != null)
            {
                target = con.gameObject;
            }
        }

        if (target != null)
            controller.transform.LookAt(target.transform);

        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 24);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);
        int skillIndex = controller.role.RoleSkills.IndexOf(skill);
        GameObject go = Instantiate(prefab, controller.transform.position, controller.transform.rotation) as GameObject;

        SkillRootManager skillRootManager = go.GetComponent<SkillRootManager>();

        skillRootManager.gameSkill = skill;
        skillRootManager.controller = controller;
        skillRootManager.skillIndex = skillIndex;
        skillRootManager.audioEffect = null;

        yield return null;
    }
    IEnumerator DoSkill25(Controller controller)//Dew of Nature
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 25);

        SoundManager.Instance.PlaySkillBegin(skill, controller);       

        GameObject prefab = GetSkillPrefab(skill.SkillId);

        foreach (GameObject player in GameObject.FindGameObjectsWithTag(controller.tag))
        {
            if (player == null) continue;
            Controller playerTarget = player.GetComponent<Controller>();

            if (playerTarget == null) continue;
            Instantiate(prefab, playerTarget.transform.position, playerTarget.transform.rotation);
            
            gamePlayManager.SendSkillHit(controller.role.Id, controller.index, playerTarget, skill);

            yield return null;
        }
    }
    IEnumerator DoSkill26(Controller controller)//Kamekameha
    {
        if (controller.target != null)
        {
            if (controller.target.GetComponent<Controller>().actionStat != Controller.ActionStat.Dead)
            {

                RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 26);

                SoundManager.Instance.PlaySkillBegin(skill, controller);

                GameObject prefab = GetSkillPrefab(skill.SkillId);
                int skillIndex = controller.role.RoleSkills.IndexOf(skill);

                GameObject go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                Songoku_SkillKamejoko kamejoko = go.GetComponent<Songoku_SkillKamejoko>();
                kamejoko.startTarget = controller.transform;
                kamejoko.endTarget = controller.target.transform;
                kamejoko.skillIndex = skillIndex;
                kamejoko.StartSkill();
            }
        }
        yield return null;
    }
    IEnumerator DoSkill27(Controller controller)//Gravily Ball
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 27);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);
        int skillIndex = controller.role.RoleSkills.IndexOf(skill);

        GameObject go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        GravilyBallController gb = go.GetComponent<GravilyBallController>();
        gb.Player = controller.transform;
        gb.skillIndex = skillIndex;
        gb.controllerAttack = controller;
        gb.roleSkill = skill;
        yield return null;
    }
    IEnumerator DoSkill28(Controller controller)//Thorn Shield
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 28);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);

        GameObject go = GameObject.Instantiate(prefab, controller.transform.position + Vector3.up, Quaternion.identity) as GameObject;

        go.transform.parent = controller.transform;
        go.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        
        gamePlayManager.SendSkillHit(controller.role.Id, controller.index, controller, skill);

        go.GetComponent<DestroyForTime>().time = skill.GameSkill.Duration;

        yield return null;
    }
    IEnumerator DoSkill29(Controller controller)//Whirlwind
    {
        if (controller.target != null)
        {
            RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 29);

            SoundManager.Instance.PlaySkillBegin(skill, controller);

            GameObject prefab = GetSkillPrefab(skill.SkillId);
            int skillIndex = controller.role.RoleSkills.IndexOf(skill);

            Transform wind = controller.transform.FindChild("Garuda_Wind");
            if (wind != null)
                wind.gameObject.SetActive(true);

            controller.animation.CrossFade("BeginFly");
            yield return new WaitForSeconds(controller.animation["BeginFly"].length);

            Vector3 startPos = controller.transform.position;
            float time = Time.time;

            if (controller == null || controller.target == null)
            {
                if (controller != null)
                    controller.animation.CrossFade("EndFlying");

                yield return null;
            }

            Vector3 dir = controller.target.transform.position - controller.transform.position;
            do
            {
                if (controller == null || controller.target == null) break;

                Vector3 endPos = controller.target.transform.position;
                float journeyLength = Vector3.Distance(startPos, endPos);
                dir = controller.target.transform.position - controller.transform.position;

                if (dir.magnitude > 6)
                {
                    controller.animation.CrossFade("Flying");
                }
                else
                {
                    controller.animation.CrossFade("EndFlying");
                }
                float distCovered = (Time.time - time) * controller.speedMove * 4;
                float fracJourney = distCovered / journeyLength;

                controller.transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
                dir = controller.target.transform.position - controller.transform.position;
                yield return null;
            }
            while (dir.magnitude > 1.5f);

            if (wind != null)
                if (wind.gameObject.activeInHierarchy)
                    wind.gameObject.SetActive(false);

            GameObject cylone = GameObject.Instantiate(prefab, controller.transform.position, Quaternion.identity) as GameObject;
           // SoundManager.Instance.PlaySkillEnd(skill, controller);
            
            Destroy(cylone, 1.5f);

            yield return new WaitForSeconds(0.7f);
            SkillCollider.HandleSkillHit(null, controller, skillIndex, DEngine.Common.GameLogic.TargetType.EnemyGroup);
        }

        yield return null;
    }
    IEnumerator DoSkill55(Controller controller)//Suicide Bomb
    {
        if (controller.target != null)
        {

            string tagName = Helper.GetTagEnemy(controller);

            RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 55);
            int skillIndex = controller.role.RoleSkills.IndexOf(skill);

            CreateRangeOfSkill(controller.transform.position, skill.GameSkill.EffectRange);

            SoundManager.Instance.PlaySkillBegin(skill, controller);

            GameObject prefab = GetSkillPrefab(skill.SkillId);
            GameObject go = Instantiate(prefab, controller.transform.position, Quaternion.identity) as GameObject;
           

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(tagName))
            {
                float distance = Vector3.Distance(enemy.transform.position, controller.transform.position);

                if (distance <= skill.GameSkill.EffectRange)
                {
                    if (enemy != null)
                    {
                        gamePlayManager.SendSkillHit(controller.role.Id,
                            controller.index, enemy.GetComponent<Controller>(), skill);
                    }
                }
            }

            gamePlayManager.SendSkillHit(controller.role.Id,
                           controller.index, controller, skill);

        }
        yield return null;
    }
    IEnumerator DoSkill56(Controller controller)//Detonate!
    {

        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 56);

        int skillIndex = controller.role.RoleSkills.IndexOf(skill);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        GameObject prefab = GetSkillPrefab(skill.SkillId);

        GameObject go = Instantiate(prefab, controller.transform.position, Quaternion.identity) as GameObject;

        SkillCollider.HandleSkillHit(null, controller, skillIndex, DEngine.Common.GameLogic.TargetType.EnemyGroup);

        gamePlayManager.SendSkillHit(controller.role.Id,
                        controller.index, controller, skill);

        yield return null;
    }
    #endregion

    #region Buff
    IEnumerator DoSkill30(Controller controller)
    {
        DoBuff(controller, 30);
        yield return null;
    }
    IEnumerator DoSkill31(Controller controller)
    {
        DoBuff(controller, 31);
        yield return null;
    }
    IEnumerator DoSkill32(Controller controller)
    {
        DoBuffAllAlly(controller, 32);
        yield return null;
    }
    IEnumerator DoSkill33(Controller controller)
    {
        DoBuffAllAlly(controller, 33);
        yield return null;
    }
    IEnumerator DoSkill34(Controller controller)
    {
        DoBuffAllAlly(controller, 34);
        yield return null;
    }
    IEnumerator DoSkill35(Controller controller)
    {
        DoBuffAllAlly(controller, 35);
        yield return null;
    }
    IEnumerator DoSkill36(Controller controller)
    {
        DoBuff(controller, 36);
        yield return null;
    }

    IEnumerator DoSkill40(Controller controller)
    {
        DoBuffAllAlly(controller, 40);
        yield return null;
    }
    IEnumerator DoSkill45(Controller controller)
    {
        DoBuffAllAlly(controller, 45);
        yield return null;
    }
    IEnumerator DoSkill46(Controller controller)
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == 46);

        string tagEnemy = Helper.GetTagEnemy(controller);

        CreateRangeOfSkill(controller.transform.position, skill.GameSkill.EffectRange);

        bool isplaySkill = false;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag(tagEnemy))
        {          
            if (player == null) continue;

            float distance = Vector3.Distance(player.transform.position, controller.transform.position);

             if (distance <= skill.GameSkill.EffectRange)
             {
                 isplaySkill = true;

                 Controller playerTarget = player.GetComponent<Controller>();

                 if (playerTarget == null) continue;

                 gamePlayManager.SendSkillHit(controller.role.Id, controller.index, playerTarget, skill);
             }
        }

        if (isplaySkill)
            SoundManager.Instance.PlaySkillBegin(skill, controller);


        yield return null;
    }
    IEnumerator DoSkill47(Controller controller)
    {
        DoBuff(controller, 47);
        yield return null;
    }
    IEnumerator DoSkill48(Controller controller)
    {
        DoBuff(controller, 48);
        yield return null;
    }
    IEnumerator DoSkill49(Controller controller)
    {
        DoBuff(controller, 49);
        yield return null;
    }
    IEnumerator DoSkill50(Controller controller)
    {
        DoBuffAllEnemy(controller, 50);
        yield return null;
    }
    IEnumerator DoSkill51(Controller controller)
    {
        DoBuff(controller, 51);
        yield return null;
    }
    IEnumerator DoSkill52(Controller controller)
    {
        DoBuffAllAlly(controller, 52);
        yield return null;
    }
    IEnumerator DoSkill53(Controller controller)
    {
        DoBuff(controller, 53);
        yield return null;
    }
    IEnumerator DoSkill54(Controller controller)
    {
        DoBuff(controller, 54);
        yield return null;
    }

    private void DoBuff(Controller controller, int skillID)
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == skillID);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        gamePlayManager.SendSkillHit(controller.role.Id, controller.index, controller, skill);
    }
    private void DoBuffAllAlly(Controller controller, int skillID)
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == skillID);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        foreach (GameObject player in GameObject.FindGameObjectsWithTag(controller.tag))
        {
            if (player == null) continue;
            Controller playerTarget = player.GetComponent<Controller>();

            if (playerTarget == null) continue;            

            gamePlayManager.SendSkillHit(controller.role.Id, controller.index, playerTarget, skill);
        }
    }
    private void DoBuffAllEnemy(Controller controller, int skillID)
    {
        RoleSkill skill = controller.role.RoleSkills.FirstOrDefault(p => p.SkillId == skillID);

        SoundManager.Instance.PlaySkillBegin(skill, controller);

        string tagEnemy = Helper.GetTagEnemy(controller);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(tagEnemy))
        {
            if (player == null) continue;
            Controller playerTarget = player.GetComponent<Controller>();

            if (playerTarget == null) continue;

            gamePlayManager.SendSkillHit(controller.role.Id, controller.index, playerTarget, skill);
        }
    }
    #endregion

    #region Aura
    IEnumerator DoSkill37(Controller controller)
    {
        CreateAura(controller, 37);
        yield return null;
    }
    IEnumerator DoSkill38(Controller controller)
    {
        CreateAura(controller, 38);
        yield return null;
    }
    IEnumerator DoSkill39(Controller controller)
    {
        CreateAura(controller, 39);
        yield return null;
    }
    IEnumerator DoSkill41(Controller controller)
    {
        CreateAura(controller, 41);
        yield return null;
    }
    IEnumerator DoSkill42(Controller controller)
    {
        CreateAura(controller, 42);
        yield return null;
    }
    IEnumerator DoSkill43(Controller controller)
    {
        CreateAura(controller, 43);
        yield return null;
    }
    IEnumerator DoSkill44(Controller controller)
    {
        CreateAura(controller, 44);
        yield return null;
    }
    #endregion


    public void CreateRangeOfSkill(Vector3 pos, float range)
    {
        GameObject rangeOfSkill = Instantiate(rangeOfSkillPrefab) as GameObject;
        RangeOfSkillController rangeOfSkillController = rangeOfSkill.GetComponent<RangeOfSkillController>();
        rangeOfSkillController.setPosition(pos);
        rangeOfSkillController.setRange(range);
    }

    public GameObject CreateCastRange(Transform _transform, float range)
    {
        GameObject rangeOfSkill = Instantiate(rangeOfSkillPrefab) as GameObject;
        rangeOfSkill.transform.parent = _transform;
        RangeOfSkillController rangeOfSkillController = rangeOfSkill.GetComponent<RangeOfSkillController>();
        rangeOfSkillController.setLocalPos(Vector3.zero);
        rangeOfSkillController.setRange(range / _transform.localScale.y);
        rangeOfSkill.GetComponent<DestroyForTime>().time = 1;
        return rangeOfSkill;
    }

    private void CreateAura(Controller controller, int SkillID)
    {
        GameObject prefab = GetSkillPrefab(SkillID);
        if (prefab == null)
        {
            Debug.Log("No Prefab " + SkillID);
            return;
        }
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.transform.parent = controller.transform;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.rotation = Quaternion.identity;

        if (controller.roleType == RoleType.Building)
        {
            go.transform.localRotation = Quaternion.Euler(Vector3.right * 90);
        }


        go.transform.localPosition = new Vector3(0, 0.05f, 0);

        // gamePlayManager.SendSkillHit(controller.role.Id, controller.index, controller, skill);
        // go.GetComponent<DestroyForTime>().time = skill.GameSkill.Duration;

    }

}
