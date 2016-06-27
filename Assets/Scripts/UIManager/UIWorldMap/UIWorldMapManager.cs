using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;
using System;

public class UIWorldMapManager : MonoBehaviour
{

    [System.Serializable]
    public struct UITutorial
    {
        public UISprite disableScene;
        public UIButton btnMarket;
        public UIButton btnAltar;
        public UIButton btnBlacksmith;
        public UIButton btnSocial;
        public UIButton btnArena;
        public UIButton btnMission;

        public UIButton btnMarket_Hero;
        public UIButton btnMarket_Shop;
        public UIButton btnMarket_Inventory;
        public UIButton btnBlacksmith_UpItem;
        public UIButton btnBlacksmith_UpMaterial;
        public UIButton btnAltar_Hero;
        public UIButton btnAltar_UpStar;
        public UIButton btnAltar_UpLevel;
        public UIButton btnSocial_Mail;
        public UIButton btnSocial_Friend;
        public UIButton btnMission_Quest;
        public UIButton btnMission_Dungeon;
        public UIButton btnArena_Arena;
        public UIButton btnArena_Pillage;
        public UIButton btnArena_CreateWorld;
        public UIButton btnArena_CreateWorld2;

        public GameObject ArrowBegin;

        public GameObject ArrowToShop;
        public GameObject hand;

        public GameObject mailNew;
        public UILabel lblMailNew;

        public GameObject arrowShop;
        public GameObject arrowShopHero;

        public GameObject arrowHero;
        public GameObject arrowHeroHero;

        public GameObject arrowMission;
        public GameObject arrowMissionMission;

        public UIButton btnInfoAltar;
        public UIButton btnInfoBlackmith;
        public UIButton btnInfoArena;
        public UIButton btnInfoSocial;
        public UIButton btnInfoMission;

        public GameObject infomationRoot;
        public UILabel lblInfo;
    }

    [System.Serializable]
    public struct UIUserInfor
    {
        public UILabel lvlLevel;
        public UILabel lvlNickName;
        public UILabel lvlEXP;
        public UIProgressBar exp;
        public UITexture avatar;
        public GameObject avatarSetRoot;
        public GameObject avatarPrefab;
    }

    [System.Serializable]
    public struct UILogInReward
    {
        public UILabel timeLeft;
        public UISpriteAnimation rewardAnim;

        public UIGrid reviewItemRoot;
        public GameObject reviewItemPrefab;
        public UILabel reviewTimeLeft;
        public UIButton reviewBtnTakeAll;

        public GameObject dailyRewardRoot;
        public UILabel lblLogInRewardHeader;
        public UILabel lblLogInRewardDes;
        public UIGrid dailyItemRoot;
        public GameObject dailyItemPrefab;

        public AnimationCurve animCurve;

    }

    public UIChatWindow uiChatWindow;
    public UIScrollView scrollView;
    public UIScrollBar scrollBar;
    public static float scrollBarValue = 0.0f;

    public GameObject[] SubButtonRoots;

    public UITutorial uiTutorial;
    public UIUserInfor uiUserInfo;
    public UILogInReward uiLogInReward;

    public GameObject lockArena;
    public GameObject lockPillage;
    public GameObject lockDungeon;
    public GameObject lockCreateWorld;
    public GameObject intro;

    public UIHireHeroManager hireHero;
    public UIPacketManager packetManager;

    private WorldMapController _controller;

    private float _onlineTime;
    private List<GameObject> _itemReview;

    private bool _init = false;

