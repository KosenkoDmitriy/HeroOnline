using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using DEngine.Common.GameLogic;
using System.Linq;

public class UISocialManager : MonoBehaviour {
    
    [System.Serializable]
    public struct LocalizationLabel
    {
        public UILabel lblFriend;
        public UILabel lblMail;
        public UILabel lblEnemy;
        public UILabel lblFiend2;
        public UILabel lblReport;
        public UILabel lblSystem;
        public UILabel lblTabtoChat;
        public UILabel lblbtnSendChat;
        public UILabel lblbtnRevenge;
        public UILabel lblbtnSort;
        public UILabel lblbtnDelete;
        public UILabel lblbtnExit;
        public UILabel lblbtnAdd;
        public UILabel lblbtnTakeAll;
        public UILabel lblbtnDeailDelete;
        public UILabel lblTabChoseFriend;
        public UILabel lblTabChoseEnemy;

        public UILabel lblFindUser_Header;
        public UILabel lblFindUser_ID;
        public UILabel lblFindUser_TypeIDOfUser;
        public UILabel lblFindUser_btnAdd;
        public UILabel lblFindUser_btnCancel;
    }

    [System.Serializable]
    public struct UITab
    {
        public UIToggle friend;
        public UIToggle mail;
        public UIToggle subFriend;
        public UIToggle subEnemy;
        public UIToggle subReport;
        public UIToggle subSystem;
    }

    [System.Serializable]
    public struct UISlot
    {
        public UIGrid root;
        public GameObject prefab;
        public List<GameObject> list { get; set; }
    }


    public enum FriendActiveType
    {
        AddFriend,
        AddEnemy,
        RemoveFriend,
        RemoveEnemy,
    }

    public enum Status
    {
        None,
        Friend,
        Mail
    }

    public enum SubStatus
    {
        None,
        Friend,
        Enemy,
        Report,
        System        
    }

    public LocalizationLabel localizationLabel;
    public UITab uiTab;

    public Status curStatus;
    public SubStatus curSubStatus;
    public UISlot uiEnemy;
    public UISlot uiFriend;
    public UISlot uiReport;
    public UISlot uiSystem;

    public GameObject objTabFriend;
    public GameObject objTabEmail;
    public UISocialMailDetailManager objMailDetail;
    public GameObject objAddUser;
    public GameObject objMailAddUser;
    public UISocialChatManager chatManager;

    public UITexture avatar;
    public GameObject avatarSetRoot;
    public GameObject avatarPrefab;
    public UIInput inputUser;

    public FriendActiveType friendActiveType { get { return _friendActiveType; } set { _friendActiveType = value; } }

    private UISocialUserManager _selectedUserSlot;
    private UISocialReportManager _selectedReportSlot;
    private UIsocialSystemManager _selectedSystemSlot;
    private FriendActiveType _friendActiveType;
    private SocialController _controller;
    public string _curNicknameAdd;



    void Awake()
    {
        _controller = new SocialController(this);
        _controller.SendRequestFriendList();
        _controller.SendRequestMailList();
        _controller.SendRequestReportList();
        _friendActiveType = FriendActiveType.AddFriend;
        _curNicknameAdd = "";

        Localization();

        uiTab.mail.value = true;
        uiTab.subReport.value = true;

        curStatus = Status.None;
        curSubStatus = SubStatus.None;

        uiEnemy.list = new List<GameObject>();
        uiFriend.list = new List<GameObject>();
        uiSystem.list = new List<GameObject>();
        uiReport.list = new List<GameObject>();

        OnChangeStatus(Status.Mail);
        OnChangeSubStatus(SubStatus.Report);

        InitAvatarSet();
    }

	void Start () 
    {
        avatar.mainTexture = Helper.LoadTextureForAvatar(GameManager.GameUser.Base.Avatar);
	}

