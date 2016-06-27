using UnityEngine;
using System.Collections;
using System.Linq;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;
using System.Collections.Generic;

public class UIPVEMapManager : MonoBehaviour {
      

    [System.Serializable]
    public struct Mission
    {
        public GameObject lockIcon;
        public GameObject unLockIcon;

        public void SetLock(bool b)
        {
            lockIcon.SetActive(b);
            unLockIcon.SetActive(!b);
        }
    }

    [System.Serializable]
    public struct MissionInfo
    {
        public UILabel lblMapName;
        public UILabel lblStandard;
        public UIPopupList cboStandard;
        public UILabel LButtonFight;
        public UILabel LRewards;
        public UILabel lblRoleExp;
        public UILabel lblUserExp;
        public UILabel lblSilver;
        public UILabel lblItemAmount;
        public UILabel LPossibleLoot;
        public UILabel lblEnergy;

        public UITexture item;
        public UITexture Border;

        public UITexture[] loots;


        public GameObject arrowAttack;
    }

    public static int curMap;
    public static int curStandard;

    public GameObject lockPrefab;
    public GameObject unLockPrefab;
    public GameObject[] missionRoot;
    public GameObject curWaveRoot;
    public MissionInfo missionInfo;

    public UIHireHeroManager hireHero;

    private int curMission = 3;

    private UIPlayTween _tweenPlay;
    private int _mapLevelSelected;
    private PveMapController _controller;
    private int _standard;//difficulty level

    void Start()
    {
        Tutorial();
        curMission = GameManager.GameUser.Base.MissionLevel;
        _tweenPlay = GetComponent<UIPlayTween>();
        _mapLevelSelected = 0;
        _controller = new PveMapController(this);
        _standard = 0;
        InitMissions();
        Localization();
    }


    #region Mission

