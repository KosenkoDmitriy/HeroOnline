using UnityEngine;
using System.Collections;

public class UIHeroSlotController : MonoBehaviour {


    public UISprite quality;
    public UITexture icon;
    public UILabel heroName;
    public UIProgressBar healthBar;
    public GameObject effectRoot;
    public GameObject uiDie;
    public UIHeroStarManager starManager;
    public UILabel lblLevel;

    public Controller controller { get; set; }
    public bool isSlected { get; set; }
    public Vector3 worldPos { get; set; }

    public void setRole(Controller _controller)
    {
        controller = _controller;
        icon.mainTexture = Helper.LoadTextureForMiniHero(controller.roleID);

        int grade = 4;
        if (controller.isConnectedServer())
        {
            grade = controller.role.Base.Grade;
        }
        string gradeName = Helper.GetSpriteNameElement(_controller.role);
        quality.spriteName = gradeName;
        starManager.SetStart(grade);
        heroName.text = controller.roleName;
        lblLevel.text = GameManager.localization.GetText("Global_Lvl") + controller.role.Base.Level;
        controller.uiSlot = this;
        isSlected = false;
    }

    void Update()
    {
        if (controller != null)
        {
            if (controller.actionStat != Controller.ActionStat.Dead)
            {
                healthBar.value = controller.hp / controller.maxhp;
            }
        }
    }

	void Start () {
	
	}

    public void Revive()
    {
        uiDie.SetActive(false);
        healthBar.value = 1;

    }
    public void Died()
    {
        uiDie.SetActive(true);
        healthBar.value = 0;
    }

    void OnClick()
    {
        if(GameplayManager.battleStatus != GameplayManager.BattleStatus.Playing)
        {
            if (GameManager.tutorial.step != TutorialManager.TutorialStep.Control_NPCFinshed 
                || UIBattleManager.Instance.tutStep < UIBattleManager.TutorialStepForBattle.AutoSkill)
                return;
        }

        if (controller == null ) return;
        TouchController touch = TouchController.instance;

        Debug.Log("OnClick");
        if (touch.transformRootStart != null)
            Destroy(touch.transformRootStart.gameObject);

        touch.controllerGetSkill = controller;
        touch.SetTouchController(controller);
        touch.transformRootStart = new GameObject("Root_Start");
        touch.posCircleStart.x = controller.transform.position.x;
        touch.posCircleStart.y = 0.05f;
        touch.posCircleStart.z = controller.transform.position.z;

        if (touch.circleRingStart == null)
        {
            if (touch.circleRingStart != null) Destroy(touch.circleRingStart);
            touch.circleRingStart = Instantiate(touch.circle_Ring_Pref, touch.posCircleStart, Quaternion.identity) as GameObject;
            touch.circleRingStart.transform.parent = controller.transform;
        }
        else
        {
            touch.circleRingStart.transform.parent = controller.transform;
            touch.posCircleStart.x = 0;
            touch.posCircleStart.z = 0;
            touch.circleRingStart.transform.localPosition = touch.posCircleStart;
        }

        touch.selectedSlot = this;
        touch.clickOnImage = true;

        UIBattleManager.Instance.OnSelectedHeroSlot();
    }

    void OnDrop()
    {
        //Debug.Log("Drop");
        TouchController touch = TouchController.instance;
        if (touch.controllerGetSkill == null || controller == null || GameplayManager.battleStatus != GameplayManager.BattleStatus.Playing) return;

        touch.clickOnImage = true;
        if (touch.controllerGetSkill.typeCharacter == Controller.TypeChatacter.Healer)
        {
            touch.controllerGetSkill.target = controller.gameObject;
            touch.controllerGetSkill.actionStat = Controller.ActionStat.Action;
        }

        touch.lineRenderer.enabled = false;
        if (touch.transformRootStart != null)
            Destroy(touch.transformRootStart.gameObject);
        if (touch.transformRootEnd != null)
            Destroy(touch.transformRootEnd.gameObject);
        if (touch.transformLookEnd != null)
            Destroy(touch.transformLookEnd.gameObject);
        if (touch.circleRingEnd != null)
            Destroy(touch.circleRingEnd);
        if (touch.circleTouch != null)
            Destroy(touch.circleTouch);
        if (touch.selectedSlot != null)
            touch.selectedSlot.isSlected = false;
    }

    void OnDragOver()
    {

        TouchController touch = TouchController.instance;
        if (touch.controllerGetSkill == null || controller == null 
            || GameplayManager.battleStatus != GameplayManager.BattleStatus.Playing || controller.gameObject == null
            || touch.transformRootStart == null || touch.transformLookStart == null
            ) return;

        Vector3 pointStart = touch.CirclePoint(touch.transformRootStart.transform,
            touch.transformLookStart.transform,
            touch.controllerGetSkill.transform.position,
            controller.gameObject.transform.position);
        touch.lineRenderer.enabled = true;
        touch.OnMouseOverTarget(pointStart, controller.gameObject);
        //Debug.Log("OnDragOver");
        touch.clickOnImage = true;


    }
    void OnDragOut()
    {
        TouchController.instance.clickOnImage = false;
    }

    void OnDragStart()
    {
        if (controller == null || GameplayManager.battleStatus != GameplayManager.BattleStatus.Playing) return;

        if (isSlected) return;
       
       
        TouchController touch = TouchController.instance;
       
        touch.SetTouchController(controller);
        touch.OnMouseDown(controller);
        touch.selectedSlot = this;
        touch.clickOnImage = false;
        isSlected = true;
    }
}