    #region private methods
    private void InitAvatarSet()
    {
        for (int i = 0; i <= 50; i++)
        {
            GameObject go = NGUITools.AddChild(avatarSetRoot, avatarPrefab);
            UIAvatarController avatar = go.GetComponent<UIAvatarController>();
            avatar.setAvatar(i, this);
            go.name = "Avatar_" + i.ToString("00");
        }
        avatarSetRoot.GetComponent<UIGrid>().Reposition();
    }
    private void OnChangeStatus(Status status)
    {
        if (status == curStatus) return;
        curStatus = status;
        switch (status)
        {
            case Status.Friend:
                if (objTabEmail.activeInHierarchy)
                    objTabEmail.SetActive(false);
                if (!objTabFriend.activeInHierarchy)
                    objTabFriend.SetActive(true);

                uiTab.subFriend.value = true;
                OnChangeSubStatus(SubStatus.Friend);
                break;
            case Status.Mail:
                if (!objTabEmail.activeInHierarchy)
                    objTabEmail.SetActive(true);
                if (objTabFriend.activeInHierarchy)
                    objTabFriend.SetActive(false);

                uiTab.subReport.value = true;
                OnChangeSubStatus(SubStatus.Report);
                break;
        }
    }
    private void OnChangeSubStatus(SubStatus subStatus)
    {
        if (subStatus == curSubStatus) return;
        curSubStatus = subStatus;
        switch (curSubStatus)
        {
            case SubStatus.Enemy:
                if (_selectedUserSlot != null)
                {
                    _selectedUserSlot.OnDeSelected();
                    _selectedUserSlot = null;
                }
                uiEnemy.root.gameObject.SetActive(true);
                uiFriend.root.gameObject.SetActive(false);
                uiEnemy.root.Reposition();
                _friendActiveType = FriendActiveType.AddEnemy;
                break;

            case SubStatus.Friend:
                if (_selectedUserSlot != null)
                {
                    _selectedUserSlot.OnDeSelected();
                    _selectedUserSlot = null;
                }
                uiEnemy.root.gameObject.SetActive(false);
                uiFriend.root.gameObject.SetActive(true);
                uiFriend.root.Reposition();
                _friendActiveType = FriendActiveType.AddFriend;
                break;

            case SubStatus.Report:
                if (_selectedSystemSlot != null)
                {
                    _selectedSystemSlot.OnDeSelected();
                    _selectedSystemSlot = null;
                }
                uiReport.root.gameObject.SetActive(true);
                uiSystem.root.gameObject.SetActive(false);
                break;

            case SubStatus.System:
                if (_selectedReportSlot != null)
                {
                    _selectedReportSlot.OnDeSelected();
                    _selectedReportSlot = null;
                }
                uiReport.root.gameObject.SetActive(false);
                uiSystem.root.gameObject.SetActive(true);
                break;
        }
    }
    private void Localization()
    {
        localizationLabel.lblFriend.text = GameManager.localization.GetText("Social_Tab_Friend");
        localizationLabel.lblMail.text = GameManager.localization.GetText("Social_Tab_Mail");
        localizationLabel.lblEnemy.text = GameManager.localization.GetText("Social_Tab_Enemy");
        localizationLabel.lblFiend2.text = GameManager.localization.GetText("Social_Tab_Friend");
        localizationLabel.lblReport.text = GameManager.localization.GetText("Social_Tab_Report");
        localizationLabel.lblSystem.text = GameManager.localization.GetText("Social_Tab_System");
        localizationLabel.lblTabtoChat.text = GameManager.localization.GetText("Social_TabToChat");
        localizationLabel.lblbtnSendChat.text = GameManager.localization.GetText("Social_btnSendChat");
        localizationLabel.lblbtnRevenge.text = GameManager.localization.GetText("Social_btnRevenge");
        localizationLabel.lblbtnSort.text = GameManager.localization.GetText("Social_btnSort");
        localizationLabel.lblbtnDelete.text = GameManager.localization.GetText("Social_btnDelete");
        localizationLabel.lblbtnExit.text = GameManager.localization.GetText("Social_btnExit");
        localizationLabel.lblbtnAdd.text = GameManager.localization.GetText("Social_Mail_AddFriend");
        localizationLabel.lblbtnTakeAll.text = GameManager.localization.GetText("Social_btnTakeAll");
        localizationLabel.lblTabChoseFriend.text = GameManager.localization.GetText("Social_Tab_Friend");
        localizationLabel.lblTabChoseEnemy.text = GameManager.localization.GetText("Social_Tab_Enemy");
        
        localizationLabel.lblFindUser_ID.text = GameManager.localization.GetText("Social_FindUser_ID");
        localizationLabel.lblFindUser_TypeIDOfUser.text = GameManager.localization.GetText("Social_FindUser_TypeIDOfUser");
        localizationLabel.lblFindUser_btnAdd.text = GameManager.localization.GetText("Social_FindUser_btnAdd");
        localizationLabel.lblFindUser_btnCancel.text = GameManager.localization.GetText("Social_FindUser_Cancel");
    }
    private IEnumerator InitFiendList(bool Enemy = false)
    {
        UISlot curUIUser;
        if (Enemy)
            curUIUser = uiEnemy;
        else
            curUIUser = uiFriend;
      
        Resources.UnloadUnusedAssets();

        List<UserFriend> friends;

        if (!Enemy)
            friends = GameManager.GameUser.UserFriends.Where(p => p.Mode == (int)UserRelation.FriendOne || p.Mode == (int)UserRelation.FriendTwo).ToList();
        else
            friends = GameManager.GameUser.UserFriends.Where(p => p.Mode == (int)UserRelation.Enemy).ToList();

        for (int i = 0; i < friends.Count; i++)
        {
            GameObject go = NGUITools.AddChild(curUIUser.root.gameObject, curUIUser.prefab);
            curUIUser.list.Add(go);
            go.SetActive(true);
            go.GetComponent<UISocialUserManager>().SetUser(friends[i], this, Enemy);
            curUIUser.root.Reposition();
            yield return null;
        }

    }
    private IEnumerator InitReport()
    {
        List<UIReport> reports = GameManager.uiReport;
        for (int i = 0; i < reports.Count; i++)
        {
            GameObject go = NGUITools.AddChild(uiReport.root.gameObject, uiReport.prefab);
            uiReport.list.Add(go);
            go.SetActive(true);
            go.GetComponent<UISocialReportManager>().SetReport(this, reports[i]);
            uiReport.root.Reposition();
            yield return null;
        }       
    }
    private IEnumerator InitSystem()
    {
        List<UserMail> userMail = GameManager.GameUser.UserMails;
        for (int i = 0; i < userMail.Count; i++)
        {
            GameObject go = NGUITools.AddChild(uiSystem.root.gameObject, uiSystem.prefab);
            uiSystem.list.Add(go);
            go.SetActive(true);
            go.GetComponent<UIsocialSystemManager>().SetSystem(this, userMail[i]);
            uiSystem.root.Reposition();
            yield return null;
        }
    }
    private void OnConfrimDeleteUser()
    {
        if (_selectedUserSlot._gameUser.Mode != 0)
        {
            _friendActiveType = FriendActiveType.RemoveFriend;
        }
        else
        {
            _friendActiveType = FriendActiveType.RemoveEnemy;
        }
        _curNicknameAdd = _selectedUserSlot._gameUser.Opponent.Base.NickName.Trim();
        _controller.SendRequestRemoveFriend(_curNicknameAdd);
    }    
    private void OnConfrimDeleteMail()
    {
        if (_selectedSystemSlot != null)
        {
            _controller.SendRequestReadMail(_selectedSystemSlot._userMail.MailId);
        }            
    }    
    #endregion
    