    void Start()
    {

        UIEmpireManager.isFriend = false;

        SoundManager.Instance.PlayAmbient("SoundMission");

        
        //view intro when has just login
        if (!GameManager.viewedIntro)
            intro.SetActive(true);
        else
            Init();

     

    }
    void OnDestroy()
    {
       // SoundManager.Instance.PlayAmbient("SoundMission");
    }
    public void Init()
    {
        _controller = new WorldMapController(this);

        //load maillist from server
        _controller.SendRequestMailList();
        _onlineTime = GameManager.GameUser.Base.CurrentAwardTime;
        _itemReview = new List<GameObject>();

        //load online reward from server
        _controller.SendRequestOnlineReward();

        //load language text for GUI
        Localization();

        //check tutorial , if unfinished then continue
        Tutorial();

        ChangeWorldMapOffset();

        //Load avatar Gui , nick , level..of user.
        LoadUserInfo();

        uiLogInReward.reviewBtnTakeAll.gameObject.SetActive(false);

        //Load online award , daily award
        InitReward();

        GameManager.dungeonCurEvent = 0;
        GameManager.dungeonEventCount = 0;
        LockButton();
        _init = true;
    }

    void Update()
    {
        if (!_init) return;

        //MyInput.CheckInput();

        //calculate time for new online award

        if (GameManager.GameUser.Base.OnlineAwardStep >= 0 && GameManager.GameUser.Base.OnlineAwardStep < GameConfig.ONLINE_AWARDS.Length - 1)
        {
            UpdateTimeReward();
        }
        else
        {
            if (uiLogInReward.timeLeft.transform.parent.gameObject.activeInHierarchy)
                uiLogInReward.timeLeft.transform.parent.gameObject.SetActive(false);
        }
    }

