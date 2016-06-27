using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;
using System.Linq;

public class UIBattleManager : MonoBehaviour
{
    public enum ErrorCodeSkill
    {
        Success,
        Mana,
        HP,
        Silent,
        coldown,
        Aura,
        Other
    };

    [System.Serializable]
    public struct UITutorial
    {
        public GameObject root;
        public GameObject hand;
        public GameObject effectClick;
        public Transform startPoint;
        public Transform endPoint;

        public GameObject arrowTouchHero;
        public GameObject arrowSkill1;
        public GameObject arrowSkill2;
        public GameObject arrowHero1;
        public GameObject arrowHero2;
        public GameObject arrowHero3;
        public GameObject arrowAutoSkill;
        public GameObject arrowWorldMap;
    }

    public GameObject skillRoot;
    public UITexture iconSkill1;
    public UITexture iconSkill2;
    public UISprite cooldownSkill1;
    public UISprite cooldownSkill2;
    public UISprite iconDisableSkill1;
    public UISprite iconDisableSkill2;
    public GameObject textCooldownRoot;
    public GameObject lblTextCooldowm;

    public UIButton btnItem;
    public UITexture iconItem;
    public UILabel lblAmount;
    public UISprite cooldownItem;

    public UILabel lblTimer;
    public UILabel lblWave;
    public UILabel lblButtonSpeed;
    public UIBattleResultNew battleResult;
    public static UIBattleManager Instance { get { return _instance; } }

    public UIToggle chkAutoSkill;

    public bool disableSkill1
    {
        get { return _disableSkill1; }
        set
        {
            _disableSkill1 = value;
            if (_disableSkill1)
            {
                iconDisableSkill1.gameObject.SetActive(true);
            }
            else
            {
                iconDisableSkill1.gameObject.SetActive(false);
            }
        }
    }
    public bool disableSkill2
    {
        get { return _disableSkill2; }
        set
        {
            _disableSkill2 = value;
            if (_disableSkill2)
            {
                iconDisableSkill2.gameObject.SetActive(true);
            }
            else
            {
                iconDisableSkill2.gameObject.SetActive(false);
            }
        }
    }
    public UIToggle uiToggleAuto;
    public UIPlayTween tweenclose;

    public UITutorial uiTutorial;

    private bool _isSelectedRole;
    private bool _disableSkill1;
    private bool _disableSkill2;
    private float _timerCoolTime = 5;
    private float _timerLastCoolTime;
    private static UIBattleManager _instance;
    private int _timeScale;