    #region public methods
    public void SendRequestAddFriend(string nickName)
    {
        _controller.SendRequestAddFriend(nickName.Trim());     
    }
    public void OnSelectAvatar(int ID)
    {
        GameManager.GameUser.Base.Avatar = ID;
        avatar.mainTexture = Helper.LoadTextureForAvatar(ID);
        _controller.SendChangeAvatar(ID);
    }
    public void OnSelectedUser(UISocialUserManager userManager)
    {
        if (_selectedUserSlot != null)
            _selectedUserSlot.OnDeSelected();
        _selectedUserSlot = userManager;
    }
    public void OnSelectedReport(UISocialReportManager reportManager)
    {
        if (_selectedReportSlot != null)
            _selectedReportSlot.OnDeSelected();
        _selectedReportSlot = reportManager;
    }
    public void OnSelectedSytem(UIsocialSystemManager systemManager)
    {
        if (_selectedSystemSlot != null)
            _selectedSystemSlot.OnDeSelected();
        _selectedSystemSlot = systemManager;
    }
    public void OnReciveChat(int id, string s)
    {
        chatManager.OnReciveChat(id, s);
    }
    public void OnSendChat(string s)
    {
        _controller.SendChat(0, s);
    }
    public void OnResponseFriendList()
    {
        StartCoroutine(InitFiendList(true));
        StartCoroutine(InitFiendList());
    }
    public void OnResponseAddFriend()
    {
        string tab;
        if (_friendActiveType == FriendActiveType.AddFriend)
        {
            tab = GameManager.localization.GetText("Social_Tab_Friend");
            string s = string.Format(GameManager.localization.GetText("Social_FindUser_AddSuccess"), _curNicknameAdd, tab);
            MessageBox.ShowDialog(s, UINoticeManager.NoticeType.Message);
        }
        else if (_friendActiveType == FriendActiveType.AddEnemy)
        {
            tab = GameManager.localization.GetText("Social_Tab_Enemy");
            string s = string.Format(GameManager.localization.GetText("Social_FindUser_AddSuccess"), _curNicknameAdd, tab);
            MessageBox.ShowDialog(s, UINoticeManager.NoticeType.Message);
        }
        else if (_friendActiveType == FriendActiveType.RemoveEnemy)
        {
            tab = GameManager.localization.GetText("Social_Tab_Enemy");
            string s = string.Format(GameManager.localization.GetText("Social_Dialog_DeleteSuccess"), _curNicknameAdd, tab);
            MessageBox.ShowDialog(s, UINoticeManager.NoticeType.Message);
        }
        else if (_friendActiveType == FriendActiveType.RemoveFriend)
        {
            tab = GameManager.localization.GetText("Social_Tab_Friend");
            string s = string.Format(GameManager.localization.GetText("Social_Dialog_DeleteSuccess"), _curNicknameAdd, tab);
            MessageBox.ShowDialog(s, UINoticeManager.NoticeType.Message);
        }
      

        foreach (GameObject go in uiEnemy.list)
        {
            NGUITools.Destroy(go);
        }
        uiEnemy.list.Clear();

        foreach (GameObject go in uiFriend.list)
        {
            NGUITools.Destroy(go);
        }
        uiFriend.list.Clear();

        StartCoroutine(InitFiendList(true));
        StartCoroutine(InitFiendList());
    }    
    public void OnResponseCheckMail()
    {
        foreach (GameObject go in uiSystem.list)
        {
            NGUITools.Destroy(go);
        }
        uiSystem.list.Clear();
        StartCoroutine(InitSystem());
    }
    public void OnResponseReport()
    {
        StartCoroutine(InitReport());
    }
    public void SendRequestReadMail(int id)
    {
        _controller.SendRequestReadMail(id);
    }
    public void OnResponseDeleteMail()
    {
        if(_selectedSystemSlot!=null)
        {
            if (_selectedSystemSlot._userMail != null)
            {
                if (_selectedSystemSlot._userMail.Items.Count > 0)
                {
                    MessageBox.ShowDialog(GameManager.localization.GetText("Social_MailDetail_TakeGiftFinished"), UINoticeManager.NoticeType.Message);
                }
            }
        }

    }
    #endregion