    //back to worldmap
    public void OnButtonBack_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.PVEMap, GameScenes.MyScene.WorldMap);
    }

    //show info of Mission 1
    public void OnButtonMap1_Click()
    {
        ShowIformationMap(1);
    }
    public void OnButtonMap2_Click()
    {
        ShowIformationMap(2);
    }
    public void OnButtonMap3_Click()
    {
        ShowIformationMap(3);
    }
    public void OnButtonMap4_Click()
    {
        ShowIformationMap(4);
    }
    public void OnButtonMap5_Click()
    {
        ShowIformationMap(5);
    }
    public void OnButtonMap6_Click()
    {
        ShowIformationMap(6);
    }
    public void OnButtonMap7_Click()
    {
        ShowIformationMap(7);
    }
    public void OnButtonMap8_Click()
    {
        ShowIformationMap(8);
    }
    public void OnButtonMap9_Click()
    {
        ShowIformationMap(9);
    }
    public void OnButtonMap10_Click()
    {
        ShowIformationMap(10);
    }
    public void OnButtonMap11_Click()
    {
        ShowIformationMap(11);
    }
    public void OnButtonMap12_Click()
    {
        ShowIformationMap(12);
    }
    public void OnButtonMap13_Click()
    {
        ShowIformationMap(13);
    }
    public void OnButtonMap14_Click()
    {
        ShowIformationMap(14);
    }
    public void OnButtonMap15_Click()
    {
        ShowIformationMap(15);
    }
    public void OnButtonMap16_Click()
    {
        ShowIformationMap(16);
    }
    public void OnButtonMap17_Click()
    {
        ShowIformationMap(17);
    }
    public void OnButtonMap18_Click()
    {
        ShowIformationMap(18);
    }
    public void OnButtonMap19_Click()
    {
        ShowIformationMap(19);
    }
    public void OnButtonMap20_Click()
    {
        ShowIformationMap(20);
    }
    #endregion

    #region Button
  
    public void OnButtonCloseInformation_Click()
    {
    }

    //Start mission 
    public void OnButtonFight_Click()
    {        
        if (!GameManager.CheckHeroToBattle()) return;

        MissionData data = MissionConfig.MISSIONS[_mapLevelSelected];

        float percent = 1;
        if (_standard == 1)
            percent = 1.2f;
        if (_standard == 2)
            percent = 1.5f;

        int engery = (int)(data.Energy * percent);

        if (GameManager.GameUser.UserRoles.Count(p => p.Base.Status == RoleStatus.Active && p.Base.Energy >= engery) < 3)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_NotEnoghtEnery"), UINoticeManager.NoticeType.Message);
            return;
        }


        Debug.Log("GameManager.tutorial.step " + GameManager.tutorial.step);

        //check if is tutorial how to control ,not show  hirehero list and Start mission
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            Debug.Log("OnHireAccept ");

            //require start mission
            OnHireAccept();
        }
        else//require HireHero list
        {
            Debug.Log("SendRequestHire");
            _controller.SendRequestHire();
        }
    }

    //reciever HeroHire list from server , show list
    public void OnResponseHeroHire(UserRoleHire[] roles)
    {
        //set OnHireAccept for OnStartClick event
        hireHero.OnStartClick += OnHireAccept;

        hireHero.Show(roles);
    }

    //if selected difficult level (easy, normal , hard), refresh map info (each difficulty level is different rewards)
    public void OnCboStandard_Selected()
    {
        //difficulty level
        _standard = missionInfo.cboStandard.items.IndexOf(missionInfo.cboStandard.value);

        missionInfo.Border.color = Helper.ItemColor[_standard + 1];

        ShowIformationMap(_mapLevelSelected);
    }
    #endregion

    #region private methods

    //require start mission
    private void OnHireAccept()
    {
        int roleIDHire = 0;
        if (hireHero.selectedSlot != null)
        {
            if (hireHero.uiHireHero.togAcceptToHire.value)
            {
                roleIDHire = hireHero.selectedSlot.userRoleHire.Id;
            }

        }

        GameManager.curMission = _mapLevelSelected;
        curMap = _mapLevelSelected;
        curStandard = missionInfo.cboStandard.items.IndexOf(missionInfo.cboStandard.value);

        _controller.SendRequestBattle(_mapLevelSelected, roleIDHire, curStandard);
    }

    //load text for Gui with selected language
    private void Localization()
    {
        missionInfo.LRewards.text = GameManager.localization.GetText("PVE_Rewards");
        missionInfo.lblStandard.text = GameManager.localization.GetText("PVE_Mission_Standard");
        missionInfo.LPossibleLoot.text = GameManager.localization.GetText("PVE_Mission_PossibleLoot");
        missionInfo.LButtonFight.text = GameManager.localization.GetText("PVE_Mission_Fight");

        missionInfo.cboStandard.Clear();
        missionInfo.cboStandard.AddItem(GameManager.localization.GetText("PVE_Mission_Standard_Easy"));
        missionInfo.cboStandard.AddItem(GameManager.localization.GetText("PVE_Mission_Standard_Normal"));
        missionInfo.cboStandard.AddItem(GameManager.localization.GetText("PVE_Mission_Standard_Hard"));

    }


   // show mission info by id 
    private void ShowIformationMap(int level)
    {
        if (level > curMission)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("PVE_NotLevel"), UINoticeManager.NoticeType.Message);
            return;
        }
        _tweenPlay.Play(true);
        _mapLevelSelected = level;

        missionInfo.lblMapName.text = string.Format(GameManager.localization.GetText("PVE_Misson_Name"), level);

        MissionData data = MissionConfig.MISSIONS[level];

        float percent = 1;
        if (_standard == 1)
            percent = 1.2f;
        if (_standard == 2)
            percent = 1.5f;

        missionInfo.lblEnergy.text = string.Format(GameManager.localization.GetText("PVE_Mission_Energy"), (int)(data.Energy * percent));
        missionInfo.lblRoleExp.text = string.Format(GameManager.localization.GetText("PVE_Mission_EXP"), (int)(data.RoleExp * percent));
        missionInfo.lblUserExp.text = string.Format(GameManager.localization.GetText("PVE_Mission_EXP"), (int)(data.UserExp * percent));
        missionInfo.lblSilver.text = "+" + (data.Silver * percent);
        missionInfo.lblItemAmount.text = "x1";

        missionInfo.Border.color = Helper.ItemColor[_standard + 1];

        if (data.EquipItems.Length > 0)
        {

            int itemId = data.EquipItems[Random.Range(0, data.EquipItems.Length)];
            Texture2D icon = Helper.LoadTextureForEquipItem(itemId);
            if (icon == null)
                icon = Helper.LoadTextureForSupportItem(itemId);
            missionInfo.item.mainTexture = icon;
        }

        if (level < MissionConfig.DROP_ITEMS.Count)
        {
            List<int> itemsDrop = MissionConfig.DROP_ITEMS[level].Where(p => p.Id != 0).Select(p => p.Id).ToList();

            int lootIndex = 0;
            while (lootIndex < 4 && itemsDrop.Count > 0)
            {
                int index = Random.Range(0, itemsDrop.Count);
                int itemID = itemsDrop[index];
                Texture2D icon = Helper.LoadTextureForEquipItem(itemID);
                if (icon == null)
                    icon = Helper.LoadTextureForSupportItem(itemID);
                if (icon == null)
                {
                    Debug.Log("Null " + itemID);
                }
                missionInfo.loots[lootIndex].mainTexture = icon;
                itemsDrop.RemoveAt(index);
                lootIndex++;
            }


        }

    }

    private void InitMissions()
    {
        for (int i = 0; i < missionRoot.Length; i++)
        {
            UISprite spriteMission = missionRoot[i].transform.FindChild("Texture").GetComponent<UISprite>();
            if (i < curMission)
            {
                spriteMission.spriteName = "diem_on";
            }
            if (i == curMission-1)
            {
                curWaveRoot.transform.parent = missionRoot[i].transform;
                curWaveRoot.transform.localPosition = new Vector3(0, 100, 0);
                curWaveRoot.transform.localScale = new Vector3(1, 1, 1);
            }
            UILabel lbl = missionRoot[i].transform.FindChild("Label").GetComponent<UILabel>();
            lbl.text = "[00FFFF]" + (i + 1).ToString() + "[-]";
            Vector3 posLabel = new Vector3(0, 40, 0);

            if (i == 4)
            {
                posLabel = new Vector3(0,100, 0);
            }
            if (i == 9)
            {
                posLabel = new Vector3(-50, 88, 0);
            }
            if (i == 14)
            {
                posLabel = new Vector3(0, 130, 0);
            }
            if (i == 19)
            {
                posLabel = new Vector3(16, 90, 0);
            }
            lbl.transform.localPosition = posLabel;
          /*  GameObject go = NGUITools.AddChild(missionLockRoot[i], lblMapInfo.gameObject);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localPosition = new Vector3(0, -30, 0);
            UILabel lbl = go.GetComponent<UILabel>();
            lbl.text ="[0000FF]" + (i + 1).ToString() + "[-]";
            lbl.depth = 20;
            lbl.width = 50;*/
        }
    }
    #endregion

    private void Tutorial()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            missionInfo.arrowAttack.SetActive(true);
        }
    }
}
