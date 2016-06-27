using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;
using DEngine.Common.Config;

public class UIArenaManager : MonoBehaviour {

    public enum Type
    {
        PVA,
        PVP
    }

    public UILabel lblArena;

    public UILabel lblbtnYourRank;
    public UILabel lblbtnFight;
    public UILabel lblbtnBack;

    public UILabel lblInfo;
    public UILabel lblYourCore;
    public UILabel lblYourCoreValue;
    public UILabel lblYourRank;
    public UILabel lblYourRankValue;
    //public UISprite lblYourRankStatus;
    public UILabel lblweeklyReward;

    public UIPanel RewardPanel;
    public GameObject RewardRoot;
    public GameObject ItemRewardObject;

    public UILabel lblWin;
    public UILabel lblLose;
    public UILabel lblRatio;
    public UILabel lblLastMatch;
    public UILabel lblRanking;

    public UIGrid UserTopRoot;
    public GameObject UserTopPrefab;

    public UIArenaEnemyManager enemyManager;

    public UIUserTopManager userTopSelected { get; set; }
    public ArenaController controller { get; set; }

    public GameObject lastMatchRoot;
    public GameObject lastMatchPrefab;

    public Type type;
    public GameObject AttackRoot;

    public UILabel lblAttack;
    public UILabel lblView;

    private Dictionary<int, int> _rewardItems;
    private List<UIUserTopManager> _userTopList;
    private GameObjList _topUserMini;
    private int _curPage;
    private int _maxPage;
    private int _maxObjPerPage;
    private UIUserTopManager _userTop;

	void Awake () {
        
        Localization();
	}

    void Start()
    {
        controller = new ArenaController(this);

        _rewardItems = new Dictionary<int, int>();
        _rewardItems[59] = 1;
        _rewardItems[60] = 2;
        _rewardItems[61] = 3;

        _userTopList = new List<UIUserTopManager>();
        _curPage = 1;
        _maxObjPerPage = 8;

        controller.SendRequestTopList();
        controller.SendRequestReportList();
        ShowInforUser();
        ShowItemReward();
    }

    #region private methods
    private void ShowPage()
    {
        for (int i = 0; i < _userTopList.Count; i++)
        {
            if (i >= (_curPage - 1) * _maxObjPerPage && i < _curPage * _maxObjPerPage)
            {
                _userTopList[i].gameObject.SetActive(true);
            }
            else
            {
                _userTopList[i].gameObject.SetActive(false);
            }
        }
        UserTopRoot.Reposition();
    }
    private void ShowTopUser()
    {
        if (_topUserMini == null)
        {
            lblbtnYourRank.text = string.Format("{0}/{1}", 1, 1);
            _maxPage = 1;
            _curPage = 1;
            return;
        }

        _maxPage = Mathf.CeilToInt((float)_topUserMini.Count / (float)_maxObjPerPage);

        lblbtnYourRank.text = string.Format("{0}/{1}", _curPage, _maxPage);

        for (int i = 0; i < _topUserMini.Count; i++)
        {
            GameObject go = NGUITools.AddChild(UserTopRoot.gameObject, UserTopPrefab);
            UIUserTopManager userTop = go.GetComponent<UIUserTopManager>();
            userTop.SetUser((GameUser)_topUserMini[i], this);
            _userTopList.Add(userTop);

            if (i >= _maxObjPerPage)
                go.SetActive(false);
        }
        UserTopRoot.Reposition();
    }
    private void ShowItemReward()
    {
        foreach (var reward in _rewardItems)
        {
            GameObject go = NGUITools.AddChild(RewardRoot, ItemRewardObject);
            go.SetActive(true);
            go.transform.FindChild("Label").GetComponent<UILabel>().text = "x" + reward.Value.ToString();
            go.transform.FindChild("Texture").GetComponent<UITexture>().mainTexture = Helper.LoadTextureForSupportItem(reward.Key);
        }
        RewardRoot.GetComponent<UIGrid>().Reposition();
    }
    private void ShowInforUser()
    {
        float win = 0, lose = 0;
        float ratio = 0;
        
        GameUser gameUser = GameManager.GameUser;

        win = GameManager.GameUser.Base.TotalWon;
        lose = GameManager.GameUser.Base.TotalLost;

        if ((win + lose) > 0)
            ratio = (win / (win + lose)) * 100;
        else
            ratio = 0;

        lblYourCoreValue.text = gameUser.Base.Honor.ToString();
        lblYourRankValue.text = gameUser.Base.HonorRank.ToString();
        lblWin.text = string.Format(GameManager.localization.GetText("Arena_Win"), string.Format("[00FF00]{0}[-]", win));
        lblLose.text = string.Format(GameManager.localization.GetText("Arena_Lose"), string.Format("[FF0000]{0}[-]", lose));
        lblRatio.text = string.Format(GameManager.localization.GetText("Arena_Ratio"), ratio.ToString("0.0") + " %");
                  
    }
    private void Localization()
    {
        lblArena.text = GameManager.localization.GetText("Arena_Header");
        lblbtnFight.text = GameManager.localization.GetText("Arena_btnFight");
        lblbtnYourRank.text = GameManager.localization.GetText("Arena_btnYourRank");
        lblbtnBack.text = GameManager.localization.GetText("Shop_Back");
        lblYourCore.text = GameManager.localization.GetText("Arena_YourCore");
        lblYourRank.text = GameManager.localization.GetText("Arena_Rank");
        lblweeklyReward.text = GameManager.localization.GetText("Arena_CurrentWeeklyReward");
        lblLastMatch.text = GameManager.localization.GetText("Arena_LastMatch");
        lblRanking.text = GameManager.localization.GetText("Arena_HeaderRanking");
        lblAttack.text = GameManager.localization.GetText("Arena_Attack");
        lblView.text = GameManager.localization.GetText("Arena_ViewEmpire");
        string info = GameManager.localization.GetText("Arena_Info");

        lblInfo.text = Helper.StringToMultiLine(info);
    }
    #endregion

