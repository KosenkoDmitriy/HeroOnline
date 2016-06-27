/// <summary>
/// This script use for control state of character.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;
using DEngine.Common.Config;

public class Controller : MonoBehaviour
{
    public enum TypeChatacter
    { //Type of character
        Melee, Range, Healer
    }

    public enum ActionStat
    { //State of character
        Idle, Move, Action, Skill, Dead, hit
    }

    #region variable
    public TypeChatacter typeCharacter;
    public AnimationClip idle, walk, action, action1, action2, cast_1, cast_2, skill_1, skill_2, dead, hit; //animation character
    public GameObject[] objMeshAndSkinMesh; //Gameobject Mesh use for change color material when taking attack
    public Texture2D icon_Skill_1, icon_Skill_2; //Icon skill
    public Texture2D textureHpFull, textureHpEmpty; //hp bar
    public Color colorTakeDamage; //color when character take damage

    //Status Character
    public float maxhp; //Max hp
    public float def; //Defend
    public float speedMove; //Speed movement
    public float actionValue; //If character is Melee,Range it mean Attack , If character is Healer it mean Healing power
    public float skill_1_Value; //Skill 1 Damage
    public float skill_2_Value; //Skill 2 Damage
    public float actionSpeed; //If character is Melee,Range it mean AttackSpeed , If character is Healer it mean Healing speed
    public float distanceAction; //Distance attack , Distance heal
    public float deley_Cast_Skill_1; //Cast skill timer
    public float deley_Cast_Skill_2; //Cast skill timer
    public float coolDown_Skill_1; //Cooldown to use skill 1 again
    public float coolDown_Skill_2; //Cooldown to use skill 2 again
    public int strength;
    public int agility;
    public int intelligent;


    //Delegate update function
    public delegate void FunctionHandle();
    public FunctionHandle ActionHandle;
    public FunctionHandle SkillHandle;
    public bool autoSkill;

    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public float damageGet;
    [HideInInspector]
    public Vector3 positionWay;
    [HideInInspector]
    public ActionStat actionStat;
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public float value_CoolDown_Skill_1, value_CoolDown_Skill_2;
    [HideInInspector]
    public float hp { get { return _hp; } set { _hp = value; if (_healthBar != null) _healthBar.SetHP(_hp, maxhp); } }
    [HideInInspector]
    public Transform pointSpell;
    [HideInInspector]
    public int index = 0;
    public UserRole role { get; set; }
    public string roleName;
    public int roleID;
    public int level;
    public bool isMob { get; set; }
    [HideInInspector]
    public int oldSkillIndexSend = -1;
    public int curSkillIndexSelect { get; set; }
    public RoleSkill curSkill { get; set; }
    public Controller activeEnemy { get; set; }
    public bool isAuto { get { return stateManager.enabled; } }
    [HideInInspector]
    public RoleAttribute attribute;
    [HideInInspector]
    public UIHeroSlotController uiSlot;
    public Vector3 offsetEffect;
    public RoleType roleType;
    public Vector2 healthbarSize = new Vector2(1, 1);
    public StateManager stateManager;
    public Texture2D itemIconEquiped;
    public UserItem consumeEquiped;
    public float itemCooldown;
    public bool sendingSkill;
    public Vector3 posEndGame;
    public AudioSource audio;
    public bool isLockAuto;

    public bool isMobTutorial = false;

    //Variable private field 
    private float countCastSkill_1;
    private float countCastSkill_2;
    private float countAction;
    private float checkDistance;
    private Vector3 pointHp;
    private Rect rectHp;

    private float _hp;
    private Transform _myTransform;
    private UIHUDHealthBarController _healthBar;
    private Dictionary<EffectType, UserRole.MagicState> _effect;
    private EffectManager _effectManager;
    private float _timerAnim;
    private float _oldMoveSpeed;
    private float _oldActionSpeed;
    private ActionStat _oldActionStat;
    private float _actionAnimSpeed;
    private float _skill1AnimSpeed;
    private float _skill2AnimSpeed;
    private float _moveAnimSpeed;
    private Vector3 _startPos;
    private GameObject _castRange;
    #endregion 

    #region Common Methods
    void Awake()
    {
        _myTransform = transform;
        _effect = new Dictionary<EffectType, UserRole.MagicState>();
        _effectManager = new EffectManager(this);
        isLockAuto = false; 
        stateManager = GetComponent<StateManager>();
    }
    void Start()
    {
        if (tag == GameManager.PlayerTagName)
            autoSkill = false;
        else
            autoSkill = true;

        _timerAnim = 0;
        curSkillIndexSelect = -1;
        ActionHandle = Action_2;
        sendingSkill = false;   
        InitHealthBar();
        hp = maxhp;        
        ConfigAnimation();

        if (isConnectedServer())
        {
            if (role.RoleSkills.Count > 0)
            {
                //set current skill = deafult skill
                GameSkill curSkill = role.RoleSkills[0].GameSkill;

                distanceAction = curSkill.CastRange;
            }
        }

        audio = gameObject.AddComponent<AudioSource>();

    }       
    void Update()
    {


        //TestEffect();
        if (GameplayManager.battleStatus == GameplayManager.BattleStatus.End)
        {         
            return;
        }

        if (GameplayManager.battleStatus == GameplayManager.BattleStatus.Pause && !isMobTutorial)
        {
            if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed && UIBattleManager.Instance.tutStep < UIBattleManager.TutorialStepForBattle.AutoSkill)
            {
                if (UIBattleManager.Instance.heroFirst == null || this != UIBattleManager.Instance.heroFirst.controller)
                {
                    animation.CrossFade(idle.name);
                    return;
                }
            }
            else
            {
                if (actionStat != ActionStat.Move)
                {
                    if (idle != null)
                        animation.CrossFade(idle.name);
                    return;
                }
            }
        }

