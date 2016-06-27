using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DEngine.Common;
using DEngine.Common.GameLogic;

public class GameplayManager : MonoBehaviour
{
    public enum BattleStatus
    {
        Start,
        Begin,
        CooldownTime,
        Playing, 
        Pause,
        Win,
        Lose,
        Draw,
        End
    }

    public static List<UserRole> playerRoles = new List<UserRole>();
    public static List<UserRole> enemyRoles = new List<UserRole>();
    public static List<UserRole> allRoles = new List<UserRole>();

    public static BattleStatus battleStatus;

    public HeroSet heroPlayerSet;//list hero cua player
    public HeroSet heroEnemySet;
    public HeroSet allheroSet;
    public int totalPointUser;
    public int totalPointEnemy;

    public GameObject BattleMapRoot;
    public GameObject BattleMapDefault;
    public GameObject BattleMapPVP;

    public List<UserRole> mobsNextwave = new List<UserRole>();
    public CameraPathBezierAnimator cameraAnimEndBattle;

    private BattleContoller _battleController;
    private static GameplayManager _instance;
    public static GameplayManager Instance
    {
        get { return _instance; }
    }

    private Vector3[] _startPosPVP = {
                                         new Vector3(0,0,0),
                                         new Vector3(-1,0,0),
                                         new Vector3(1,0,0),
                                         new Vector3(0,0,1),
                                         new Vector3(0,0,-1),
                                     };