    #region Button
    public void OnButtonYourRank_Click()
    {

    }
    public void OnButtonLeft_Click()
    {
        _curPage = Mathf.Max(_curPage - 1, 1);
        lblbtnYourRank.text = string.Format("{0}/{1}", _curPage, _maxPage);
        ShowPage();
    }
    public void OnButtonRight_Click()
    {
        _curPage = Mathf.Min(_curPage + 1, _maxPage);
        lblbtnYourRank.text = string.Format("{0}/{1}", _curPage, _maxPage);
        ShowPage();
    }
    public void OnButtonYourFight_Click()
    {
        if (!GameManager.CheckHeroToBattle()) return;

        int silverNeed = GameConfig.ARENALOSTSILVER;

        if (GameManager.GameUser.Base.Silver < silverNeed)
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Arena_NotSilver"), silverNeed), UINoticeManager.NoticeType.Message);
            return;
        }

        type = Type.PVP;
        controller.SendRandomBattle();
        enemyManager.gameObject.SetActive(true);
        enemyManager.Init();
    }
    public void OnButtonYourBack_Click()
    {
        if (type == Type.PVP)
            controller.SendQuitArena();

        GameScenes.ChangeScense(GameScenes.MyScene.Arena, GameScenes.MyScene.WorldMap);
    }
    public void OnExitArene()
    {
        controller.SendQuitArena();
    }
    #endregion

    #region public
    public void OnResponseReport()
    {
        for (int i = 0; i < GameManager.GameUser.PvPLogs.Count; i++)
        {
            GameObject go = NGUITools.AddChild(lastMatchRoot, lastMatchPrefab);

            PvPLog log = GameManager.GameUser.PvPLogs[i];

            go.GetComponent<UILastMatchManager>().SetLog(log, this);

            go.SetActive(true);
        }
        lastMatchRoot.GetComponent<UIGrid>().Reposition();
    }
    public void OnSlected(UIUserTopManager uiUser)
    {
        if (userTopSelected != null)
            userTopSelected.OnDeSelected();

        userTopSelected = uiUser;
        userTopSelected.OnSelected();
    }
    public void OnReciveListTopUser(GameObjList gameUserMini)
    {
        _topUserMini = gameUserMini;
        ShowTopUser();
    }
    public void OnReciveBattleInfo()
    {
        enemyManager.SetInfoEnemy();
    }
    public void OnButtonReplay_Click(PvPLog log)
    {
        controller.SendReplay(log.Opponent.Id);
        type = Type.PVA;
    }
    public void OnClickAttack(UIUserTopManager userTop)
    {
        _userTop = userTop;
        AttackRoot.SetActive(true);
    }
    public void OnButton_AttackClick()
    {
        controller.SendReplay(_userTop._miniUser.Id);
    }
    public void OnButton_ViewClick()
    {
        UIEmpireManager.isFriend = true;
        UIEmpireManager.friendLand = _userTop._miniUser.Land;
        GameScenes.ChangeScense(GameScenes.MyScene.Pillage, GameScenes.MyScene.Empire);
    }
    public void CloseAttack()
    {
        AttackRoot.SetActive(false);
    }
    #endregion

}