        //affected by stun , pause role
        if (_effect.ContainsKey(EffectType.Stun) && actionStat != ActionStat.Dead) return;

        if (roleType == RoleType.Building) return;

        if (actionStat != ActionStat.Skill)
            countAction += Time.smoothDeltaTime;

        //if fall off scene , role will die by set hp =-1
        if (_myTransform.position.y <= -100 && hp >= 0)
        {
            hp = -1;
            GameplayManager.Instance.SendSkillRole(this);
        }

        //if (sendingSkill) return;

        //update animation , move
        UpdateActionStat();

        if (itemCooldown > 0)
        {
            itemCooldown -= Time.deltaTime;
            if (itemCooldown < 0) itemCooldown = 0;
        }
    }
    void TestEffect()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            UserRole.RoleState roleState = new UserRole.RoleState()
            {
                RoleUId = 1,
            };

            roleState.MagicStates = new UserRole.MagicState[1];

            UserRole.MagicState magicState = new UserRole.MagicState()
            {
                RemainTime = 5,
                AttribType = AttribType.SkillDisable,
                EffectType = EffectType.Slow,
                AffectValue = 1
            };
            roleState.MagicStates[0] = magicState;

            CheckEffect(roleState);
        }
    }
    void OnDestroy()
    {
        StopCoroutine("Send2Server");
    }
    #endregion
    
    
    #region Action Kit Methods

    //update animation , move
    private void UpdateActionStat()
    {        
        //Hero State animation
        switch (actionStat)
        {
            case ActionStat.Idle:
                {
                    /*if (tag == "Player" && !isAuto && typeCharacter != TypeChatacter.Healer)
                    {
                        Controller enemy;
                        float maxRange = distanceAction;

                        maxRange = Mathf.Max(maxRange, 3);

                        enemy = stateManager.FindEnemyNearest(true, maxRange);

                        if (enemy != null && actionStat == ActionStat.Idle)
                        {
                            HandleAction(enemy.gameObject);
                        }

                    }*/

                    oldSkillIndexSend = -1;
                    curSkillIndexSelect = -1;
                    animation.CrossFade(idle.name);

                }
                break;

             //this role move to point
            case ActionStat.Move://receiver from server 
                {

                    oldSkillIndexSend = -1;
                    curSkillIndexSelect = -1;
                    countAction = float.MaxValue;
                    checkDistance = Vector3.Distance(transform.position, LookAtTo(positionWay));
                    //Debug.Log(name + " move");
                    if (checkDistance >= 0.5f)
                    {
                        animation.CrossFade(walk.name);
                        transform.Translate(Vector3.forward * speedMove * Time.deltaTime);
                    }
                    else
                    {
                        //positionWay = Vector3.zero;
                        OnIdle();

                        //completed how to move tutorial 
                        UIBattleManager.Instance.OnHeroFinishedMove(this);

                        //hanlde =default attack 1 (only mobs)
                        ActionHandle = Action_1;
                    }

                }
                break;

            //this role move to target , and attack
            case ActionStat.Action://receiver from server 
                {
                  
                    if (target != null)
                    {

                        checkDistance = (transform.position - LookAtTo(target.transform.position)).magnitude;// *0.8f;

                        if (checkDistance >= distanceAction)
                        {
                            animation.CrossFade(walk.name);
                            transform.Translate(Vector3.forward * speedMove * Time.deltaTime);

                            //hanlde save =default Action_1 , prepare for this action
                            ActionHandle = Action_1;
                        }
                        else
                        {
                            if (isConnectedServer() && oldSkillIndexSend != 0)
                            {
                                //in range attack of default skill, send active skill
                                if (target.GetComponent<Controller>().actionStat != ActionStat.Dead)
                                    GameplayManager.Instance.SendSkillCast(role.Id, role.RoleSkills[0].SkillId);

                                oldSkillIndexSend = 0;

                                
                            }
                            else
                            {
                               //call function of hanlde
                                ActionHandle();
                            }


                            if (target.GetComponent<Controller>() != null && target.GetComponent<Controller>().hp <= 0)
                            {
                                target = null;
                            }

                        }
                    }
                    else//target not exist
                    {
                        OnIdle();
                    }
                }
                break;

            case ActionStat.Skill://receiver from server 
                {
                    if (SkillHandle != null)
                    {
                        if (isConnectedServer())
                        {
                            GameSkill curSkill = role.RoleSkills[curSkillIndexSelect].GameSkill;
                            if (isSkillNeedTarget(curSkill))
                            {
                                if (target == null)
                                {
                                    OnIdle();
                                    return;
                                }


                                checkDistance = (transform.position - LookAtTo(target.transform.position)).magnitude;// *0.8f;

                                if (checkDistance >= curSkill.CastRange)
                                {
                                    //Debug.Log("Ok CastRange " + curSkill.CastRange);
                                    if (animation[action.name].normalizedTime <= 0)
                                    {
                                        animation.CrossFade(walk.name);
                                        transform.Translate(Vector3.forward * speedMove * Time.deltaTime);
                                        break;
                                    }
                                }
                                else
                                {
                                    //Debug.Log("CastRange < " + curSkill.CastRange);
                                }
                            }
                        }
                        SkillHandle();
                    }
                    else
                    {
                        actionStat = ActionStat.Idle;
                    }
                }
                break;

            case ActionStat.Dead:
                {
                    if (animation.IsPlaying(dead.name)) return;
                    if (!animation.enabled)
                        animation.enabled = true;
                    animation.CrossFade(dead.name);
                    this.enabled = false;                
                }
                break;
            case ActionStat.hit:
                if (hit != null)
                    animation.CrossFade(hit.name);
                break;
        }
    }

     //check time for next attack
    private void Action_1()
    {
        //when  this time, Wait for next attack
        animation.CrossFade(idle.name);

        //countAction (time start idle to now) , when time of the next attack came 
        if (countAction >= actionSpeed - CalculatorTimeAnim(action))
        {
            if (action2 != null)//is mobs
            {
                //set clip of default skill to play is random clip (default 1  or default 2)
                if (Random.Range(0, 2) == 1)
                {
                    action = action2;
                }
                else
                {
                    action = action1;
                }
            }

            countAction = 0;
            ActionHandle = Action_2;//attack by default skill

        }
    }
    private void Action_2()
    {
        //play animation default skill
        animation.CrossFade(action.name);

        _timerAnim += Time.deltaTime;
        //attack is completed 
        if (_timerAnim >= CalculatorTimeAnim(action) * 0.9f)// if (animation[action.name].normalizedTime > 0.9f)
        {
            //reset
            _timerAnim = 0;

            //back to Action_1, handle to action_1 (check time for next attack)
            ActionHandle = Action_1;
            countAction = 0;

            oldSkillIndexSend = -1;
            curSkillIndexSelect = -1;
            

            //send message to server , this role wants to impact target
           // OnAction(target);
        }
    }
    public void Skill_1_Cast()
    {
        //Cast Skill 1
        //animation.CrossFade(cast_1.name);
        // countCastSkill_1 += 1 * Time.deltaTime;// Time.smoothDeltaTime;
        //// if (countCastSkill_1 >= deley_Cast_Skill_1)
        // {
        SoundManager.Instance.PlaySkillCast(this, 1);
        SkillHandle = Skill_1_Action;
        countCastSkill_1 = 0;
        StartCoroutine("CalCoolDownSkill_1");
        // }
    }
    public void Skill_1_Action()
    {
        
        //Skill 1
        animation.CrossFade(skill_1.name);
        _timerAnim += Time.deltaTime;

        //attack is completed 
        if (_timerAnim >= CalculatorTimeAnim(skill_1)*0.9f)// if (animation[skill_1.name].normalizedTime > 0.9f)
        {
            _timerAnim = 0;
            countAction = 0;
            oldSkillIndexSend = -1;
            curSkillIndexSelect = -1;
            
            SkillHandle = null;

            ActionHandle = Action_1;//skills completed, wait for default skill

            if (target != null)
            {
               // OnAction(target);
            }
            else
            {
                OnIdle();
            }
        }
    }
    public void Skill_2_Cast()
    {
        //Cast Skill 2
        //animation.CrossFade(cast_2.name);
        // countCastSkill_2 += 1 * Time.deltaTime;//Time.smoothDeltaTime;
        // if (countCastSkill_2 >= deley_Cast_Skill_2)
        // {
        SoundManager.Instance.PlaySkillCast(this, 2);
        SkillHandle = Skill_2_Action;
        countCastSkill_2 = 0;
        StartCoroutine("CalCoolDownSkill_2");
        // }
    }
    public void Skill_2_Action()
    {
        
        //Skill 2
        animation.CrossFade(skill_2.name);
        _timerAnim += Time.deltaTime;
        if (_timerAnim >= CalculatorTimeAnim(skill_2) * 0.9f)// if (animation[skill_2.name].normalizedTime > 0.9f)
        {
            _timerAnim = 0;
            countAction = 0;
            oldSkillIndexSend = -1;
            curSkillIndexSelect = -1;

            SkillHandle = null;
            ActionHandle = Action_1;

            if (target != null)
            {
                OnAction(target);
            }
            else
            {
                OnIdle();
            }
        }
    }

    //active text damage
    public void TakingDamage()
    {
        //if take damage material monster will change to white color
        int index = 0;
        while (index < objMeshAndSkinMesh.Length)
        {
            if (objMeshAndSkinMesh[index] != null)
            {
                foreach (Material mat in objMeshAndSkinMesh[index].renderer.materials)
                    mat.color = Color.white;
            }
            index++;
        }

        if (gameObject != null && gameObject.activeInHierarchy)
            StartCoroutine(TakeDamage(0.1f));
    }
    public void InitTextDamage(Color colorText, bool isUp = false)
    {
        if (damageGet == 0) return;
        TextDamageController.Instance.create(_myTransform.position, damageGet, colorText, isUp);
    }
    public void AddBuff(float buffValue, float time)
    {
        // Buff value
        actionValue += buffValue;
        skill_1_Value += buffValue;
        skill_2_Value += buffValue;
        StartCoroutine(BuffCount(buffValue, time));
    }
    private IEnumerator BuffCount(float buffValue, float time)
    {
        // Buff duration
        yield return new WaitForSeconds(time);
        actionValue -= buffValue;
        skill_1_Value -= buffValue;
        skill_2_Value -= buffValue;
    }
    private IEnumerator TakeDamage(float time)
    {
        //if take damage material monster will change to setting color
        int index = 0;
        Color[] colorDef = new Color[objMeshAndSkinMesh.Length];
        while (index < objMeshAndSkinMesh.Length)
        {
            if (objMeshAndSkinMesh[index] != null)
            {
                foreach (Material mat in objMeshAndSkinMesh[index].renderer.materials)
                {
                    colorDef[index] = mat.color;
                    mat.color = colorTakeDamage;
                }
            }
            index++;
        }
        yield return new WaitForSeconds(time);
        index = 0;
        while (index < objMeshAndSkinMesh.Length)
        {
            foreach (Material mat in objMeshAndSkinMesh[index].renderer.materials)
            {
                mat.color = colorDef[index];
            }
            index++;
        }
        yield return 0;
        StopCoroutine("TakeDamage");
    }

    //update cooldown skill 1
    private IEnumerator CalCoolDownSkill_1()
    {
        //Cooldown skill 1
        float countCoolDown = coolDown_Skill_1;
        Debug.Log("coolDown_Skill_1 : " + coolDown_Skill_1+" time "+Time.deltaTime);
        while (countCoolDown > 0)
        {
            countCoolDown -= 1 * Time.deltaTime;//Time.smoothDeltaTime;
            value_CoolDown_Skill_1 = countCoolDown / coolDown_Skill_1;
            yield return 0;
        }
        yield return 0;
    }
    private IEnumerator CalCoolDownSkill_2()
    {
        //Cooldown skill 2
        float countCoolDown = coolDown_Skill_2;
        while (countCoolDown > 0)
        {
            countCoolDown -= 1 * Time.deltaTime;//Time.smoothDeltaTime;
            value_CoolDown_Skill_2 = countCoolDown / coolDown_Skill_2;
            yield return 0;
        }
        yield return 0;
    }
    private Vector3 LookAtTo(Vector3 pos)
    {     

        //Lookat Monster
        Vector3 look = Vector3.zero;
        look.x = pos.x;
        look.y = transform.position.y;
        look.z = pos.z;
        try
        {
            this.transform.LookAt(look);
        }
        catch
        {
            Debug.Log("None Look");
        }
        return look;
    }
    #endregion
    

    #region Network Methods
    //not use
    private IEnumerator Send2Server()
    {
        while (true)
        {
            switch (actionStat)
            {
                case ActionStat.Move:
                    {
                        GameplayManager.Instance.SendMove(role.Id, index, positionWay);
                    }
                    break;
                case ActionStat.Action:
                    {
                        GameplayManager.Instance.SendAction(role.Id, index, target.GetComponent<Controller>());
                    }
                    break;
            }
            yield return new WaitForSeconds(1);
        }
    }
    public bool isConnectedServer()
    {
        if (role != null)
            return true;
        return false;
    }
    public void OnReciveBattleSync(UserRole.RoleState roleState)
    {
        if (this == null) return;
        
        role.State = roleState;

        float damage = hp - roleState.CurHP;

        this.hp -= damage;
        
        //update MP from server
        if (this._healthBar != null)
            this._healthBar.SetMP(roleState.CurMP, roleState.MaxMP);

        // set die from server
        if (roleState.Action == RoleAction.Dead)
        {
            this.hp = 0;
            StartCoroutine(OnDied());
        }
               
        if (damage > 0)
        {
            this.damageGet = Mathf.Abs(damage);
            this.TakingDamage();
            if (role.Base.UserId == GameManager.GameUser.Id)
                this.InitTextDamage(Color.green);
            else
                this.InitTextDamage(Color.red);

           
            float curHPPercent = hp / maxhp;

            float l = Mathf.Lerp(2.0f / 3.0f, 0.1f, curHPPercent);

            if (damage > 10)
            {
                if (Random.Range(0.0f, 1.0f) <= l)
                {
                    OnHit();
                    //Debug.Log("Hit " + curHPPercent.ToString("0.0") + " " + l);
                }
            }

        }
        else//Buff
        {
            this.damageGet = Mathf.Abs(damage);
            if (this.damageGet > 1)
                this.InitTextDamage(Color.green, true);
        }

        if (this.hp <= 0)
        {
            //Debug.Log("this.hp " + this.hp);
            actionStat = ActionStat.Dead;
            if (isMob)
            {
                UIBattleManager.Instance.OnMonsterIsDie();
            }
        }
        else
        {
            CheckEffect(roleState);
        }
    }
    public void OnReciveSkillCast(int skillID)
    {
        if (actionStat == ActionStat.Move || actionStat == ActionStat.Dead || actionStat == ActionStat.Skill) return;

        sendingSkill = false;
        RoleSkill roleSkill = role.RoleSkills.FirstOrDefault(p => p.SkillId == skillID);

        curSkill = roleSkill;

        if (roleSkill != null)
        {
            this.HandleSkillCast(role.RoleSkills.IndexOf(roleSkill));        
        }
    }
    public void OnReciveAction(GameObject targetAction)
    {
        if (actionStat == ActionStat.Dead || actionStat == ActionStat.hit) return;

        if (_myTransform == null) return;
        this.HandleAction(targetAction);
    }
    public void UpdateTransform(Vector3 pos)
    {
        if (_myTransform == null) return;
        this.HandleMove(pos);
      
    }
    public void OnSysnPosition(Vector3 pos)
    {
        if (_myTransform == null) return;

        //Debug.Log("OnSysnPosition " + pos);
        if (Vector3.Distance(_myTransform.position, pos) > 1.0f)
            _myTransform.position = pos;
    }
    public void SendSkillCast(RoleSkill roleSkill)
    {

        if (target == null)
        {
            Controller controlerTarget;
            if (role.Base.Class != RoleClass.Healer)
                controlerTarget = stateManager.FindEnemyNearest(true);
            else
                controlerTarget = stateManager.FindAllyNearest();

            if (controlerTarget != null)
            {
                OnAction(controlerTarget.gameObject);
            }
        }

        if (tag == GameManager.PlayerTagName)
        {
            if (_castRange != null) Destroy(_castRange);
            _castRange = SkillManager.Instance.CreateCastRange(_myTransform, roleSkill.GameSkill.CastRange);
        }

        GameplayManager.Instance.SendSkillCast(role.Id, roleSkill.SkillId);
        sendingSkill = true;

    }
    #endregion

    #region public methods

    //reborn controller 
    public void Revive()
    {
        Debug.Log(name + " Revive");
        gameObject.SetActive(true);
        transform.position = _startPos;

        actionStat = ActionStat.Idle;
        hp = role.State.MaxHP;
        role.State.CurHP = role.State.MaxHP;
        role.State.CurMP = role.State.MaxMP;

        this.enabled = true;
        if (stateManager != null)
            stateManager.enabled = true;

        if (_healthBar != null)
            _healthBar.gameObject.SetActive(true);
        if (uiSlot != null)
            uiSlot.Revive();


        //reset cooldown
        value_CoolDown_Skill_1 = 0;
        value_CoolDown_Skill_2 = 0;
       // StartCoroutine("CalCoolDownSkill_1");
       // StartCoroutine("CalCoolDownSkill_2");
    }
    public void SetRole(UserRole userRole, string tagName)
    {
        role = userRole;
        maxhp = userRole.State.MaxHP;
        hp = userRole.State.MaxHP;
        speedMove = userRole.State.MoveSpeed;
        actionValue = userRole.State.AttackValue;
        actionSpeed = userRole.State.AtkDelay;
        roleID = userRole.Base.RoleId;
        level = userRole.Base.Level;
        roleName = userRole.Name;
        attribute.Update();

        ConfigAnimation();

        tag = tagName;


        for (int i = 0; i < userRole.RoleSkills.Count; i++)
        {
            if (i == 1)
            {
                coolDown_Skill_1 = userRole.RoleSkills[i].GameSkill.CoolTime;
                //Debug.Log("Skill1 " + gameObject.name + " " + coolDown_Skill_1);
            }
            if (i == 2)
            {
                coolDown_Skill_2 = userRole.RoleSkills[i].GameSkill.CoolTime;
                //Debug.Log("Skill2 " + gameObject.name + " " + coolDown_Skill_2);
            }
        }

        if (tagName == GameManager.EnemyTagName)
        {
            textureHpFull = Resources.Load<Texture2D>("Images/HealthBarEnemy");
            if (GameManager.battleType == BattleMode.RandomPvA || isMob)
            {
                if (stateManager != null)
                    stateManager.enabled = true;
            }
        }
        else if (tag == GameManager.PlayerTagName)
        {
            UserRole myRole = GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == userRole.Id);
            if (myRole != null)
            {
                myRole.Base.Energy = userRole.Base.Energy;
            }

            //if (GameManager.battleType == BattleMode.RandomPvA || GameManager.battleType == BattleMode.RandomPvE)
            //{
            if (role.Base.Type != DEngine.Common.GameLogic.RoleType.Mob)
            {
                stateManager.enabled = true;
            }
            //}
        }
        _startPos = role.State.TargetPos;
        //stateManager.enabled = false;
        role.RoleItems.Clear();
        var roleItems = GameManager.GameUser.UserItems.Where(p => p.RoleUId == role.Id);
        role.RoleItems.AddRange(roleItems);

        UserItem consume = role.RoleItems.FirstOrDefault(p => p.GameItem.Kind == (int)ItemKind.Support);
        if (consume != null)
        {
            itemIconEquiped = Helper.LoadTextureForSupportItem(consume.ItemId);
            consumeEquiped = consume;
        }

        if (role.RoleSkills.Count > 1)
            icon_Skill_1 = Helper.LoadTextureForSkill(role.RoleSkills[1].SkillId);

        if (role.RoleSkills.Count > 2)
            icon_Skill_2 = Helper.LoadTextureForSkill(role.RoleSkills[2].SkillId);

        PlayAura();
   
    }
    public void ConfigAnimation()
    {     
        if (skill_1 != null)
        {
            if (skill_1.length > 1)
                animation[skill_1.name].speed = skill_1.length * 0.5f;
        }

        if (skill_2 != null)
        {
            if (skill_2.length > 1)
                animation[skill_2.name].speed = skill_2.length * 0.5f;
        }

        if (walk != null)
        {
            if (walk.length > 1)
                animation[walk.name].speed = walk.length * 0.6f;
            _moveAnimSpeed = animation[walk.name].speed;
        }

        if (action != null)
        {
            if (action.length > 1)
                animation[action.name].speed = action.length;
            _actionAnimSpeed = animation[action.name].speed;
        }

        if (action1 != null)
        {
            if (action1.length > 1)
                animation[action1.name].speed = action1.length;
            _skill1AnimSpeed = animation[action1.name].speed;
        }

        if (action2 != null)
        {
            if (action2.length > 1)
                animation[action2.name].speed = action2.length;
            _skill2AnimSpeed = animation[action2.name].speed;
        }

        if (roleType != RoleType.Building)
            ChangeActionTime(actionSpeed);


        if (hit != null)
        {
            if (hit.length > 0.8f)
                animation[hit.name].speed = hit.length / 0.8f;
        }
     
    }
    public void OnIdle()
    {        
        countAction = -1;
        actionStat = Controller.ActionStat.Idle;
    }

    //send move to server
    public void OnMove(Vector3 pos)
    {
        //if (actionStat == Controller.ActionStat.hit)
        //{
        //    UIEffectInfoManager.Instance.Add(GameManager.localization.GetText("ErrorCodeSkill_HitNoMove"));
        //    return;
        //}

        if (actionStat == Controller.ActionStat.Dead) return;

        GameplayManager.Instance.SendMove(role.Id, index, pos);
    }

    //send to server and all player (SubCode.Action) , this character wants to impact on target
    public void OnAction(GameObject targetAction)
    {
        if (isLockAuto) return;

        if (actionStat == Controller.ActionStat.Dead || actionStat == Controller.ActionStat.Skill) return;


        if (actionStat == Controller.ActionStat.hit)
        {
            UIEffectInfoManager.Instance.Add(GameManager.localization.GetText("ErrorCodeSkill_HitNoMove"));
            return;
        }

        if (role.RoleSkills.Count > 0)
        {
            if (role.RoleSkills.FirstOrDefault(p => p.GameSkill.Id == 56) != null)
            {

                //send to server , This character wants to impact on target
                StartCoroutine(GameplayManager.Instance.SendAction(role.Id, index, targetAction.GetComponent<Controller>(), 10));
                isLockAuto = true;
                return;
            }
        }

        GameplayManager.Instance.SendAction(role.Id, index, targetAction.GetComponent<Controller>());
    }
    public void OnSkillCast(int curSkillIndex)
    {
        if (actionStat == Controller.ActionStat.Dead 
            || actionStat == Controller.ActionStat.Skill
            || actionStat == Controller.ActionStat.Move) return;      
  
        SendSkillCast(role.RoleSkills[curSkillIndex]);     
    }
    public void OnSelected()
    {
        foreach (GameObject go in objMeshAndSkinMesh)
        {
            if (go != null)
            {
                foreach (Material mat in go.renderer.materials)
                {
                    mat.shader = Resources.Load<Shader>("Shaders/Toony-BasicOutline");
                    if (tag == GameManager.PlayerTagName)
                    {
                        mat.SetColor("_OutlineColor", new Color(0, 0.8f, 1, 0.7f));
                    }
                    else
                        mat.SetColor("_OutlineColor", new Color(0.8f, 0f, 0f, 0.7f));

                    mat.SetFloat("_OuntLine", 0.0f);
                }
            }
        }
    }
    public void OnDeSelected()
    {
        foreach (GameObject go in objMeshAndSkinMesh)
        {
            if (go != null)
            {
                foreach (Material mat in go.renderer.materials)
                {
                    mat.shader = Resources.Load<Shader>("Shaders/Toony-Basic");
                }
            }
        }
    }
    public bool isControl()
    {
        if (_effect.ContainsKey(EffectType.Stun)) return false;
        return true;
    }
    public UIBattleManager.ErrorCodeSkill LockedSkill(int skillIndex)
    {
        if (GameplayManager.battleStatus != GameplayManager.BattleStatus.Playing && !isMobTutorial)
        {
            return UIBattleManager.ErrorCodeSkill.Other;

        }

        if (role.RoleSkills.Count <= 0) return UIBattleManager.ErrorCodeSkill.Other;
        if (curSkillIndexSelect == skillIndex) return UIBattleManager.ErrorCodeSkill.Other;
        if (_effect.ContainsKey(EffectType.Silent)) return UIBattleManager.ErrorCodeSkill.Silent;


        if (skillIndex == 1)
        {
            if (value_CoolDown_Skill_1 > 0) return UIBattleManager.ErrorCodeSkill.coldown;
            if (skill_1 == null) return UIBattleManager.ErrorCodeSkill.Other;
        }

        else if (skillIndex == 2)
        {
            if (value_CoolDown_Skill_2 > 0) return UIBattleManager.ErrorCodeSkill.coldown;
            if (skill_2 == null) return UIBattleManager.ErrorCodeSkill.Other;
        }

        RoleSkill roleSkill = role.RoleSkills[skillIndex];

        if (roleSkill.GameSkill.SkillType == (int)SkillType.Aura) return UIBattleManager.ErrorCodeSkill.Aura;  

        switch ((CostType)roleSkill.GameSkill.CostType)
        {
            case CostType.None:
                break;
            case CostType.MPValue:
                if (role.State.CurMP < roleSkill.CostValue)
                    return UIBattleManager.ErrorCodeSkill.Mana;
                break;
            case CostType.HPValue:
                if (role.State.CurHP <= roleSkill.CostValue)
                    return UIBattleManager.ErrorCodeSkill.HP;
                break;

            default: break;
        }


        return UIBattleManager.ErrorCodeSkill.Success;
    }
    public void AddForce(Vector3 force)
    {
        GetComponent<Rigidbody>().AddForce(force);
    }
    public void OnHit()
    {
        if (actionStat == ActionStat.Skill)
        {
            return;
        }

        if (actionStat != ActionStat.hit && actionStat != ActionStat.Dead)
        {
            //if (IsMyHero() && !isMob)
            //{
            //    Debug.Log(name + ": " + actionStat + " -> Hit");
            //}

            _oldActionStat = actionStat;
            actionStat = ActionStat.hit;

            if (hit != null)
                Invoke("DeHit", CalculatorTimeAnim(hit));
            else
                Invoke("DeHit", 0.5f);      
        }
    }
    public IEnumerator OnDied()
    {
        actionStat = ActionStat.Dead;
        float timeDestroy = 0.8f;

        if (dead != null)
        {
            timeDestroy += animation[dead.name].length;
        }     
        
        SoundManager.Instance.PlayDeath(this);
        
        
        yield return new WaitForSeconds(timeDestroy);

        if (isMob)
        {
            Destroy(gameObject);
            Destroy(_healthBar.gameObject);
            GameplayManager.Instance.Remove(this);
        }
        else
        {
            gameObject.SetActive(false);
            if (_healthBar != null)
                _healthBar.gameObject.SetActive(false);
            if (uiSlot != null)
                uiSlot.Died();
        }
    }
    public bool IsMyHero()
    {
        return role.Base.UserId == GameManager.GameUser.Id;
    }
    public bool isSkillNeedTarget(GameSkill curSkill)
    {
        if (curSkill.TargetType == (int)TargetType.AllyOne ||
            curSkill.TargetType == (int)TargetType.EnemyOne ||
            (curSkill.TargetType == (int)TargetType.AreaEffect && curSkill.EffectRange > 0)
            || curSkill.Id == 29)//Garuda
        {
            return true;
        
        }
        else
        {
            return false;
        }
    }
    public void RemoveEffect(EffectType type)
    {
        if (_effect.ContainsKey(type))
        {
            _effect.Remove(type);
        }
    }
    public void MoveToStartPos()
    {
        if (actionStat == ActionStat.Dead) return;  
       // positionWay = _startPos;
        if (GameplayManager.battleStatus == GameplayManager.BattleStatus.Win)
        {
            HandleMove(_startPos);
        }
        else
            OnMove(_startPos);
    }
    public bool IsStandStartPos()
    {
        Vector3 curPos = _myTransform.transform.position;
        curPos.y = 0;
        if (Vector3.Distance(curPos, _startPos) <= 1.0f)
        {
            if (GameManager.Status == GameStatus.Dungeon && GameManager.battleType == BattleMode.RandomPvE)
            {

                OnIdle();
               // _myTransform.LookAt(Vector3.zero);
            }
            return true;
        }
        return false;
    }
    public void HideHealthBar()
    {
        if (_healthBar != null)
            _healthBar.gameObject.SetActive(false);
    }
    public void SetStartPos(Vector3 pos)
    {
        _startPos = pos;
    }
    #endregion

    #region private mehtods
    private void PlayAura()
    {
        foreach (RoleSkill skill in role.RoleSkills)
        {
            if (skill.GameSkill.SkillType == (int)SkillType.Aura)
            {
                SkillManager.Instance.DoSkill(this, skill);
            }
        }
    }
    private void ChangeActionTime(float time)
    {
        if (time < CalculatorTimeAnim(action))
        {
            animation[action.name].speed /= time; 
        }
        actionSpeed = time;
    }
    private void HandleMove(Vector3 pos)
    {
        //if (IsMyHero() && !isMob)
        //{
        //    Debug.Log("HandleMove " + pos);
        //}

        positionWay = pos;
        actionStat = ActionStat.Move;        
    }
    //network event Action
    private void HandleAction(GameObject targetAction)
    {
        if (actionStat == ActionStat.Move || actionStat == ActionStat.Dead || actionStat == ActionStat.Skill) return;
        target = targetAction;       
        actionStat = ActionStat.Action;
    }

    //network event active skill
    private void HandleSkillCast(int curSkillIndex)
    {
        curSkillIndexSelect = curSkillIndex;
        if (curSkillIndexSelect == 0)
        {
            countAction = 0;
            actionStat = ActionStat.Action;
        }
        else if (curSkillIndexSelect == 1)
        {
            _timerAnim = 0;
            actionStat = ActionStat.Skill;
            SkillHandle = Skill_1_Cast;
        }
        else if (curSkillIndexSelect == 2)
        {
            _timerAnim = 0;
            actionStat = ActionStat.Skill;
            SkillHandle = Skill_2_Cast;
        }
    }
    private void CheckEffect(UserRole.RoleState roleState)
    {
        foreach (UserRole.MagicState magicAttrib in roleState.MagicStates)
        {
            if (magicAttrib.EffectType != EffectType.None && magicAttrib.AttribType != AttribType.None)
            {
                if (!_effect.ContainsKey(magicAttrib.EffectType))
                {
                    //_effectManager.AddEffect(magicAttrib);
                    //if (isConnectedServer())
                    //{
                    //    if (role.Base.UserId == GameManager.GameUser.Id)
                    //        UITextManager.Instance.createText(string.Format(GameManager.localization.GetText("Battle_Effect"), role.Name, magicAttrib.EffectType.ToString()));
                    //}

                    _effect.Add(magicAttrib.EffectType, magicAttrib);
                    _healthBar.AddEffect(magicAttrib);


                    switch (magicAttrib.EffectType)
                    {
                        case EffectType.Stun:
                            {
                                //Debug.Log(gameObject.name + " Stun");
                                _oldMoveSpeed = speedMove;
                                _oldActionSpeed = actionSpeed;
                                PauseAnim();
                                Invoke("DePauseAnim", magicAttrib.RemainTime);
                            }
                            break;
                        case EffectType.Slow:
                            {
                                Slow(Mathf.Abs(magicAttrib.AffectValue / 100.0f));
                                Invoke("RestoreAnimationSpeed", magicAttrib.RemainTime);
                            }
                            break;
                        case EffectType.BattleMage:
                        case EffectType.ATSup:
                            {
                                float percent = magicAttrib.AffectValue / 100.0f;
                                animation[action.name].speed = _actionAnimSpeed * (1 + percent);
                                Invoke("RestoreAnimationSpeed", magicAttrib.RemainTime);
                            }
                            break;
                        case EffectType.ATSdown:
                            {
                                float percent = magicAttrib.AffectValue / 100.0f;
                                animation[action.name].speed = _actionAnimSpeed * (1 - percent);
                                Invoke("RestoreAnimationSpeed", magicAttrib.RemainTime);
                            }
                            break;
                        case EffectType.MVSup:
                            {
                                float percent = magicAttrib.AffectValue / 100.0f;
                                animation[walk.name].speed = _moveAnimSpeed * (1 + percent);
                                Invoke("RestoreAnimationSpeed", magicAttrib.RemainTime);
                            }
                            break;
                        case EffectType.Cleansing:
                            {
                                _healthBar.ClearAllEffect();
                                RestoreAnimationSpeed();
                            }
                            break;
                    }

                }
            }
        }

        actionSpeed = roleState.AtkDelay;
        speedMove = roleState.MoveSpeed;


        CheckAndRemoveEffect(roleState.MagicStates);

    }
    private void CheckAndRemoveEffect(UserRole.MagicState[] states)
    {
        for (int i = 0; i < _effect.Count; i++)
        {
            var effect = _effect.ElementAt(i);
            if (states.FirstOrDefault(p => p.EffectType == effect.Key) == null)
            {
                RemoveEffect(effect.Key);
                _healthBar.RemoveEffect(effect.Key);
                i--;
            }
        }
    }
    private void RemoveAllEffect()
    {
        _effect.Clear();
    }    
    private void InitAttribute()
    {
        attribute = new RoleAttribute(roleID);
        attribute.SetBaseAttribute(strength, agility, intelligent);

        maxhp = attribute.maxHP;
        actionValue = attribute.physicAttack;
        def = attribute.physicDefence;


        /*Debug.Log("----Name " + roleName);
        Debug.Log("strength " + attribute.strength);
        Debug.Log("agility " + attribute.agility);
        Debug.Log("intelligent " + attribute.intelligent);
        Debug.Log("maxHP " + attribute.maxHP);
        Debug.Log("physicDefence " + attribute.physicDefence);
        Debug.Log("critPower " + attribute.critPower);
        Debug.Log("attackSpeed " + attribute.attackSpeed);
        Debug.Log("physicAttack " + attribute.physicAttack);
        Debug.Log("critRate " + attribute.critRate);
        Debug.Log("magicAttack " + attribute.magicAttack);
        Debug.Log("magicDefence " + attribute.magicDefence);
        Debug.Log("maxMP " + attribute.maxMP);
        Debug.Log("range " + attribute.range);*/
    }
    private void InitHealthBar()
    {
        string path = "Prefabs/UI/Battle/HUDHealthBar";
        if (!isMob)
            path = "Prefabs/UI/Battle/HUDHeroHealthBar";

        GameObject hudRoot = GameObject.FindGameObjectWithTag("HUDRoot");
        if (hudRoot != null)
        {
            GameObject go = NGUITools.AddChild(hudRoot, Resources.Load(path) as GameObject);
            _healthBar = go.GetComponent<UIHUDHealthBarController>();
            go.GetComponent<UIPanel>().depth = index;
            _healthBar.Init(_myTransform, gameObject.tag, this);
            go.transform.localScale = new Vector3(healthbarSize.x, healthbarSize.y, 1);
        }
    }
    private float CalculatorTimeAnim(AnimationClip clip)
    {
        return clip.length / animation[clip.name].speed;
    }
    private void PauseAnim()
    {
        if (animation != null)
            animation.enabled = false;
    }
    private void DePauseAnim()
    {
        if (actionStat != ActionStat.Dead)
        {
            OnIdle();
            RemoveEffect(EffectType.Stun);
        }
        speedMove = _oldMoveSpeed;
        actionSpeed = _oldActionSpeed;
        if (animation != null)
            animation.enabled = true;
    }
    private void DeHit()
    {
        if (actionStat != ActionStat.hit || actionStat == ActionStat.Dead) return;
        if (_oldActionStat != ActionStat.Skill)
            actionStat = _oldActionStat;
        else
            actionStat = ActionStat.Idle;
    }
    private void Slow(float percent)
    {
        if (walk != null)
            animation[walk.name].speed = _moveAnimSpeed * (1 - percent);
        if (action != null)
            animation[action.name].speed = _actionAnimSpeed * (1 - percent);
        if (action1 != null)
            animation[action1.name].speed = _skill1AnimSpeed * (1 - percent);
        if (action2 != null)
            animation[action2.name].speed = _skill2AnimSpeed * (1 - percent);
    }
    private void RestoreAnimationSpeed()
    {
        if (walk != null)
            animation[walk.name].speed = _moveAnimSpeed;
        if (action != null)
            animation[action.name].speed = _actionAnimSpeed;
        if (action1 != null)
            animation[action1.name].speed = _skill1AnimSpeed;
        if (action2 != null)
            animation[action2.name].speed = _skill2AnimSpeed;
    }
    #endregion

}