    #region Button Click
    public void OnButtonMarket_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            uiTutorial.arrowShop.SetActive(false);
            if (GameManager.tutorial.step >= TutorialManager.TutorialStep.BuyHero_Exit)
            {
                return;
            }
        }


        HideAllSubButton();
        //uiTutorial.btnMarket.transform.FindChild("Label").gameObject.SetActive(false);
        UIPlayTween tween = uiTutorial.btnMarket.GetComponent<UIPlayTween>();

        tween.tweenTarget = uiTutorial.btnMarket_Hero.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnMarket_Shop.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnMarket_Inventory.gameObject;
        tween.Play(true);

    }
    public void OnButtonBlacksmith_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;

        HideAllSubButton();

        UIPlayTween tween = uiTutorial.btnBlacksmith.GetComponent<UIPlayTween>();
        tween.tweenTarget = uiTutorial.btnBlacksmith_UpItem.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnBlacksmith_UpMaterial.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnInfoBlackmith.gameObject;
        tween.Play(true);

    }
    public void OnButtonAltar_Click()
    {

        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            uiTutorial.arrowHero.SetActive(false);
            if (GameManager.tutorial.step < TutorialManager.TutorialStep.BuyHero_Exit)
                return;

            if (GameManager.tutorial.step >= TutorialManager.TutorialStep.Control_NPCFinshed && GameManager.tutorial.step < TutorialManager.TutorialStep.Equip_Begin )
                return;
        }

        HideAllSubButton();

        UIPlayTween tween = uiTutorial.btnAltar.GetComponent<UIPlayTween>();
        tween.tweenTarget = uiTutorial.btnAltar_Hero.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnAltar_UpLevel.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnAltar_UpStar.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnInfoAltar.gameObject;
        tween.Play(true);

    }
    public void OnButtonSocial_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            return;
        }

        HideAllSubButton();

        UIPlayTween tween = uiTutorial.btnSocial.GetComponent<UIPlayTween>();
        tween.tweenTarget = uiTutorial.btnSocial_Friend.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnSocial_Mail.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnInfoSocial.gameObject;
        tween.Play(true);
    }
    public void OnButtonArena_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            return;
        }

        HideAllSubButton();

        UIPlayTween tween = uiTutorial.btnArena.GetComponent<UIPlayTween>();
        tween.tweenTarget = uiTutorial.btnArena_Arena.gameObject;
        tween.Play(true);       
        
        tween.tweenTarget = uiTutorial.btnInfoArena.gameObject;
        tween.Play(true);
    }
    public void OnButtonMission_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            uiTutorial.arrowMission.SetActive(false);
            if (GameManager.tutorial.step != TutorialManager.TutorialStep.Control_NPCFinshed)
                return;
        }


        HideAllSubButton();

        UIPlayTween tween = uiTutorial.btnMission.GetComponent<UIPlayTween>();
        
        tween.tweenTarget = uiTutorial.btnMission_Quest.gameObject;
        tween.Play(true);
        tween.tweenTarget = uiTutorial.btnInfoMission.gameObject;
        tween.Play(true);
    }
    public void OnButtonMarket_Hero_Click()
    {
      
        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.ShopHero;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.BuyHero);
    }
    public void OnButtonMarket_Shop_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {           
            return;
        }

        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Shop;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.ChargeShop);
    }
    public void OnButtonMarket_Inventory_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            return;
        }
        
        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Storage;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.Hero);
    }
    public void OnButtonBlacksmith_UpItem_Click()
    {
       
        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Blacksmith;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.ItemUpgrade);
    }
    public void OnButtonBlacksmith_UpMaterial_Click()
    {       
        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Lab;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.ItemUpgrade);
    }
    public void OnButtonAltar_Hero_Click()
    {
       
        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Hero;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.Hero);
    }
    public void OnButtonAltar_UpStar_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;

        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.UpStar;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.HeroUpStar);
    }
    public void OnButtonAltar_UpLevel_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;

        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.UpLevel;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.HeroUpgrade);
    }
    public void OnButtonSocial_Friend_Click()
    {
        
        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Social;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.Social);
        Debug.Log("OnButtonSocial_Click");
    }
    public void OnButtonSocial_Mail_Click()
    {
        /*  if (GameManager.tutorial.step <= TutorialManager.TutorialStep.WorldMap_ClickShop)
              return;

          scrollBarValue = scrollBar.value;
          GameManager.Status = GameStatus.Social;
          GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.Social);
          Debug.Log("OnButtonSocial_Click");*/
    }
    public void OnButtonMission_Quest_Click()
    {
        
        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Quest;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.PVEMap);
        Debug.Log("OnButtonSocial_Click");
    }
    public void OnButtonMission_Dungeon_Click()
    {
       
    }
    public void OnButtonArena_Arena_Click()
    {
       
        int levelArena = GameConfig.ARENALEVEL;
        if (GameManager.GameUser.Base.Level < levelArena)
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Arena_NotLevel"), levelArena), UINoticeManager.NoticeType.Message);
            return;
        }


        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Arena;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.Arena);
        Debug.Log("OnButtonSocial_Click");
    }
    public void OnButtonArena_Pillage_Click()
    {
      
        int levelPillage = GameConfig.PILLAGELEVEL;
        if (GameManager.GameUser.Base.Level < levelPillage)
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Pillage_NotLevel"), levelPillage), UINoticeManager.NoticeType.Message);
            return;
        }

        scrollBarValue = scrollBar.value;
        GameManager.Status = GameStatus.Pillage;
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.Pillage);
        Debug.Log("OnButtonSocial_Click");
    }
    public void OnButtonArena_CreateWorld_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            return;
        }
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.Empire);     
    }
    public void OnBackButton_Click()
    {       
        scrollBarValue = 0.5f;
        GameManager.Status = GameStatus.Start;
        HandleExitWorldMap();
    }
    public void OnButtonExitMarket_Click()
    {
        uiTutorial.btnMarket.transform.FindChild("Label").gameObject.SetActive(true);
    }
    public void OnButtonExitBlacksmith_Click()
    {
        uiTutorial.btnBlacksmith.transform.FindChild("Label").gameObject.SetActive(true);
    }
    public void OnButtonExitAltar_Click()
    {
        uiTutorial.btnAltar.transform.FindChild("Label").gameObject.SetActive(true);
    }
    public void OnButtonExitSocial_Click()
    {
        uiTutorial.btnSocial.transform.FindChild("Label").gameObject.SetActive(true);
    }
    public void OnButtonExitArena_Click()
    {
        uiTutorial.btnArena.transform.FindChild("Label").gameObject.SetActive(true);
    }
    public void OnButtonExitMission_Click()
    {
        uiTutorial.btnMission.transform.FindChild("Label").gameObject.SetActive(true);
    }
    public void OnButtonReviewReward_Click()
    {
    }
    public void OnButtonTakeAll_Click()
    {
        _controller.SendRequestOnlineReward(1);
    }
    public void OnButtonInfoAltar_Click()
    {
        uiTutorial.infomationRoot.SetActive(true);
        string info = GameManager.localization.GetText("Info_Altar");
        uiTutorial.lblInfo.text = Helper.StringToMultiLine(info);
    }
    public void OnButtonInfoBlackmith_Click()
    {
        uiTutorial.infomationRoot.SetActive(true);
        uiTutorial.lblInfo.text = Helper.StringToMultiLine(GameManager.localization.GetText("Info_Blacksmith"));
    }
    public void OnButtonInfoArena_Click()
    {
        uiTutorial.infomationRoot.SetActive(true);
        uiTutorial.lblInfo.text = Helper.StringToMultiLine(GameManager.localization.GetText("Info_Arena"));
    }
    public void OnButtonInfoSocial_Click()
    {
        uiTutorial.infomationRoot.SetActive(true);
        uiTutorial.lblInfo.text = Helper.StringToMultiLine(GameManager.localization.GetText("Info_Social"));
    }
    public void OnButtonInfoMission_Click()
    {
        uiTutorial.infomationRoot.SetActive(true);
        uiTutorial.lblInfo.text = Helper.StringToMultiLine(GameManager.localization.GetText("Info_Mission"));
    }
    public void OnButtonHideInfomation_Click()
    {
        uiTutorial.infomationRoot.SetActive(false);
    }
    #endregion

    #region public methods
    public void SendChat(string message)
    {
        _controller.SendChat(0, message);
    }
    public void SendBuyPackage(int index)
    {
        int targetID = 999 + index;
        _controller.SendBuyPackage(targetID);
    }
   
    public void OnSelectAvatar(int ID)
    {
        GameManager.GameUser.Base.Avatar = ID;
        uiUserInfo.avatar.mainTexture = Helper.LoadTextureForAvatar(ID);
        _controller.SendChangeAvatar(ID);
    }
    public void OnResponseCheckMail(UserMail[] userMail)
    {
        int totalNewMail = userMail.Length;
        //int totalOldMail = GameManager.GameUser.UserMails.Count;

        //int newMail = totalNewMail - totalOldMail;

        if (totalNewMail > 0)
        {
            uiTutorial.mailNew.SetActive(true);
            uiTutorial.lblMailNew.text = totalNewMail.ToString();
        }
        else
        {
            uiTutorial.mailNew.SetActive(false);
        }

    }
    public void OnResponseOnlineReward(GameAward gameAward)
    {
        _onlineTime = GameManager.GameUser.Base.CurrentAwardTime;
        if (gameAward != null)
        {
            InitOnlineRewardReview();
            DisableAnimReward();
            uiLogInReward.reviewBtnTakeAll.gameObject.SetActive(false);
        }
    }
    public void OnResponseHeroHire(UserRoleHire[] roles)
    {
        hireHero.OnStartClick += OnHireAccept;
        hireHero.Show(roles);
    }
    public void OnResponseBuyPackage(int itemNewCount,int heroNewCount)
    {
        packetManager.OnResponseBuyPackage(itemNewCount, heroNewCount);
    }
    #endregion

    #region private methods
    private void InitReward()
    {
        if (GameManager.GameUser.Base.OnlineAwardStep >= 0 && GameManager.GameUser.Base.OnlineAwardStep < GameConfig.ONLINE_AWARDS.Length - 1)
        {
            InitOnlineRewardReview();
        }
        else
        {
            uiLogInReward.timeLeft.transform.parent.gameObject.SetActive(false);
        }

        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Begin || 
            (GameManager.GameUser.DailyLoginCount > 0 && !GameManager.GameUser.DailyLoginAward.IsEmpty && !GameManager.takeLoginReward))
        {
            StartCoroutine(ShowDailyReward());
            GameManager.takeLoginReward = true;
        }
        else
        {
            uiLogInReward.dailyRewardRoot.SetActive(false);
            InitDailyReward();
        }
    }

    //Check to hire another hero in dungeon , and start Dungeon
    private void OnHireAccept()
    {
        scrollBarValue = scrollBar.value;

        int roleIDHire = 0;
        if (hireHero.selectedSlot != null)
        {
            //if clicked Accept checkbox , then agree to hire
            if (hireHero.uiHireHero.togAcceptToHire.value)
            {
                roleIDHire = hireHero.selectedSlot.userRoleHire.Id;
            }

        }

        _controller.SendDungeonMode(roleIDHire);
    }
    private void HandleExitWorldMap()
    {
        UINoticeManager.OnButtonOK_click += ExitWorldMap;
        MessageBox.ShowDialog(GameManager.localization.GetText("MainMenu_QuitGame"), UINoticeManager.NoticeType.YesNo);
    }
    private void ExitWorldMap()
    {
        _controller.Disconnect();
        GameScenes.ChangeScense(GameScenes.MyScene.WorldMap, GameScenes.MyScene.MainMenu);
    }

    //lock funtions if user not eligible to join 
    private void LockButton()
    {
        int levelDungeon = GameConfig.DUNGEONLEVEL;
        if (GameManager.GameUser.Base.Level < levelDungeon)
        {
            lockDungeon.SetActive(true);
        }

        int levelArena = GameConfig.ARENALEVEL;
        if (GameManager.GameUser.Base.Level < levelArena)
        {
            lockArena.SetActive(true);            
        }

        int levelPillage = GameConfig.PILLAGELEVEL;
        if (GameManager.GameUser.Base.Level < levelPillage)
        {
            lockPillage.SetActive(true);            
        }

        //lockCreateWorld.SetActive(true);
    }
    private IEnumerator ShowDailyReward()
    {
        yield return new WaitForSeconds(0.5f);
        uiLogInReward.dailyRewardRoot.SetActive(true);
        InitDailyReward();
    }
    private void UpdateTimeReward()
    {
        _onlineTime += Time.deltaTime;
        int timeLeft = (GameConfig.ONLINE_STEPS[GameManager.GameUser.Base.OnlineAwardStep + 1] + 10) - (int)_onlineTime;

        //expiration of the waiting period, and the rewards will be received
        if (timeLeft <= 0)
        {
            timeLeft = 0;
            uiLogInReward.rewardAnim.enabled = true;
            uiLogInReward.reviewBtnTakeAll.gameObject.SetActive(true);
        }

        string s = Helper.FloatToTime(timeLeft);
        uiLogInReward.timeLeft.text = s;
        uiLogInReward.reviewTimeLeft.text = s;

    }
    private void DisableAnimReward()
    {
        uiLogInReward.rewardAnim.enabled = false;
    }
    private void InitDailyReward()
    {
        uiLogInReward.lblLogInRewardHeader.text = GameManager.localization.GetText("LogInReward_Header");
        uiLogInReward.lblLogInRewardDes.text = Helper.StringToMultiLine(GameManager.localization.GetText("LoginReward_Desc"));

        for (int i = 1; i < GameConfig.DAILY_AWARDS.Length; i++)
        {
            GameObject go = NGUITools.AddChild(uiLogInReward.dailyItemRoot.gameObject, uiLogInReward.dailyItemPrefab);
            go.SetActive(true);

            List<GameAward> gameAwards = GameConfig.DAILY_AWARDS[i];
            GameAward gameAward = gameAwards[GameManager.GameUser.Base.Level];

            foreach (var item in gameAward.Items)
            {
                Texture2D icon = Helper.LoadTextureForSupportItem(item.Key);
                if (icon == null)
                    icon = Helper.LoadTextureForEquipItem(item.Key);
                go.transform.FindChild("icon").GetComponent<UITexture>().mainTexture = icon;
                go.transform.FindChild("amount").GetComponent<UILabel>().text = "x" + item.Value;

            }

            if (gameAward.Silver > 0)
            {
                Texture2D icon = Helper.LoadTextureSilver();
                go.transform.FindChild("icon").GetComponent<UITexture>().mainTexture = icon;
                go.transform.FindChild("amount").GetComponent<UILabel>().text = "x" + gameAward.Silver;
                go.SetActive(true);
            }

            if (gameAward.Gold > 0)
            {
                Texture2D icon = Helper.LoadTextureGold();
                go.transform.FindChild("icon").GetComponent<UITexture>().mainTexture = icon;
                go.transform.FindChild("amount").GetComponent<UILabel>().text = "x" + gameAward.Gold;
                go.SetActive(true);
            }

            go.transform.FindChild("day").GetComponent<UILabel>().text = string.Format(GameManager.localization.GetText("LoginReward_Day"), i);

            GameObject check = go.transform.FindChild("check").gameObject;
            if (GameManager.GameUser.DailyLoginCount < i)
            {
                check.SetActive(false);
                go.transform.FindChild("border").gameObject.SetActive(false);
            }
            else
            {
                check.SetActive(true);
                go.transform.FindChild("border").gameObject.SetActive(true);
            }


            if (GameManager.GameUser.DailyLoginCount == i && check.activeInHierarchy)
            {
                Hashtable hash = new Hashtable();
                hash["scale"] = new Vector3(4, 4, 4);
                hash["time"] = 0.5;
                hash["delay"] = 1;
                hash["easetype"] = iTween.EaseType.easeInOutExpo;
                iTween.ScaleFrom(check, hash);

                Helper.FadeIn(check.GetComponent<UISprite>(), 1.2f, uiLogInReward.animCurve, 0, FadeInConplete);
            }

        }
        uiLogInReward.dailyItemRoot.Reposition();
    }
    private void FadeInConplete()
    {
     //   Debug.Log("FadeInConplete");
    }
    private void InitOnlineRewardReview()
    {
        foreach (GameObject go in _itemReview)
            NGUITools.Destroy(go);

        _itemReview.Clear();

        if (GameManager.GameUser.Base.OnlineAwardStep < 0 || GameManager.GameUser.Base.OnlineAwardStep >= GameConfig.ONLINE_AWARDS.Length - 1) return;

        GameAward gameAward = GameConfig.ONLINE_AWARDS[GameManager.GameUser.Base.OnlineAwardStep + 1][GameManager.GameUser.Base.Level];
        if (gameAward != null)
        {
            foreach (var item in gameAward.Items)
            {
                GameObject go = NGUITools.AddChild(uiLogInReward.reviewItemRoot.gameObject, uiLogInReward.reviewItemPrefab);
                Texture2D icon = Helper.LoadTextureForSupportItem(item.Key);
                if (icon == null)
                    icon = Helper.LoadTextureForEquipItem(item.Key);

                go.transform.FindChild("icon").GetComponent<UITexture>().mainTexture = icon;
                go.transform.FindChild("amount").GetComponent<UILabel>().text = "x" + item.Value;
                go.transform.FindChild("name").GetComponent<UILabel>().text = GameManager.localization.getItem(item.Key).Name;
                go.transform.FindChild("check").gameObject.SetActive(false);
                go.SetActive(true);
                _itemReview.Add(go);
            }
        }

        if (gameAward.Silver > 0)
        {
            GameObject go = NGUITools.AddChild(uiLogInReward.reviewItemRoot.gameObject, uiLogInReward.reviewItemPrefab);
            Texture2D icon = Helper.LoadTextureSilver();
            go.transform.FindChild("icon").GetComponent<UITexture>().mainTexture = icon;
            go.transform.FindChild("amount").GetComponent<UILabel>().text = "x" + gameAward.Silver;
            go.transform.FindChild("name").GetComponent<UILabel>().text = "";
            go.transform.FindChild("check").gameObject.SetActive(false);
            go.SetActive(true);
            _itemReview.Add(go);
        }

        if (gameAward.Gold > 0)
        {
            GameObject go = NGUITools.AddChild(uiLogInReward.reviewItemRoot.gameObject, uiLogInReward.reviewItemPrefab);
            Texture2D icon = Helper.LoadTextureGold();
            go.transform.FindChild("icon").GetComponent<UITexture>().mainTexture = icon;
            go.transform.FindChild("amount").GetComponent<UILabel>().text = "x" + gameAward.Gold;
            go.transform.FindChild("name").GetComponent<UILabel>().text = "";
            go.transform.FindChild("check").gameObject.SetActive(false);
            go.SetActive(true);
            _itemReview.Add(go);
        }

        uiLogInReward.reviewItemRoot.Reposition();

    }
    private void HideAllSubButton()
    {
        UIPlayTween playTween = new UIPlayTween();
        playTween.playDirection = AnimationOrTween.Direction.Reverse;
        playTween.ifDisabledOnPlay = AnimationOrTween.EnableCondition.DoNothing;
        playTween.disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;
        foreach (GameObject go in SubButtonRoots)
        {
            if (go.activeInHierarchy)
            {
                playTween.tweenTarget = go;
                playTween.Play(true);
            }
        }
    }
    private void ChangeWorldMapOffset()
    {
        scrollBar.value = scrollBarValue;
        scrollView.UpdatePosition();
    }

    //load text for language selected
    private void Localization()
    {
        uiTutorial.btnMarket.transform.FindChild("Root").FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Market");
        uiTutorial.btnAltar.transform.FindChild("Root").FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Altar");
        uiTutorial.btnBlacksmith.transform.FindChild("Root").FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Blacksmith");
        uiTutorial.btnSocial.transform.FindChild("Root").FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Social");
        uiTutorial.btnArena.transform.FindChild("Root").FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Arena");
        uiTutorial.btnMission.transform.FindChild("Root").FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Mission");

        uiTutorial.btnMarket_Hero.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Market_Hero").ToUpper();
        uiTutorial.btnMarket_Shop.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Market_Shop");
        uiTutorial.btnMarket_Inventory.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Market_Inventory");
        uiTutorial.btnBlacksmith_UpItem.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Blacksmith_UpgradeItem");
        uiTutorial.btnBlacksmith_UpMaterial.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Blacksmith_UpgradeMaterial");
        uiTutorial.btnAltar_Hero.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Altar_Hero");
        uiTutorial.btnAltar_UpStar.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Altar_UpStar");
        uiTutorial.btnAltar_UpLevel.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Altar_UpLevel");
        uiTutorial.btnSocial_Mail.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Social_Mail");
        uiTutorial.btnSocial_Friend.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Social_Friend");
        uiTutorial.btnMission_Quest.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Mission_Quest");
       
        uiTutorial.btnArena_Arena.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Arena_Arena");
        
        
        uiTutorial.btnArena_CreateWorld2.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("WorldMap_Arena_CreateWorld");

        uiTutorial.btnInfoAltar.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("Info_Button");
        uiTutorial.btnInfoArena.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("Info_Button");
        uiTutorial.btnInfoBlackmith.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("Info_Button");
        uiTutorial.btnInfoMission.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("Info_Button");
        uiTutorial.btnInfoSocial.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("Info_Button");
      
        uiLogInReward.reviewBtnTakeAll.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("LoginReward_BtnTakeGitf");
    }

    //Load avatar , nick , level GUI bar
    private void LoadUserInfo()
    {
        uiUserInfo.lvlNickName.text = GameManager.GameUser.Base.NickName;
        uiUserInfo.lvlLevel.text = GameManager.GameUser.Base.Level.ToString("D2");

        float exp = (float)GameManager.GameUser.Base.Exp / (float)UserConfig.LEVELS_EXP[GameManager.GameUser.Base.Level];

        uiUserInfo.lvlEXP.text = string.Format("{0:0}%", exp * 100.0f);
        uiUserInfo.exp.value = exp;

        uiUserInfo.avatar.mainTexture = Helper.LoadTextureForAvatar(GameManager.GameUser.Base.Avatar);

        InitAvatarSet();
    }
    private void InitAvatarSet()
    {
        for (int i = 0; i <= 50; i++)
        {
            GameObject go = NGUITools.AddChild(uiUserInfo.avatarSetRoot, uiUserInfo.avatarPrefab);
            UIAvatarController avatar = go.GetComponent<UIAvatarController>();
            avatar.setAvatar(i, this);
            go.name = "Avatar_" + i.ToString("00");
        }
        uiUserInfo.avatarSetRoot.GetComponent<UIGrid>().Reposition();
    }  
    #endregion

    #region Tutorial
    //Learn how to play
    private void Tutorial()
    {
        //Step : tutorial
        //1-Summon Hero
        //2-Select heroes to  join battle (require enough 3 hero)  
        //3- Select Mission and Start . Learn how to move, select the target, select skill.
        //4-learn how to equip items.

        if (GameManager.tutorial.step >= TutorialManager.TutorialStep.Finished)
        {
            return;
        }
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Begin)
        {
            uiTutorial.ArrowBegin.SetActive(true);
        }
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.SummonHeroes_NPC)
        {
            GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("Tut_Summon"));
            UINPCTutorialManager.OnFinished += OnFinishedShowNPC;
        }
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.SummonHeroes_BuyHero)
        {
            if (GameManager.GameUser.UserRoles.Count >= 3)
            {               
                GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.BuyHero_Exit);
                Tutorial();
            }
            else
            {
                uiTutorial.arrowShop.SetActive(true);
                uiTutorial.arrowShopHero.SetActive(true);
            }
        }
        if (GameManager.tutorial.step >= TutorialManager.TutorialStep.BuyHero_Exit && GameManager.tutorial.step < TutorialManager.TutorialStep.Party_NPCFinished)
        {
            uiTutorial.arrowHero.SetActive(true);
            uiTutorial.arrowHeroHero.SetActive(true);
        }
        
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Party_NPCFinished)
        {
            if (GameManager.GameUser.UserRoles.Count(p => p.Base.Status == RoleStatus.Active) < 3)
            {
                GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.BuyHero_Exit);
                Tutorial();
                return;
            }
            GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("Tut_PartyFinished"));
            UINPCTutorialManager.OnFinished += OnFinishedShowMission;           
        }

        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            if (GameManager.GameUser.UserRoles.Count(p => p.Base.Status == RoleStatus.Active) < 3)
            {
                GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.BuyHero_Exit);
                Tutorial();
                return;
            }
            uiTutorial.arrowMission.SetActive(true);
            uiTutorial.arrowMissionMission.SetActive(true);
        }

        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Begin)
        {
            //show MC guide
            GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("Tut_EquipBegin"));
            UINPCTutorialManager.OnFinished += OnFinishedShowMCEquip;           
        }
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Arrow)
        {
            uiTutorial.arrowHero.SetActive(true);
            uiTutorial.arrowHeroHero.SetActive(true);
        }
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Equip_Finished)
        {
            //show MC guide
            GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("Tut_FinishTutorial"));
            GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.Finished);
        }
    }
    public void OnFinishedCloseReward()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Begin)
        {
            GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.SummonHeroes_NPC);
            Tutorial();
        }       
    }
    //Show MC say :
    public void OnFinishedShowMCEquip()
    {
        GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.Equip_Arrow);
        uiTutorial.arrowHero.SetActive(true);
        uiTutorial.arrowHeroHero.SetActive(true);
    }
    public void OnFinishedShowNPC()
    {
        GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.SummonHeroes_BuyHero);
        uiTutorial.arrowShop.SetActive(true);
        uiTutorial.arrowShopHero.SetActive(true);
    }
    public void OnFinishedShowMission()
    {
        GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.Control_NPCFinshed);
        uiTutorial.arrowMission.SetActive(true);
        uiTutorial.arrowMissionMission.SetActive(true);
    }
    #endregion

}