    void Start()
    {
        _instance = this;
        _isSelectedRole = false;
        disableSkill1 = false;
        disableSkill2 = false;
        _timeScale = 1;

        if (GameManager.battleType == BattleMode.Challenge || GameManager.battleType == BattleMode.RandomPvP)
        {
            //uiToggleAuto.gameObject.SetActive(false);
            chkAutoSkill.gameObject.SetActive(false);
        }

        _timerLastCoolTime = Time.time;

        if (GameManager.battleType == BattleMode.RandomPvE)
        {
            lblTimer.gameObject.SetActive(false);
        }

        if (GameManager.battleType == BattleMode.RandomPvP || GameManager.battleType == BattleMode.Challenge)
        {
            lblButtonSpeed.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            lblButtonSpeed.text = string.Format(GameManager.localization.GetText("Battle_Speed"), _timeScale);
        }

        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            lblButtonSpeed.transform.parent.gameObject.SetActive(false);
        }
    }

    private void ShowEnd()
    {
    }

    void Update()
    {      

        UpdateTimer();

        if (GameplayManager.battleStatus < GameplayManager.BattleStatus.Begin)
            return;

        if (GameManager.battleType != BattleMode.RandomPvE)
        {
            if (_timerCoolTime >= 0)
            {
                if (Time.time - _timerLastCoolTime >= 1)
                {
                    HandleTimerBeginBattle();
                    _timerLastCoolTime = Time.time;
                }
            }
        }

        

        UpdateUISkill();
               
    }

    void OnDestroy()
    {
        Time.timeScale = 1;
    }

    #region private methods
    public void UpdateUISkill()
    {
        if (TouchController.instance.controllerGetSkill == null)
            Debug.Log("TouchController.instance.controllerGetSkill == null");

        if (TouchController.instance.controllerGetSkill != null)
        {
            OnSelectedRole();

            Controller controller = TouchController.instance.controllerGetSkill;
            iconSkill1.mainTexture = controller.icon_Skill_1;
            iconSkill2.mainTexture = controller.icon_Skill_2;

            cooldownSkill1.fillAmount = controller.value_CoolDown_Skill_1;
            cooldownSkill2.fillAmount = controller.value_CoolDown_Skill_2;

          //  Debug.Log("controller.value_CoolDown_Skill_1: "+ controller.value_CoolDown_Skill_1+" time "+Time.deltaTime);


            if (controller.consumeEquiped != null)
            {
                iconItem.mainTexture = controller.itemIconEquiped;
                lblAmount.text = "x" + controller.consumeEquiped.Count;
                cooldownItem.fillAmount = controller.itemCooldown / BattleConfig.ITEM_USEDELAY;


            }
            else
            {
                iconItem.mainTexture = null;
                lblAmount.text = "";
                cooldownItem.fillAmount = 0;
            }

            StateManager stateManager = controller.GetComponent<StateManager>();
            uiToggleAuto.value = stateManager.enabled;
            chkAutoSkill.value = TouchController.instance.controllerGetSkill.autoSkill;

            if(controller.LockedSkill(1) != ErrorCodeSkill.Success)
                disableSkill1 = true;
            else
                disableSkill1 = false;


            if (controller.LockedSkill(2) != ErrorCodeSkill.Success)
                disableSkill2 = true;
            else
                disableSkill2 = false;      
        }
    }

    private void HandleTimerBeginBattle()
    {
        //Debug.Log("HandleTimerBeginBattle " + Time.time);
        GameObject go = NGUITools.AddChild(textCooldownRoot, lblTextCooldowm);
        go.SetActive(true);
        if (_timerCoolTime > 0)
        {
            // go.GetComponent<UILabel>().text = _timerCoolTime.ToString("0");
            go.GetComponent<UISprite>().spriteName = _timerCoolTime.ToString("0");
        }
        else
        {
            //go.GetComponent<UILabel>().text = "Fight!";
            go.GetComponent<UISprite>().spriteName = "BattleFightight";
            go.GetComponent<UISprite>().MakePixelPerfect();
            GameplayManager.battleStatus = GameplayManager.BattleStatus.Playing;
        }

        _timerCoolTime -= 1;
    }

    private void UpdateTimer()
    {
        if (GameplayManager.Instance == null) return;
        float dt = GameManager.battleTime;
        float m = dt / 60.0f;

        int playerDamage = 0;
        int enemyDamage = 0;

        foreach (Hero hero in GameplayManager.Instance.allheroSet)
        {
            if (hero.controller.role.Base.UserId == GameManager.GameUser.Id)//GameplayManager.Instance.isMyHero(hero))
            {
                playerDamage += hero.damage;
            }
            else
            {
                enemyDamage += hero.damage;
            }
        }

        int leftPoint = 0;
        int rightPoint = 0;

        if (GameManager.GameUser.Position == 0)
        {
            rightPoint = playerDamage;
            leftPoint = enemyDamage;
        }
        else
        {
            rightPoint = enemyDamage;
            leftPoint = playerDamage;
        }
        lblTimer.text = string.Format("{0:00} : {1:00}\n{2:0} - {3:0}", (int)m, (int)dt % 60, leftPoint, rightPoint);
    }    
    #endregion

    #region Button
    public void OnQuitBattle_Click()
    {
        MyInput.HandleExitBattle();
    }
    private void OnSelectedRole()
    {
        if (_isSelectedRole == true) return;
        skillRoot.SetActive(true);
        _isSelectedRole = true;
    }

    public void OnButtonSkill1_Click()
    {
        Controller controller = TouchController.instance.controllerGetSkill;

        if (!controller.isControl()) return;
        OnSkilClick();
        if (controller.actionStat != Controller.ActionStat.Skill && controller.actionStat != Controller.ActionStat.Dead)
        {
            if (controller.isConnectedServer())
            {
                ErrorCodeSkill errorSkill = controller.LockedSkill(1);
                if (errorSkill != ErrorCodeSkill.Success)
                {
                    HandleErrorCodeSkill(errorSkill);
                    return;
                }
                controller.SendSkillCast(controller.role.RoleSkills[1]);
            }
            else
            {
                controller.SkillHandle = controller.Skill_1_Cast;
                controller.actionStat = Controller.ActionStat.Skill;
                controller.curSkillIndexSelect = 1;
            }

        }
    }

    public void OnButtonSkill2_Click()
    {

        Controller controller = TouchController.instance.controllerGetSkill;
             
        if (!controller.isControl()) return;
        OnSkilClick();
        if (controller.actionStat != Controller.ActionStat.Skill && controller.actionStat != Controller.ActionStat.Dead)
        {
            if (controller.isConnectedServer())
            {
                ErrorCodeSkill errorSkill = controller.LockedSkill(2);
                if (errorSkill != ErrorCodeSkill.Success)
                {
                    HandleErrorCodeSkill(errorSkill);
                    return;
                }


                controller.SendSkillCast(controller.role.RoleSkills[2]);
            }
            else
            {
                controller.SkillHandle = controller.Skill_2_Cast;
                controller.actionStat = Controller.ActionStat.Skill;
                controller.curSkillIndexSelect = 2;
            }

        }
    }

    public void OnButtonUseItem_Click()
    {
        if (TouchController.instance.controllerGetSkill != null)
        {
            Controller controller = TouchController.instance.controllerGetSkill;
            if (controller.consumeEquiped != null)
            {
                if (controller.consumeEquiped.Count > 0)
                {
                    if (controller.itemCooldown <= 0)
                    {
                        GameplayManager.Instance.SendUseItem(controller.role.Id, controller.consumeEquiped.Id);
                        controller.consumeEquiped.Count -= 1;
                        controller.itemCooldown = BattleConfig.ITEM_USEDELAY;
                    }
                }
            }
        }
    }

    public void OntoogleAutoSkill()
    {
        Controller curController = TouchController.instance.controllerGetSkill;
        if (curController != null)
        {
            curController.autoSkill = chkAutoSkill.value;
            if (chkAutoSkill.value == true)
            {
                UIEffectInfoManager.Instance.Add(string.Format(GameManager.localization.GetText("AutoSkill_SetAuto"), curController.roleName));
                OnToggleAutoSkill();
            }
        }
    }

    public void OnToogleAuto()
    {
        Controller curController = TouchController.instance.controllerGetSkill;

        if (curController == null)
        {
            uiToggleAuto.value = false;
            return;
        }
        else
        {
            StateManager stateManager = curController.GetComponent<StateManager>();
            stateManager.enabled = uiToggleAuto.value;

            if (uiToggleAuto.value == true)
            {
                if (GameManager.battleType == BattleMode.Challenge || GameManager.battleType == BattleMode.RandomPvP)
                {
                    return;
                }

                UIPlayTween playTween = new UIPlayTween();
                playTween.tweenTarget = chkAutoSkill.gameObject;
                playTween.playDirection = AnimationOrTween.Direction.Forward;
                playTween.ifDisabledOnPlay = AnimationOrTween.EnableCondition.EnableThenPlay;
                playTween.disableWhenFinished = AnimationOrTween.DisableCondition.DoNotDisable;
                playTween.Play(true);

            }
            else
            {

                tweenclose.Play(true);
            }
        }
    }

    public void OnButtonSpeed_Click()
    {
        _timeScale = _timeScale >= 3 ? 1 : _timeScale + 1;
        Time.timeScale = _timeScale;
        lblButtonSpeed.text = string.Format(GameManager.localization.GetText("Battle_Speed"), _timeScale);
    }
    #endregion

    #region Tutorial
    public Hero heroFirst;

    public enum TutorialStepForBattle
    {
        None,
        TabToHero,
        MoveHero,
        CreateMob,
        TargetToMob,
        AutoSkill,
    }
    public TutorialStepForBattle tutStep = TutorialStepForBattle.None;
       
    private Vector3 _tutorial_PosMove = new Vector3(-3.15f, 0, -3.3f);

    private IEnumerator MoveHand()
    {
       
        uiTutorial.hand.SetActive(true);
        uiTutorial.hand.transform.position = uiTutorial.startPoint.position;

        Hashtable hash = new Hashtable();
        hash["name"] = "movehand";
        hash["position"] = uiTutorial.endPoint;
        hash["time"] = 4;
        hash["easetype"] = iTween.EaseType.easeInOutExpo;
        hash["loopType"] = iTween.LoopType.loop;
        iTween.MoveTo(uiTutorial.hand, hash);

        yield return new WaitForSeconds(1);
        uiTutorial.effectClick.SetActive(false);
        //   yield return new WaitForSeconds(4);
        //  uiTutorial.hand.SetActive(false);
    }
    public void OnBattleStart()
    {
        Tutorial();
    }
    public void Tutorial()
    {
        if (GameManager.tutorial.step != TutorialManager.TutorialStep.Control_NPCFinshed) return;

        tutStep = TutorialStepForBattle.None;
        uiTutorial.root.gameObject.SetActive(true);
        heroFirst = GameplayManager.Instance.heroPlayerSet.FirstOrDefault(p => p.controller.role.GameRole.Class != (int)RoleClass.Healer);
        if (heroFirst == null) return;

        uiTutorial.arrowTouchHero.transform.localPosition = Helper.GetScreenPosOfWorldPos(heroFirst.gameObject.transform.position,
            uiTutorial.arrowTouchHero.transform, NGUITools.FindCameraForLayer(uiTutorial.startPoint.gameObject.layer));
        uiTutorial.arrowTouchHero.SetActive(true);

        uiTutorial.hand.SetActive(false);

        uiTutorial.arrowWorldMap.SetActive(true);

    }
    public void OnTouchHeroFinish()
    {

        if (GameManager.tutorial.step != TutorialManager.TutorialStep.Control_NPCFinshed) return;
        if (tutStep >= TutorialStepForBattle.TabToHero) return;
        if (heroFirst == null) return;
        if (TouchController.instance.controllerGetSkill != heroFirst.controller) return;

        tutStep = TutorialStepForBattle.TabToHero;

        uiTutorial.arrowTouchHero.SetActive(false);

        uiTutorial.hand.SetActive(true);

        uiTutorial.startPoint.localPosition = Helper.GetScreenPosOfWorldPos(heroFirst.gameObject.transform.position,
             uiTutorial.startPoint.transform, NGUITools.FindCameraForLayer(uiTutorial.startPoint.gameObject.layer));

        uiTutorial.endPoint.localPosition = Helper.GetScreenPosOfWorldPos(_tutorial_PosMove,
            uiTutorial.endPoint.transform, NGUITools.FindCameraForLayer(uiTutorial.endPoint.gameObject.layer));

        StartCoroutine(MoveHand());

    }
    public void OnMoveHero()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            if (uiTutorial.hand.activeInHierarchy)
            {
                if (uiTutorial.hand != null && uiTutorial.hand.GetComponent<iTween>() != null)
                    iTween.Stop();
                else
                    Debug.Log("*************NULL");
                uiTutorial.hand.SetActive(false);
            }

            if (tutStep >= TutorialStepForBattle.MoveHero) return;
            tutStep = TutorialStepForBattle.MoveHero;

        }
    }
    public void OnHeroFinishedMove(Controller con)
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            if (heroFirst == null) return;
            if (con == heroFirst.controller)
            {
                if (tutStep == TutorialStepForBattle.MoveHero)
                {
                    GameplayManager.Instance.CreateFirstMobForTutorial();
                    tutStep = TutorialStepForBattle.CreateMob;
                }

                OnTargetToMonster();
            }
        }
    }
    public void OnTargetToMonster()
    {
        if (GameManager.tutorial.step != TutorialManager.TutorialStep.Control_NPCFinshed) return;
        if (tutStep >= TutorialStepForBattle.TargetToMob) return;
        if (GameplayManager.Instance.heroEnemySet.Count <= 0)
        {
            Debug.Log("*************NULL");
            return;
        }
        uiTutorial.hand.SetActive(true);

        uiTutorial.startPoint.localPosition = Helper.GetScreenPosOfWorldPos(heroFirst.gameObject.transform.position,
             uiTutorial.startPoint.transform, NGUITools.FindCameraForLayer(uiTutorial.startPoint.gameObject.layer));

        uiTutorial.endPoint.localPosition = Helper.GetScreenPosOfWorldPos(GameplayManager.Instance.heroEnemySet[0].gameObject.transform.position,
            uiTutorial.endPoint.transform, NGUITools.FindCameraForLayer(uiTutorial.endPoint.gameObject.layer));

        uiTutorial.arrowTouchHero.SetActive(true);
        uiTutorial.arrowTouchHero.transform.localPosition = Helper.GetScreenPosOfWorldPos(GameplayManager.Instance.heroEnemySet[0].gameObject.transform.position,
            uiTutorial.arrowTouchHero.transform, NGUITools.FindCameraForLayer(uiTutorial.arrowTouchHero.layer));

        StartCoroutine(MoveHand());
    }
    public void OnFinishedTargetToMonster()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            if (tutStep >= TutorialStepForBattle.TargetToMob) return;

            tutStep = TutorialStepForBattle.TargetToMob;
            if (uiTutorial.hand.activeInHierarchy)
            {
                if (uiTutorial.hand != null && uiTutorial.hand.GetComponent<iTween>() != null)
                    iTween.Stop(uiTutorial.hand);
                else
                    Debug.Log("*************NULL");
                uiTutorial.hand.SetActive(false);

            }
            uiTutorial.arrowTouchHero.SetActive(false);
            GameplayManager.Instance.heroEnemySet[0].controller.isMobTutorial = true;

            if (heroFirst.controller.role.RoleSkills.Count > 1)
            {
                if (heroFirst.controller.role.RoleSkills[1].GameSkill.SkillType != (int)SkillType.Aura)
                {
                    uiTutorial.arrowSkill1.SetActive(true);
                }
                else
                {
                    uiTutorial.arrowSkill2.SetActive(true);
                }
            }

            heroFirst.controller.isMobTutorial = true;
        }
    }
    public void OnSkilClick()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            uiTutorial.arrowSkill1.SetActive(false);
            uiTutorial.arrowSkill2.SetActive(false);
        }
    }
    public void OnMonsterIsDie()
    {

        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            if (tutStep >= TutorialStepForBattle.AutoSkill) return;

            if (uiTutorial.hand.activeInHierarchy)
            {
                iTween.StopByName("movehand");
                uiTutorial.hand.SetActive(false);
            }
            uiTutorial.arrowSkill1.SetActive(false);
            uiTutorial.arrowSkill2.SetActive(false);

            tutStep = TutorialStepForBattle.AutoSkill;
            OnToggleAutoSkill();
        }
    }
    public void OnToggleAutoSkill()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            uiTutorial.arrowAutoSkill.SetActive(false);
            uiTutorial.arrowHero1.SetActive(false);
            uiTutorial.arrowHero2.SetActive(false);
            uiTutorial.arrowHero3.SetActive(false);
            TutorialAutoskill();

            //click checkbox  auto skill for three heroes is completed , create mob 
            if (GameplayManager.Instance.heroPlayerSet.Count(p => p.controller.autoSkill) >= 3)
            {
                StartCoroutine(CreateMob());
            }
        }
    }
    private IEnumerator CreateMob()
    {
        yield return new WaitForSeconds(2);
        GameplayManager.Instance.ResumeGame();
        StartCoroutine(GameplayManager.Instance.CreateMobsNextWave());
    }
    private void TutorialAutoskill()
    {
        if (tutStep != TutorialStepForBattle.AutoSkill) return;


        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {

            for (int i = 0; i < GameplayManager.Instance.heroPlayerSet.Count; i++)
            {
                if (!GameplayManager.Instance.heroPlayerSet[i].controller.autoSkill)
                {

                    if (i == 0)
                    {
                        uiTutorial.arrowHero1.SetActive(true);
                        break;
                    }
                    if (i == 1)
                    {
                        uiTutorial.arrowHero2.SetActive(true);
                        break;
                    }
                    if (i == 2)
                    {
                        uiTutorial.arrowHero3.SetActive(true);
                        break;
                    }

                }
            }
        }

    }
    public void OnSelectedHeroSlot()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed && tutStep == TutorialStepForBattle.AutoSkill)
        {
            uiTutorial.arrowHero1.SetActive(false);
            uiTutorial.arrowHero2.SetActive(false);
            uiTutorial.arrowHero3.SetActive(false);

            if (TouchController.instance.controllerGetSkill != null)
            {
                if (!TouchController.instance.controllerGetSkill.autoSkill)
                {
                    uiTutorial.arrowAutoSkill.SetActive(true);
                }
                else
                {
                    uiTutorial.arrowAutoSkill.SetActive(false);

                }
            }
        }
    }
    #endregion

    #region private
    private void HandleErrorCodeSkill(ErrorCodeSkill errorCodeSkill)
    {
        string s = "";
        switch (errorCodeSkill)
        {
            case ErrorCodeSkill.Mana:
                s = GameManager.localization.GetText("ErrorCodeSkill_NoManaForSkill");
                break;
            case ErrorCodeSkill.HP:
                s = GameManager.localization.GetText("ErrorCodeSkill_NoHealthForSkill");
                break;
            case ErrorCodeSkill.Silent:
                s = GameManager.localization.GetText("ErrorCodeSkill_Silent");
                break;
            case ErrorCodeSkill.coldown:
                s = GameManager.localization.GetText("ErrorCodeSkill_Coldown");
                break;
            case ErrorCodeSkill.Aura:
                s = GameManager.localization.GetText("ErrorCodeSkill_Aura");
                break;
        }
        if (s != "")
            UIEffectInfoManager.Instance.Add(s);
    }
    #endregion
}
