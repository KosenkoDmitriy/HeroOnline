using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;
using DEngine.Common.Config;

public class UIHeroUpgradeManager : MonoBehaviour {

    public struct Feed
    {
       public UserRole role;
       public GameObject go;
    }
    public enum Status
    {
        Idle,
        Rotate,
        ReduceSpeed,
        Destroy,
        Finished
    }

    public UITexture quality;
    public UITexture icon;
    public GameObject feedRoot;
    public GameObject feedPrefab;
    public GameObject heroRoot;
    public GameObject heroPrefab;
    public float distanceFeed = 180;
    public float angleFeed = 20;
    public float startAngle = -20;
    public float maxSpeed = 5;
    public float minSpeed = 0.5f;
    public float duration = 3;
    public float offsetDestroy = 2;

    public UILabel lblMainName;
    public UILabel lblMainLevel;
    public UIHeroStarManager mainStarManager;

    public UILabel lblSelectName;
    public UILabel lblSelectLevel;
    public UILabel lblSelectSkill;
    public UILabel lblSelectStar;
    public UIHeroStarManager selectedStar;
    public UILabel lblStat;
    public UITexture iconSkill1;
    public UITexture iconSkill2;
   

    public UILabel lblNextLevelInfor;
    public UILabel lblNextLevel;
    public UILabel lblNextStatUp;
    public UILabel lblExp;
    public UILabel lblLevelUp;
    public UIProgressBar expBar;

    public GameObject NotificationRoot;
    public UILabel Notification_lblHeader;
    public UILabel Notification_lblOk;
    public UILabel Notification_lblCancel;
    public UILabel Notification_lblText;
    public UIButton Notification_btnOk;
    public UIButton Notification_btnCancel;

    public Dictionary<int, Feed> feeds { get { return _feeds; } }

    public UIStatsManager statManager;
    public GameObject particleEatPrefab;
    public GameObject particleEatEndPrefab;

    public GameObject userRoleRoot;
    public GameObject ChoseHeroPrefab;

    public GameObject arrowSelectHero;
    public UILabel lblArrowSelectHero;
    public GameObject arrowSelectFeed;
    public UILabel lblarrowSelectFeed;
    public GameObject arrowButton;
    public UIPlayTween tweenCloseSelectedHero;
    public UILabel lblGoldNeed;
    public UILabel lblSilverNeed;

    private Status _status;
    private Dictionary<int, Feed> _feeds;
    private List<UIHeroUpgradeHeroSlot> _heros;
    private float _speed;
    private float _timer;
    private UserRole _curRoleUpgrade;
    private UIHeroUpgradeHeroSlot _selectedHeroSlot;
    private HeroUpgradeController _controller;
    private int _addEXP;
    private UserRole _tempRole;
    private List<UIHeroSlotChoseForUpgrade> _choseHeroSlots;
    private UIHeroSlotChoseForUpgrade _heroSlotForUpgradeChose;
    private float _silverNeed;
    private float _goldNeed;

    void Start () {
        _status = Status.Idle;
        _feeds = new Dictionary<int, Feed>();     
        _controller = new HeroUpgradeController(this);
        _choseHeroSlots = new List<UIHeroSlotChoseForUpgrade>();
        _heros = new List<UIHeroUpgradeHeroSlot>();
        Localization();
        arrowSelectHero.SetActive(true);
        
        ShowAllUserRoles();
        InitAllFeed();

        if (GameManager.CurRoleSelectedUpGrade != null)
        {
            OnSelectedRoleForUpgrade(new UIHeroSlotChoseForUpgrade() { _userRole = GameManager.CurRoleSelectedUpGrade });
            GameManager.CurRoleSelectedUpGrade = null;
        }

	}

    void Update()
    {
        if (_status == Status.Idle || _status == Status.Finished) return;

        if (Time.time - _timer > duration && _status == Status.Rotate)
           _status = Status.ReduceSpeed;

        if (_status == Status.Rotate)
        {
            _speed += Time.deltaTime * 3;
            _speed = Mathf.Min(_speed, maxSpeed);
        }
        
        Playanim();

        if (_feeds.Count == 0)
        {
            OnFinished();
        }
    }

    #region public methods
    public void OnButtonLevelUp_Click()
    {
        if (IsLock()) return;
        if (_feeds.Count <= 0)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_UpGrade_NotEnougtFeed"), UINoticeManager.NoticeType.Message);
            return;
        }