    #region tab button
    public void OnFriendRevenge_Click()
    {
        if (_selectedUserSlot == null)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_Social_Revenge_NotSelectFriend"), UINoticeManager.NoticeType.Message);
            return;
        }

        if (_selectedUserSlot._gameUser == null) return;

        if (_selectedUserSlot._gameUser.Mode == 2|| _selectedUserSlot._gameUser.Mode == 0)
            _controller.SendRevenge(_selectedUserSlot._gameUser.Opponent.Id);
        else
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Social_RevengeFriend_NotFriend"), UINoticeManager.NoticeType.Message);
        }
        
    }
    public void onTabFriend_Click()
    {
        OnChangeStatus(Status.Friend);
    }
    public void onTabMail_Click()
    {
        OnChangeStatus(Status.Mail);
    }
    public void onSubTabFriend_Click()
    {
        OnChangeSubStatus(SubStatus.Friend);
    }
    public void onSubTabEnemy_Click()
    {
        OnChangeSubStatus(SubStatus.Enemy);
    }
    public void onSubTabReport_Click()
    {
        OnChangeSubStatus(SubStatus.Report);
    }
    public void onSubTabSystem_Click()
    {
        OnChangeSubStatus(SubStatus.System);
    }
    public void onButtonAdd_Click()
    {
        inputUser.value = "";
        if (curSubStatus == SubStatus.Friend)
            localizationLabel.lblFindUser_Header.text = GameManager.localization.GetText("Social_FindUser_HeaderAddFriend");
        else
            localizationLabel.lblFindUser_Header.text = GameManager.localization.GetText("Social_FindUser_HeaderAddEnemy");
        objAddUser.SetActive(true);

    }
    public void onButtonSearch_Click()
    {
        if (_selectedUserSlot == null)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_Social_ViewEmpire_NotSelectFriend"), UINoticeManager.NoticeType.Message);
            return;
        }

        if (_selectedUserSlot._gameUser == null) return;

        UIEmpireManager.isFriend = true;
        UIEmpireManager.friendLand = _selectedUserSlot._gameUser.Opponent.Land;
        GameScenes.ChangeScense(GameScenes.MyScene.Social, GameScenes.MyScene.Empire);

    }
    public void onButtonRemove_Click()
    {
        if (_selectedUserSlot == null) return;
        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(OnConfrimDeleteUser);
        if (_friendActiveType == FriendActiveType.AddFriend)
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Social_Dialog_DeleteConfirm"), 
                _selectedUserSlot._gameUser.Opponent.Base.NickName.Trim(), 
                GameManager.localization.GetText("Social_Tab_Friend")), UINoticeManager.NoticeType.YesNo);
        }
        else
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Social_Dialog_DeleteConfirm"),
                _selectedUserSlot._gameUser.Opponent.Base.NickName.Trim(),
                GameManager.localization.GetText("Social_Tab_Enemy")), UINoticeManager.NoticeType.YesNo);
        }

    }
    public void onButtonInfo_Click()
    {

    }
    public void onButtonRevenge_Click()
    {
        if (_selectedReportSlot == null)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_Social_Revenge_NotSelectMail"), UINoticeManager.NoticeType.Message);          
            return;
        }
        Debug.Log("onButtonRevenge_Click");


        int mailID = _selectedReportSlot._report.id;
        PvALog log = GameManager.GameUser.PvALogs.FirstOrDefault(p => p.LogId == mailID);
        if (log != null)
        {
            _controller.SendRevenge(log.Opponent.Id);
        }
        else
        {
            PvPLog pvpLog = GameManager.GameUser.PvPLogs.FirstOrDefault(p => p.LogId == mailID);
            if (pvpLog != null)
            {
                _controller.SendRevenge(pvpLog.Opponent.Id);
            }
        }

    }
    public void onButtonSort_Click()
    {
        if (_selectedReportSlot == null && _selectedSystemSlot == null)
            return;
        Debug.Log("onButtonView_Click");

        objMailDetail.gameObject.SetActive(true);
        if (_selectedSystemSlot != null)
            objMailDetail.SetDetail(_selectedSystemSlot._userMail);
        if (_selectedReportSlot != null)
            objMailDetail.SetDetail(_selectedReportSlot._report);
    }
    public void onButtonDelete_Click()
    {
        if (_selectedReportSlot == null && _selectedSystemSlot == null)
            return;
        Debug.Log("onButtonDelete_Click");


        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(OnConfrimDeleteMail);

        MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Social_Dialog_DeleteConfirmMail")), UINoticeManager.NoticeType.YesNo);


    }
    public void onButtonExit_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Social, GameScenes.MyScene.WorldMap);
    }    
    public void onButtonTakeAll_Click()
    {
    }
    public void onButtonDeleteDeail_Click()
    {
    }
    public void onButtonExitDetail_Click()
    {

        objMailDetail.gameObject.SetActive(false);
    }
    public void OnButtonAddUserAdd_Click()
    {            
        if (curSubStatus == SubStatus.Enemy)
        {
            if (GameManager.GameUser.UserFriends.FirstOrDefault(p => p.Opponent.Base.NickName.ToUpper().Trim() == inputUser.value.ToUpper().Trim() && p.Mode == 0) != null)
            {
                string s = string.Format(GameManager.localization.GetText("Social_Dialog_ExitsFriend"), inputUser.value.Trim(), localizationLabel.lblEnemy.text);
                MessageBox.ShowDialog(s, UINoticeManager.NoticeType.Message);
                return;
            }
            _friendActiveType = FriendActiveType.AddEnemy;
            _controller.SendRequestAddEnemy(inputUser.value);          
        }
        else
        {
            if (GameManager.GameUser.UserFriends.FirstOrDefault(p => p.Opponent.Base.NickName.ToUpper().Trim() == inputUser.value.ToUpper().Trim() && p.Mode != 0) != null)
            {
                string s = string.Format(GameManager.localization.GetText("Social_Dialog_ExitsFriend"), inputUser.value.Trim(), localizationLabel.lblFriend.text);
                MessageBox.ShowDialog(s, UINoticeManager.NoticeType.Message);
                return;
            }

            _friendActiveType = FriendActiveType.AddFriend;
            _controller.SendRequestAddFriend(inputUser.value);        
        }
        _curNicknameAdd = inputUser.value;
        objAddUser.SetActive(false);
    }
    public void OnButtonAddUserCancel_Click()
    {
        objAddUser.SetActive(false);
    }
    public void OnButtonMailAdd_Click()
    {
        if (_selectedReportSlot == null) return;
        objMailAddUser.SetActive(true);
    }
    public void OnTabChoseEnemy_Click()
    {
        string nickname = _selectedReportSlot._report.nickName.Trim();
        if (GameManager.GameUser.UserFriends.FirstOrDefault(p => p.Opponent.Base.NickName.ToUpper().Trim() == nickname.ToUpper() && p.Mode == 0) != null)
        {
            string s = string.Format(GameManager.localization.GetText("Social_Dialog_ExitsFriend"), inputUser.value.Trim(), localizationLabel.lblEnemy.text);
            MessageBox.ShowDialog(s, UINoticeManager.NoticeType.Message);
            return;
        }
        _friendActiveType = FriendActiveType.AddEnemy;
        _controller.SendRequestAddEnemy(nickname);
        _curNicknameAdd = nickname;

        OnTabChoseExit_Click();
    }
    public void OnTabChosePlayer_Click()
    {
        string nickname = _selectedReportSlot._report.nickName.Trim();
        if (GameManager.GameUser.UserFriends.FirstOrDefault(p => p.Opponent.Base.NickName.ToUpper().Trim() == nickname.ToUpper() && p.Mode != 0) != null)
        {
            string s = string.Format(GameManager.localization.GetText("Social_Dialog_ExitsFriend"), inputUser.value.Trim(), localizationLabel.lblFriend.text);
            MessageBox.ShowDialog(s, UINoticeManager.NoticeType.Message);
            return;
        }
        _friendActiveType = FriendActiveType.AddFriend;
        _controller.SendRequestAddFriend(nickname);
        _curNicknameAdd = nickname;

        OnTabChoseExit_Click();
    }
    public void OnTabChoseExit_Click()
    {
        objMailAddUser.SetActive(false);
    }
    #endregion

 
}