    #region Common methods
    void Awake()
    {
        GameObject map;

        //set map to load 
        if (GameManager.Status == GameStatus.PVP || GameManager.Status == GameStatus.Dungeon)
        {
            map = BattleMapPVP;

        }
        else
        {
            //full version
            //map = BattleMapDefault;

            //demo version
            map = BattleMapPVP;

        }

        //load map
        GameObject go = GameObject.Instantiate(map) as GameObject;

        go.transform.parent = BattleMapRoot.transform;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.localPosition = Vector3.zero;
    }
    void Start()
    {
        _instance = this;

        //init network
        _battleController = new BattleContoller(this);

        //heroes of player
        heroPlayerSet = new HeroSet();

        //opponents list (mobs , boss , heroes of Other)
        heroEnemySet = new HeroSet();

        //all role character (heroes of player,opponents list) in battle
        allheroSet = new HeroSet();

        //Load heroes of player , init transform
        InitHeroSet(GameplayManager.playerRoles, heroPlayerSet, GameManager.PlayerTagName);

        //Load opponent , init transform
        InitHeroSet(GameplayManager.enemyRoles, heroEnemySet, GameManager.EnemyTagName);

        foreach (Hero h in allheroSet)
        {
            if (h.controller.stateManager != null)
                h.controller.stateManager.OnInit();
        }

        totalPointUser = 0;
        totalPointEnemy = 0;

        _battleController.SendBattleReady();
    }
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            if (battleStatus == BattleStatus.Playing)
            {
                Time.timeScale = 0;
                battleStatus = BattleStatus.Pause;
            }
            else
            {
                Time.timeScale = 1;
                battleStatus = BattleStatus.Playing;
            }
        }*/


        //MyInput.CheckInput();
        //CheckBattleStatus();


        if (GameplayManager.battleStatus >= BattleStatus.Begin && GameManager.battleTime > 0)
            GameManager.battleTime -= Time.deltaTime;
    }
    void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }
    #endregion

    #region public methods

    //end battle , set idle anim for roles
    private void HandleEnd()
    {
        foreach (Hero hero in allheroSet)
        {
            if (hero != null)
                if (hero.controller != null)
                    if (hero.controller.actionStat != Controller.ActionStat.Dead)
                        hero.controller.OnIdle();
        }
    }

    //get network event 
    public IEnumerator OnLoseBattle()
    {
        HandleEnd();
        battleStatus = BattleStatus.Lose;
        yield return new WaitForSeconds(4);
        UIBattleManager.Instance.battleResult.win = false;

        //show battle result form
        UIBattleManager.Instance.battleResult.gameObject.SetActive(true);
        UIBattleManager.Instance.battleResult.ShowWindow();
    }

    //called when the camera's endbattle event go to target completed
    public IEnumerator OnWinBattle()
    {
        HandleEnd();
        battleStatus = BattleStatus.Win;
        yield return new WaitForSeconds(1);

        UIBattleManager.Instance.battleResult.win = true;
        UIBattleManager.Instance.battleResult.gameObject.SetActive(true);
        UIBattleManager.Instance.battleResult.ShowWindow();

        yield return null;
    }
    public IEnumerator OnDrawBattle()
    {
        yield return null;
    }

    // called in SubCode.Begin network event, Init all roles in battle is loaded from server
    public static void InitRoles(GameObjCollection userData)
    {
        //Debug.Log("InitRoles " + userData.Count);
        GameplayManager.allRoles.Clear();
        playerRoles.Clear();
        enemyRoles.Clear();

        playerRoles = new List<UserRole>();
        enemyRoles = new List<UserRole>();

        foreach (GameObj obj in userData)
        {
            UserRole userRole = (UserRole)obj;

            userRole.GameRole = (GameRole)GameManager.GameHeroes[userRole.Base.RoleId];

            GameplayManager.allRoles.Add(userRole);

            for (int i = 0; i < userRole.RoleSkills.Count; i++)
                userRole.RoleSkills[i].GameSkill = (GameSkill)GameManager.GameSkills[userRole.RoleSkills[i].SkillId];

            if (GameManager.GameUser.Id == userRole.Base.UserId)
            {
                playerRoles.Add(userRole);
                UserRole roleOfUser = GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == userRole.Id);
                if (roleOfUser != null)
                    roleOfUser.Base.Energy = userRole.Base.Energy;
            }
            else
            {
                enemyRoles.Add(userRole);
            }
        }
  
    }

    //remove a userrole  
    public void Remove(Controller c)
    {
        if (playerRoles.Contains(c.role))
            if(!playerRoles.Remove(c.role))
            {
                Debug.Log("!playerRoles.Remove " + c.name);
            }

        if (enemyRoles.Contains(c.role))
            if (!enemyRoles.Remove(c.role))
            {
                Debug.Log("!playerRoles.Remove " + c.name);
            }

        if (allRoles.Contains(c.role))
            if (!allRoles.Remove(c.role))
            {
                Debug.Log("!playerRoles.Remove " + c.name);
            }
        

        for (int i = 0; i < allheroSet.Count; i++)
        {
            if (allheroSet[i].controller == c)
            {
                allheroSet.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < heroPlayerSet.Count; i++)
        {
            if (heroPlayerSet[i].controller == c)
            {
                heroPlayerSet.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < heroEnemySet.Count; i++)
        {
            if (heroEnemySet[i].controller == c)
            {
                heroEnemySet.RemoveAt(i);
                i--;
            }
        }
    }


    //end game , move all heroes of player to start point
    public void MoveAllHeroToStartPos()
    {
        List<Hero> heros = heroPlayerSet.Where(p => p.controller.actionStat != Controller.ActionStat.Dead && p.controller.roleType != RoleType.Building).ToList();

        foreach (Hero hero in heros)
        {
            hero.controller.OnIdle();
            hero.controller.MoveToStartPos();
            if (battleStatus == BattleStatus.Win)
            {
                hero.controller.SetStartPos(hero.controller.posEndGame);
                hero.controller.HideHealthBar();
            }
        }
        StartCoroutine(CheckFinishedMoveToStartPos());
    }

    //when all heroes of player is completed move to start point , chase camera to heroes
    public IEnumerator CheckFinishedMoveToStartPos()
    {
        List<Hero> heros = heroPlayerSet.Where(p => p.controller.actionStat != Controller.ActionStat.Dead &&
            p.controller.role.Base.Type == DEngine.Common.GameLogic.RoleType.Hero && p.controller.roleType != RoleType.Building).ToList();

        while (true)
        {
            int count = 0;
            foreach (Hero hero in heros)
            {
                if (hero.controller.IsStandStartPos())
                {
                    count++;                  
                }
                else
                {
                    hero.controller.MoveToStartPos();
                }
            }
            if (count >= heros.Count)
            {
                if (battleStatus == BattleStatus.Win)
                {
                    StartCoroutine(PlayCameraEndBattle());
                    break;
                }
                else
                {
                    
                    yield return new WaitForSeconds(1.0f);

                    if (GameManager.Status == GameStatus.Dungeon && GameManager.battleType == BattleMode.RandomPvE)
                    {
                        for (int i = 0; i < heroPlayerSet.Count; i++)
                        {
                            if (heroPlayerSet[i] != null)
                            {
                                if (heroPlayerSet[i].controller != null)
                                {
                                    heroPlayerSet[i].controller.OnIdle();
                                    if (heroPlayerSet[i].controller.transform != null)
                                    {
                                        heroPlayerSet[i].controller.transform.LookAt(Vector3.zero);
                                    }
                                }
                            }
                        }
                    }

                    yield return new WaitForSeconds(0.5f);

                    UIWaveManager.Instance.Show();
                    break;
                }
            }
            yield return null;
        }
    }

    //get list of team for this Controller (Each role is a game object has controller script)
    public HeroSet getAllySet(Controller c)
    {
        if (c.tag == GameManager.EnemyTagName)
            return heroEnemySet;
        return heroPlayerSet;
    }

    //get list of enemy for this Controller
    public HeroSet getEnemySet(Controller c)
    {
        if (c.tag == GameManager.EnemyTagName)
            return heroPlayerSet;
        return heroEnemySet;
    }
    //reborn all heroes, play continues when lose
    public void ReviveAlHero()
    {
        Debug.Log("ReviveAlHero");

        
        heroPlayerSet.ForEach(p => p.controller.Revive());
    }
    #endregion

    #region Network Send Methods
    public void SendQuitBattle()
    {
        _battleController.SendQuit();
    }

    //index not used by old version
    public void SendMove(int roleUID, int index, Vector3 target)
    {
        float[] targetPos = new float[3] { target.x, target.y, target.z };
        _battleController.SendMove(roleUID, index, targetPos);
    }

    //send command attack ,roleUIDAttact= id of attack role ,roleIndex not used by old version
    public IEnumerator SendAction(int roleUIDAttact, int roleIndex, Controller target, float delay)
    {
        yield return new WaitForSeconds(delay);

        Hero hero = allheroSet.GetHeroWithID(roleUIDAttact);
        //Debug.Log("SendAction " + roleUIDAttact + " " + roleIndex + " " + target.gameObject.name + " ");
        if (hero != null && isMyHero(hero) && target.actionStat != Controller.ActionStat.Dead)
        {
            //Debug.Log("SendAction " + hero.gameObject.name);
            _battleController.SendAction(roleUIDAttact, roleIndex, target.role.Id);
        }
    }
    //send command attack, roleIndex not used by old version
    public void SendAction(int roleUIDAttact, int roleIndex, Controller target)
    {
        Hero hero = allheroSet.GetHeroWithID(roleUIDAttact);
        //Debug.Log("SendAction " + roleUIDAttact + " " + roleIndex + " " + target.gameObject.name + " ");
        if (hero != null && isMyHero(hero) && target.actionStat!= Controller.ActionStat.Dead)
        {
            //Debug.Log("SendAction " + hero.gameObject.name);
            _battleController.SendAction(roleUIDAttact,roleIndex, target.role.Id);
        }
    }

    //send active a skill
    public void SendSkillCast(int roleUIDAttact, int SkillID)
    {
        Hero hero = allheroSet.GetHeroWithID(roleUIDAttact);
        if (hero != null && isMyHero(hero))
        {
            _battleController.SendSkillCast(roleUIDAttact, SkillID);
        }
    }
    
    //if fall off scene ,this role will die by set hp =-1
    public void SendSkillRole(Controller target)
    {
        if (heroPlayerSet.Count > 0)
        {
            _battleController.SendSkillHit(heroPlayerSet[0].controller.role.Id, -1, target.role.Id, -1);
            Debug.Log("SendSkillRole");
        }
    }

    //send a role hit a target by ID skill, roleIndexAttact not used by old version
    public void SendSkillHit(int roleUIDAttact, int roleIndexAttact, Controller target, RoleSkill roleSkill)
    {
        if (target == null || roleSkill == null) return;

        Hero hero = allheroSet.GetHeroWithID(roleUIDAttact);

        if (hero != null)
        {
            int skillIndex = hero.controller.role.RoleSkills.IndexOf(roleSkill);
          //  if (skillIndex > 0)
          //      target.OnHit();
        }

        if (hero != null && isMyHero(hero) && target.actionStat != Controller.ActionStat.Dead)
        {
         
            if (target.tag != hero.gameObject.tag
                && target.typeCharacter != Controller.TypeChatacter.Healer
            )
            {
                if (target.activeEnemy == null)
                {
                    target.activeEnemy = hero.controller;
                }
                else 
                {
                    GameObject oldTargetToTarget = target.activeEnemy.target;

                    if (oldTargetToTarget != null && target.activeEnemy != null)
                    {
                        if (oldTargetToTarget.name != target.name || target.activeEnemy.GetComponent<Controller>().actionStat != Controller.ActionStat.Action)
                            target.activeEnemy = hero.controller;
                    }
                }

              

                if (target.actionStat == Controller.ActionStat.Idle)
                {
                    target.OnAction(hero.gameObject);
                }
            }

            _battleController.SendSkillHit(roleUIDAttact, roleIndexAttact, target.role.Id, roleSkill.SkillId);


            CheckEffectRange(hero.controller, target, roleSkill);
        }
    }

    //not call
    public IEnumerator SendSkillHitWaiting(int roleUIDAttact, int roleIndexAttact, Controller target, RoleSkill roleSkill)
    {
        yield return new WaitForSeconds(0.1f);
        SendSkillHit(roleUIDAttact, roleIndexAttact, target, roleSkill);
    }

    //send a role use a item
    public void SendUseItem(int roleUID, int itemID)
    {
        _battleController.SendUseItem(roleUID, itemID);
    }
    //stop update gameplay , set all character is idle 
    public void PauseGame()
    {
        battleStatus = GameplayManager.BattleStatus.Pause;
    }
    //continue game
    public void ResumeGame()
    {
        battleStatus = GameplayManager.BattleStatus.Playing;
    }
    #endregion

    //network event sync , update the changes were calculated for roles by server
    //damage , mp ,hp (hit , buff..) , MagicStates (additive effect impact on role)
    #region Network Recived Methods

    public void OnReciveBattleSync(UserRole.RoleState[] roleStatesSet, float time)
    {
        GameManager.battleTime = time;
      
        foreach (UserRole.RoleState roleState in roleStatesSet)
        {
            Hero hero = allheroSet.FirstOrDefault(p => p.controller.role.Id == roleState.RoleUId);
            if (hero != null)
            {
                hero.controller.OnReciveBattleSync(roleState);
                hero.damage = roleState.Damage;
            }           
        }
    }      

    //not used
    public void OnSyncTransform(int RoleUID, Vector3 transformOfRole)
    {
        Hero hero = heroEnemySet.FirstOrDefault(p => p.controller.role.Id == RoleUID);
        if (hero != null)
        {
            hero.OnSyncTransform(transformOfRole);
        }
        else
        {
            hero = heroPlayerSet.FirstOrDefault(p => p.controller.role.Id == RoleUID);
            if (hero != null)
            {
                hero.OnSyncTransform(transformOfRole);
            }
        }
    }

    //network event a role active a skill
    public void OnReciveSkillCast(int userID, int roleUId, int skillID)
    {
        //Debug.Log("OnReciveSkillCast " + userID + " " + skillID);
        Hero hero = allheroSet.GetHeroWithID(roleUId);

        if (hero != null)
            hero.controller.OnReciveSkillCast(skillID);
    }

    //network event a role move to target in SubCode.Move event
    public void OnRecivedTarget(int userID, int roleID, float[] targetPos)
    {
        allheroSet.UpdateTarget(roleID, targetPos);
    }

    //network event  a role attack a target in SubCode.Action event
    public void OnRecivedAction(int userID, int roleUID, int targetID)
    {
        Hero target = allheroSet.GetHeroWithID(targetID);
        allheroSet.GetHeroWithID(roleUID).OnReciveAction(target);    
    }

    // network event a role get critical hit ,SubCode.SkillCrit event
    public void OnReciveSkillCrit(int roleUID)
    {     
        Hero hero = allheroSet.GetHeroWithID(roleUID);

        if (hero.gameObject != null)
        {
            Color c = Color.yellow;
            if (!hero.controller.IsMyHero())
                c = Color.red;
            TextDamageController.Instance.createText(hero.gameObject.transform.position, GameManager.localization.GetText("Dialog_Crit"), c);
            if (!hero.controller.IsMyHero())
                hero.controller.OnHit();
        }

    }

    //network event a role get miss hit ,SubCode.SkillMiss event
    public void OnRecivedSkillMiss(int roleUID)
    {
        Hero hero = allheroSet.GetHeroWithID(roleUID);
        if (hero.gameObject != null)
        {
            Color c = Color.yellow;
            if (!hero.controller.IsMyHero())
                c = Color.red;
            TextDamageController.Instance.createText(hero.gameObject.transform.position, GameManager.localization.GetText("Dialog_SkillMiss"), c);
        }
    }

    //network event a role evade the attack ,SubCode.SkillEvas event
    public void OnRecivedSkillEvas(int roleUID)
    {
        Hero hero = allheroSet.GetHeroWithID(roleUID);
        if (hero.gameObject != null)
        {
            Color c = Color.yellow;
            if (!hero.controller.IsMyHero())
                c = Color.red;
            TextDamageController.Instance.createText(hero.gameObject.transform.position, GameManager.localization.GetText("Dialog_SkillEvas"), c);
        }
    }

    //network event create mob when current wave is completed ,create mob for next wave , SubCode.MobCreate
    public IEnumerator CreateMobsNextWave()
    {
        if (mobsNextwave == null) yield return null;
        
        foreach (UserRole mob in mobsNextwave)
        {
            if (mob == null) continue;
            CreateMob(mob, heroEnemySet, GameManager.EnemyTagName);
            yield return null;
        }
    }
    //create mob for turorial
    public void CreateFirstMobForTutorial()
    {
        if (mobsNextwave == null) return;

        CreateMob(mobsNextwave[0], heroEnemySet, GameManager.EnemyTagName);
        mobsNextwave.RemoveAt(0);
    }

    //camera chase to heroes
    public IEnumerator PlayCameraEndBattle()
    {
        List<Hero> heros = heroPlayerSet.Where(p => p.controller.actionStat != Controller.ActionStat.Dead &&
          p.controller.role.Base.Type == DEngine.Common.GameLogic.RoleType.Hero && p.controller.roleType != RoleType.Building).ToList();

        Vector3 lookPoint = Camera.main.transform.position;
        lookPoint.y = 0;
        foreach (Hero hero in heros)
        {
            if (hero.controller != null)
                hero.controller.OnIdle();

            if (hero.gameObject != null)
                hero.gameObject.transform.LookAt(lookPoint);
        }

        yield return new WaitForSeconds(0.5f);
        cameraAnimEndBattle.Play();
        cameraAnimEndBattle.AnimationFinished += FinishedCameraAnimation;
    }


    
    public bool isMyHero(Hero hero)
    {
        if (GameManager.battleType == BattleMode.RandomPvA || GameManager.battleType == BattleMode.RandomPvE)
            return true;

        return hero.controller.role.Base.UserId == GameManager.GameUser.Id;
    }
    #endregion

    #region private methods
    private void FinishedCameraAnimation()
    {
        StartCoroutine(OnWinBattle());
    }

    //check skill if affect in range of target, search all enemy of attacker to check in distance will affected and send skill hit
    private void CheckEffectRange(Controller attacker, Controller target, RoleSkill skill)
    {

        if (skill.GameSkill.EffectRange <= 0 || skill.GameSkill.TargetType != (int)TargetType.AreaEffect
            || skill.SkillId == 27) return;

        HeroSet enemySet = GameplayManager.Instance.getEnemySet(attacker);

        SkillManager.Instance.CreateRangeOfSkill(target.transform.position, skill.GameSkill.EffectRange);

        foreach (Hero enemy in enemySet)
        {
            if (enemy.gameObject.name != target.gameObject.name)
            {
                if (Vector3.Distance(target.transform.position, enemy.gameObject.transform.position) <= skill.GameSkill.EffectRange)
                {
                    if (attacker != null && enemy.controller != null)
                        _battleController.SendSkillHit(attacker.role.Id, 0, enemy.controller.role.Id, skill.SkillId);
                }
            }
        }

    }

    //load mob from asset 
    private void CreateMob(UserRole role, HeroSet heroset, string tag)
    {
//        Debug.Log(role.GameRole.AssetPath);
        GameObject prefab = Resources.Load("Prefabs/" + role.GameRole.AssetPath) as GameObject;
        Vector3 pos = new Vector3(role.State.TargetPos.x, role.State.TargetPos.y, role.State.TargetPos.z);
        Quaternion rotation;
        if (pos == Vector3.zero)
            rotation = Quaternion.identity;
        else
            rotation = Quaternion.LookRotation(Vector3.zero - pos);
        GameObject go = GameObject.Instantiate(prefab, pos, rotation) as GameObject;
        go.name = tag + "_" + role.Name + "_" + role.Id;
        
        Hero hero = new Hero();
        hero.gameObject = go;
        hero.controller = go.GetComponent<Controller>();
        hero.controller.index = heroset.Count + 1;    
        hero.controller.isMob = true;
        hero.controller.SetRole(role, tag);

        if (hero.controller.stateManager != null)
            hero.controller.stateManager.OnInit();

        heroset.AddHero(hero);
        allheroSet.AddHero(hero);

        foreach (Hero h in heroPlayerSet)
        {
            if (h.controller.stateManager != null)
                h.controller.stateManager.enemySet.Add(hero.controller);
        }

        foreach (Hero h in heroEnemySet)
        {
            if (h.controller.stateManager != null)
                h.controller.stateManager.allySet.Add(hero.controller);
        }

        if (hero.controller.roleType == RoleType.Building)
        {
                go.transform.rotation = prefab.transform.rotation;
        }
    }

    //Load roles from asset , init transform
    private void InitHeroSet(List<UserRole> roles, HeroSet heroset, string tag)
    {
        int maxPos = roles.Count;
        for (int i = 0; i < maxPos; i++)
        {
            GameObject prefab = Resources.Load("Prefabs/" + roles[i].GameRole.AssetPath) as GameObject;

            Vector3 pos = new Vector3(roles[i].State.TargetPos.x, roles[i].State.TargetPos.y, roles[i].State.TargetPos.z);
            Quaternion rotation ;
            if (pos == Vector3.zero)
                rotation = Quaternion.identity;
            else
                rotation = Quaternion.LookRotation(Vector3.zero - pos);
            
            GameObject go = GameObject.Instantiate(prefab, pos, rotation) as GameObject;
            go.name = tag + "_" + roles[i].Name + "_" + roles[i].Id;

            Hero hero = new Hero();
            hero.gameObject = go;
            hero.controller = go.GetComponent<Controller>();
            hero.controller.index = i;
            hero.controller.isMob = false;
            
            hero.controller.SetRole(roles[i], tag);

            if (GameManager.battleType != BattleMode.RandomPvE)
            {
                if (i < _startPosPVP.Length)
                    hero.controller.SetStartPos(_startPosPVP[i]);
            }
            hero.controller.posEndGame = _startPosPVP[i];
            if (roles[i].Base.Type == DEngine.Common.GameLogic.RoleType.Mob)
            {
                go.transform.rotation = prefab.transform.rotation;
            }            
           
            heroset.AddHero(hero);
            allheroSet.AddHero(hero);
        }
    }
    #endregion
}

public class Hero
{
    public GameObject gameObject;
    public Controller controller;
    public int damage;

    public Hero()
    {
        damage = 0;
    }       
    public void OnReciveAction(Hero targetHero)
    {
        controller.OnReciveAction(targetHero.gameObject);
    }
    public void OnSyncTransform(Vector3 pos)
    {
        controller.OnSysnPosition(pos);
    }
}

public class HeroSet : List<Hero>
{
    public void AddHero(Hero hero)
    {
        lock (this)
        {
            this.Add(hero);
        }
    }
    public void UpdateTarget(int targetID, float[] posOfTarget)
    {
        Vector3 pos = new Vector3(posOfTarget[0], posOfTarget[1], posOfTarget[2]);
        this.GetHeroWithID(targetID).controller.UpdateTransform(pos);
    }
    public void UpdateTransform(float[] transformSet)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = new Vector3(transformSet[0 + 4 * i], transformSet[1 + 4 * i], transformSet[2 + 4 * i]);
            this[i].controller.UpdateTransform(pos);
        }
    } 
    public Hero GetHeroWithID(int RoleUID)
    {
        return this.FirstOrDefault(p => p.controller.role.Id == RoleUID);
    }
    public bool isAllDead()
    {        
        foreach(Hero hero in this)
        {
            if (hero.controller != null)
            {
                if (hero.controller.hp > 0)
                {
                    return false;
                }
            }
        }
        return true;

    }
}