        if (GameManager.GameUser.UserRoles.Count - _feeds.Count < 3)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_UpGrade_Less3Hero"), UINoticeManager.NoticeType.Message);
            return;
        }

        if (GameManager.GameUser.Base.Silver < _silverNeed)
        {
            Helper.HandleCashInsufficient();
            return;
        }

        int[] feedIDs = (from item in _feeds select item.Value.role.Id).ToArray();

        _controller.SendRequestUpgrade(_curRoleUpgrade, feedIDs);

    }
    public void OnReciveFormServer()
    {
        int[] feedIDs = (from item in _feeds select item.Value.role.Id).ToArray();

        foreach (int id in feedIDs)
        {
            UserRole role = GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == id);
            if (role != null)
            {
                GameManager.GameUser.UserRoles.Remove(role);
            }
        }


        _status = Status.Rotate;
        _speed = 0;
        _timer = Time.time;
        _curRoleUpgrade.AddExp(_addEXP);
    }
    public void OnSelectedHero(UserRole userRole, bool isSelect)
    {
        arrowSelectFeed.SetActive(false);
        arrowButton.SetActive(true);
        if (_feeds.ContainsKey(userRole.Id))
        {
            if (!isSelect)
            {
                Destroy(_feeds[userRole.Id].go);
                _feeds.Remove(userRole.Id);
                CalculatorEXPNext();
                PrefreshFeed();
            }
        }
        else
        {
            if (isSelect)
            {
                OnSelectedHeroToFeed(userRole);
                ShowInformationSelected(userRole);
                CalculatorEXPNext();
            }
        }

        _silverNeed = RoleConfig.EVOLVE_SIVLER * feeds.Count;
        lblSilverNeed.text = _silverNeed.ToString();
        lblGoldNeed.text = "0";
    }
    public bool IsLock()
    {
        if (_status != Status.Idle && _status != Status.Finished) return true;
        return false;
    }
    public void OnButtonClose_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.HeroUpgrade, GameScenes.previousSence);
    }
    public void OnShowYesNo(string text,UIHeroUpgradeHeroSlot curSlot)
    {
        NotificationRoot.SetActive(true);
        Notification_lblText.text = text;
        Notification_btnOk.gameObject.SetActive(true);
        _selectedHeroSlot = curSlot;
    }
    public void OnShowWarning(string text, UIHeroUpgradeHeroSlot curSlot)
    {
        _selectedHeroSlot = curSlot;
        NotificationRoot.SetActive(true);
        Notification_lblText.text = text;
        Notification_btnOk.gameObject.SetActive(false);
    }
    public void OnButton_OK()
    {
        _selectedHeroSlot.OnOKButtonClick();
        NotificationRoot.SetActive(false);
    }
    public void OnButton_Cancel()
    {
        NotificationRoot.SetActive(false);
    }
    public void OnSelectedRoleForUpgrade(UIHeroSlotChoseForUpgrade uiHeroChose)
    {
        ClearFeed();
        arrowSelectHero.SetActive(false);
        arrowSelectFeed.SetActive(true);
        arrowButton.SetActive(false);
        if (_heroSlotForUpgradeChose != null)
            _heroSlotForUpgradeChose.OnDeSelected();
        _heroSlotForUpgradeChose = uiHeroChose;
        _curRoleUpgrade = uiHeroChose._userRole;
        ShowCurRoleUpgrade();
        StartCoroutine(ShowAllFeed());
        ShowStat();
        _addEXP = 0;

        tweenCloseSelectedHero.Play(true);
    }
    #endregion
   
    
    #region private methods
    private void ClearFeed()
    {
        foreach (var item in _feeds)
            Destroy(item.Value.go);
        _feeds.Clear();
    }
    private void ShowAllUserRoles()
    {
        UIGrid grid= userRoleRoot.GetComponent<UIGrid>();
        foreach (UserRole role in GameManager.GameUser.UserRoles)
        {
            if (role.Base.Level < RoleConfig.LEVEL_MAX)
            {
                GameObject go = NGUITools.AddChild(userRoleRoot, ChoseHeroPrefab);
                UIHeroSlotChoseForUpgrade slot = go.GetComponent<UIHeroSlotChoseForUpgrade>();
                slot.SetUser(role, this);
                _choseHeroSlots.Add(slot);
            }
        }
        grid.Reposition();
       
    }
    private void ShowStat()
    {
        _tempRole = new UserRole();
        _tempRole.GameRole = _curRoleUpgrade.GameRole;
        _tempRole.Base = _curRoleUpgrade.Base;
        _tempRole.Attrib = _curRoleUpgrade.Attrib;
        _tempRole.AddExp(_addEXP);
        _tempRole.RoleItems.AddRange(_curRoleUpgrade.RoleItems);
        _tempRole.InitAttrib();
        statManager.SetRole(_curRoleUpgrade, _tempRole);
    }
    private void Localization()
    {
        Notification_lblHeader.text = GameManager.localization.GetText("Dialog_Notice");
        Notification_lblOk.text = GameManager.localization.GetText("Global_btn_OK");
        Notification_lblCancel.text = GameManager.localization.GetText("Global_btn_Cancel");
        lblLevelUp.text = GameManager.localization.GetText("Hero_btn_LvlUp");
        lblArrowSelectHero.text = GameManager.localization.GetText("Tut_HeroUpEXP_SelectHeroForUp");
        lblarrowSelectFeed.text = GameManager.localization.GetText("Tut_HeroUpEXP_SelectedFeedHero");

    }
    private void ShowCurRoleUpgrade()
    {
        lblMainName.text = _curRoleUpgrade.Name;
        lblMainLevel.text = GameManager.localization.GetText("Global_Lvl") + _curRoleUpgrade.Base.Level;
        mainStarManager.SetStart(_curRoleUpgrade.Base.Grade);
        quality.mainTexture = Helper.LoadTextureElement((int)_curRoleUpgrade.Base.ElemId);
        icon.mainTexture = Helper.LoadTextureForHero(_curRoleUpgrade.Base.RoleId);

        lblNextLevelInfor.text = GameManager.localization.GetText("Hero_Upgrade_NextLevelInfo");
        lblNextLevel.text = GameManager.localization.GetText("Hero_Upgrade_NextLevel");

        expBar.value = (float)_curRoleUpgrade.Base.Exp / (float)RoleConfig.LEVELS_EXP[_curRoleUpgrade.Base.Level];
    }
    private void ShowInformationSelected(UserRole userRole)
    {
        lblSelectName.text = userRole.Name;
        lblSelectLevel.text = GameManager.localization.GetText("Global_Level") + userRole.Base.Level;
        lblSelectSkill.text = GameManager.localization.GetText("Global_Skill");
        lblSelectStar.text = GameManager.localization.GetText("Hero_Upgrade_Star");

        if (!iconSkill1.gameObject.activeInHierarchy)
            iconSkill1.gameObject.SetActive(true);
        if (!iconSkill2.gameObject.activeInHierarchy)
            iconSkill2.gameObject.SetActive(true);
        if (!lblSelectStar.gameObject.activeInHierarchy)
            lblSelectStar.gameObject.SetActive(true);

        iconSkill1.mainTexture = Helper.LoadTextureForSkill(userRole, 1);
        iconSkill2.mainTexture = Helper.LoadTextureForSkill(userRole, 2);

        selectedStar.SetStart(userRole.Base.Grade);
        lblStat.text = "";            
        
    }
    private void Playanim()
    {
        for (int i = 0; i < _feeds.Count; i++)
        {
            Feed f = _feeds.Values.ToList()[i];
            Vector3 newPos = Rotate(f.go.transform.localPosition, _speed);
            f.go.transform.localPosition = newPos;

            UITexture icon = f.go.transform.GetChild(1).GetComponent<UITexture>();
            UITexture qualityFeed = f.go.transform.GetChild(0).GetComponent<UITexture>();
            icon.depth = -(int)newPos.z;
            qualityFeed.depth = -(int)newPos.z;            

            if (_status == Status.ReduceSpeed && newPos.z <= -distanceFeed + offsetDestroy && i == _feeds.Count - 1)
            {
                Invoke("OnChangeDestroyStatus", 0.5f);
            }         

            if (_status == Status.Destroy && newPos.z <= -distanceFeed + offsetDestroy)
            {
                Destroy(f.go);
                _feeds.Remove(f.role.Id);
                GameObject go = GameObject.Instantiate(particleEatPrefab) as GameObject;
                go.transform.parent = quality.transform;
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = Vector3.zero;
                Destroy(go, 1);
            }
        }
    }
    private void OnChangeDestroyStatus()
    {
        _status = Status.Destroy;
    }
    private void OnParticleEnd()
    {
        GameObject go = GameObject.Instantiate(particleEatEndPrefab) as GameObject;
        go.transform.parent = quality.transform;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.localPosition = Vector3.zero;
        Destroy(go, 1);
    }
    private void OnFinished()
    {
        Invoke("OnParticleEnd", 0.6f);

        _status = Status.Finished;
        for (int i = 0; i < _heros.Count; i++)
        {
           // Debug.Log(i + " " + _heros[i].gameObject.name + " " + _heros[i].IsSelected);
            if (_heros[i].IsSelected)
            {
                NGUITools.Destroy(_heros[i].gameObject);
                _heros.Remove(_heros[i]);
                i--;
            }
        }      
        heroRoot.GetComponent<UIGrid>().Reposition();
    }
    private void OnSelectedHeroToFeed(UserRole userRole)
    {
        GameObject go = NGUITools.AddChild(feedRoot, feedPrefab);
        go.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        Vector3 pos = CalculatorPos(_feeds.Count, angleFeed);
        go.transform.localPosition =pos;

        if (userRole != null)
        {
            UITexture icon = go.transform.GetChild(1).GetComponent<UITexture>();
            UITexture quality = go.transform.GetChild(0).GetComponent<UITexture>();
            quality.mainTexture = Helper.LoadTextureElement((int)userRole.Base.ElemId);
            icon.mainTexture = Helper.LoadTextureForHero(userRole.Base.RoleId);
            icon.depth = -(int)pos.z;
            quality.depth = -(int)pos.z;
        }

        Feed f = new Feed();
        f.role = userRole;
        f.go = go;
        _feeds[userRole.Id] = f;
    }
    private void PrefreshFeed()
    {
        int i = 0;
        foreach (var feed in _feeds)
        {
            feeds[feed.Key].go.transform.localPosition = CalculatorPos(i, angleFeed);
            i++;
        }
    }
    private void CalculatorEXPNext()
    {
        List<UserRole> roles = (from item in _feeds select item.Value.role).ToList();
        _addEXP = GameManager.GameUser.GetAddExpFromRoles(roles);
        //lblExp.text = GameManager.localization.GetText("Global_btn_Exp") + ": [00FF00]+" + _addEXP + "[-]";
       
        ShowStat();

        expBar.value = (float)_tempRole.Base.Exp / (float)RoleConfig.LEVELS_EXP[_curRoleUpgrade.Base.Level];

        if (_tempRole.Base.Level > _curRoleUpgrade.Base.Level)
            lblMainLevel.text = GameManager.localization.GetText("Global_Lvl") + "[00FF00]" + _tempRole.Base.Level + "[-]";
        else
            lblMainLevel.text = GameManager.localization.GetText("Global_Lvl") + _curRoleUpgrade.Base.Level;
    }
    private Vector3 CalculatorPos(int index, float angle)
    {
        Vector3 pos = Vector3.zero;
        float curAngle = (index * angle) + startAngle;
        pos.x = Mathf.Sin(curAngle * Mathf.Deg2Rad) * -distanceFeed;
        pos.z = Mathf.Cos(curAngle * Mathf.Deg2Rad) * -distanceFeed;
       // float vp = distanceFeed * distanceFeed - (pos.x * pos.x) - (pos.z * pos.z);
        pos.y = 0;// Mathf.Sqrt(Mathf.Abs(vp)) * distanceFeed * 5;
        return pos;
    }
    private Vector3 Rotate(Vector3 pos,float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        Vector3 newPos = Vector3.zero;
        newPos.x = pos.x * Mathf.Cos(radian) - pos.z * Mathf.Sin(radian);
        newPos.z = pos.z * Mathf.Cos(radian) + pos.x * Mathf.Sin(radian);
        //float vp = distanceFeed * distanceFeed - (pos.x * pos.x) - (pos.z * pos.z);
        newPos.y = 0;// Mathf.Sqrt(Mathf.Abs(vp)) * distanceFeed * 5; 
        return newPos;
    }
    private IEnumerator ShowAllFeed()
    {
        foreach (UIHeroUpgradeHeroSlot slot in _heros)
        {
            if (slot.userRole.Id != _curRoleUpgrade.Id && slot.userRole.Base.Status != RoleStatus.Active)
            {
                if (!slot.gameObject.activeInHierarchy)
                    slot.gameObject.SetActive(true);
            }
            else
            {
                if (slot.gameObject.activeInHierarchy)
                    slot.gameObject.SetActive(false);
            }
            heroRoot.GetComponent<UIGrid>().Reposition();
            yield return null;
        }
    }
    private void InitAllFeed()
    {
        int numHero = GameManager.GameUser.UserRoles.Count;
        for (int i = 0; i < numHero; i++)
        {
            UserRole role = GameManager.GameUser.UserRoles[i];
            
            GameObject go = NGUITools.AddChild(heroRoot, heroPrefab);
            go.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            UIHeroUpgradeHeroSlot slot = go.GetComponent<UIHeroUpgradeHeroSlot>();
            slot.SetRole(role, this);
            _heros.Add(slot);
            go.SetActive(false);
            heroRoot.GetComponent<UIGrid>().Reposition();            
        }
    }
    #endregion
}
