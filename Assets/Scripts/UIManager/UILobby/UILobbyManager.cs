using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;
using DEngine.Common.Config;

public class UILobbyManager : MonoBehaviour
{

    [System.Serializable]
    public struct HeroSlot
    {
        public UILabel lblName;
        public UILabel lblLevel;
        public UITexture textIcon;
    }

    public enum LobbyState
    {
        Chat,
        Looby
    }

  
    public GameObject lobbyRoot;
    public GameObject chatRoot;

    public GameObject lobbyUserPrab;
    public GameObject lobbyUserRoot;
    public UIPanel panelLobby;
    public UILabel lblNotification;
    public UIChatWindow uiChatWindow;

    public UITexture avatar;
    public UILabel lblNickName;
    public UILabel lblLevel;
    public UILabel lblExp;
    public HeroSlot[] heroSlot;
    public GameObject avatarSetRoot;
    public GameObject avatarPrefab;

    private LobbyMenuController _controller;
    private LobbyState _state;
    private UILobbyUserManager _selectedUser;
    private static UILobbyManager _instance;

    public Dictionary<int, GameObject> _ListUIUserInZone;
    public static UILobbyManager Instance { get { return _instance; } }

    public static float timerChallenge = -1;

    void Start()
    {
        _instance = this;
        _controller = new LobbyMenuController(this);
        //Debug.Log("Avatar " + GameManager.GameUser.Avatar);
        avatar.mainTexture = Helper.LoadTextureForAvatar(GameManager.GameUser.Base.Avatar);
        lblNickName.text = string.Format("{0}", GameManager.GameUser.Base.NickName);
        lblLevel.text = GameManager.localization.GetText("Global_Level") + GameManager.GameUser.Base.Level;

        if (GameManager.GameUser.Base.Level < UserConfig.LEVEL_MAX)
        {
            lblExp.text = GameManager.localization.GetText("Global_btn_Exp") + string.Format(": {0}/{1}", GameManager.GameUser.Base.Exp, UserConfig.LEVELS_EXP[GameManager.GameUser.Base.Level]);
        }
        else
        {
            lblExp.text = "";
        }

        InitAvatarSet();
        StartCoroutine(InitListPlayerInZoneUI());

    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Update()
    {
        //MyInput.CheckInput();

      /*  if (timerChallenge != -1)
        {
            float dt = Time.time - timerChallenge;
            float m = dt / 60.0f;
            string time = string.Format("{0:00}:{1:00}", (int)m, (int)dt % 60);
            lblNotification.text = string.Format(TextScript.LookingForBattle, time);
        }*/
    }

    #region buttonClick
    public void OnButtonChat_Click()
    {
        if (MessageBox.isShow) return;

        OnChangeState(LobbyState.Chat);
    }

    public void OnButtonLobby_Click()
    {
        if (MessageBox.isShow) return;

        OnChangeState(LobbyState.Looby);
    }

    public void OnButtonBack_Click()
    {
        if (MessageBox.isShow) return;

        _controller.SendZonesLeave();

        GameScenes.ChangeScense(GameScenes.MyScene.Lobby, GameScenes.MyScene.Zone);
    }

    public void OnButtonShop_Click()
    {
        if (MessageBox.isShow) return;

        GameScenes.ChangeScense(GameScenes.MyScene.Lobby, GameScenes.MyScene.ChargeShop);
    }

    public void OnButtonRanking_Click()
    {
        if (MessageBox.isShow) return;

        GameScenes.ChangeScense(GameScenes.MyScene.Lobby, GameScenes.MyScene.Ranking);
    }

    public void OnButtonHero_Click()
    {
        if (MessageBox.isShow) return;

        GameScenes.ChangeScense(GameScenes.MyScene.Lobby, GameScenes.MyScene.Hero);
    }

    public void OnButtonPVP_Click()
    {
        if (MessageBox.isShow) return;


        if (!GameManager.CheckHeroToBattle()) return;

        timerChallenge = Time.time;
        _controller.SendRandomBattle();

        uiChatWindow.OnReciveChat(0, "[FF0000]Looking For Battle[-]");
    }

    public void OnButtonPVA_Click()
    {
        if (MessageBox.isShow) return;


        if (!GameManager.CheckHeroToBattle()) return;
        
        _controller.SendPVAMode();

    }

    public void OnButtonPVE_Click()
    {
        if (MessageBox.isShow) return;
        GameScenes.ChangeScense(GameScenes.MyScene.Lobby, GameScenes.MyScene.PVEMap);
    }

    public void OnSelectedUser(UILobbyUserManager uiUser)
    {
      
    }
    #endregion

    #region response

    public void OnUserEnterZone(int userID, GameUser userMini)
    {
        int totalUser = _ListUIUserInZone.Count;
        _ListUIUserInZone.Add(userMini.Id, createUserUI(totalUser, userMini));
    }

    public void OnUserExitZone(int userID, GameUser userMini)
    {
        NGUITools.Destroy(_ListUIUserInZone[userID]);       
        _ListUIUserInZone.Remove(userID);

        lobbyUserRoot.GetComponent<UIGrid>().Reposition();
    }

    public void OnResponseFullUserInformation(int userID, GameUser fullUser)
    {      

    }

    public void OnResponseInviteToBattle()
    {
    }

    #endregion

    #region privateMethods

    private void InitAvatarSet()
    {
        //  for (int i = 0; i <= 50; i++)
        // {
        //    GameObject go = NGUITools.AddChild(avatarSetRoot, avatarPrefab);
        //      UIAvatarController avatar = go.GetComponent<UIAvatarController>();
        //avatar.setAvatar(i, this);
        //     go.name = "Avatar_" + i.ToString("00");
        // }
        //  avatarSetRoot.GetComponent<UIGrid>().Reposition();
    }

    private void SetUserRolesUI(GameUser fullUser)
    {
        int heroIndex = 0;
        for (int index = 0; index < 3; index++)
        {
            //Debug.Log("index " + index);

            bool isfound = false;
            while (heroIndex < fullUser.UserRoles.Count)
            {
                UserRole userRole = fullUser.UserRoles[heroIndex];
                heroIndex++;
                if (userRole != null)
                {
                    if (userRole.Base.Status == RoleStatus.Active)
                    {
                        //Debug.Log("index " + index + " " + userRole.Name);
                        heroSlot[index].lblLevel.text = string.Format("Level {0}", userRole.Base.Level);
                        heroSlot[index].lblName.text = userRole.Name;
                        heroSlot[index].textIcon.mainTexture = Helper.LoadTextureForHero(userRole.Base.RoleId);
                        isfound = true;
                        break;
                    }
                }
            }

            if (!isfound)
            {
                heroSlot[index].lblLevel.text = "";
                heroSlot[index].lblName.text = "";
                heroSlot[index].textIcon.mainTexture = null;
            }

        }
    }

    private IEnumerator RefreshListUserUI()
    {
        for (int i = 0; i < _ListUIUserInZone.Count; i++)
        {
            if (_ListUIUserInZone.Values.ToList<GameObject>()[i] != null)
                _ListUIUserInZone.Values.ToList<GameObject>()[i].transform.localPosition = new Vector3(0, -(i * 85 + i * 3), 0);
            yield return 0;
        }
    }

    private IEnumerator InitListPlayerInZoneUI()
    {
       /* _ListUIUserInZone = new Dictionary<int, GameObject>();
        List<GameObj> gamUsersInZone = GameManager.ZoneUsers.ToList<GameObj>();
        for (int i = 0; i < gamUsersInZone.Count; i++)
        {
            GameUser gameUser = (GameUser)gamUsersInZone[i];
            if (gameUser.Id != GameManager.GameUser.Id)
                _ListUIUserInZone.Add(gameUser.Id, createUserUI(i, gameUser));
            yield return 0;
        }    */
        yield return 0;
    }

    private GameObject createUserUI(int userCount, GameUser gameUser)
    {
        GameObject go = NGUITools.AddChild(lobbyUserRoot, lobbyUserPrab);
       // go.transform.localPosition = new Vector3(0, -(userCount * 85 + userCount * 3), 0);
        go.name = gameUser.Id.ToString();

        UILobbyUserManager uiLobbyUserManager = go.GetComponent<UILobbyUserManager>();
        uiLobbyUserManager.uiLobbyManager = this;
        uiLobbyUserManager.SetUser(gameUser);
        lobbyUserRoot.GetComponent<UIGrid>().Reposition();
        return go;
    }
    
    #endregion

    #region publicMethods
    public void OnChangeState(LobbyState state)
    {
        _state = state;
        switch (_state)
        {
            case LobbyState.Chat:
                lobbyRoot.SetActive(false);
                chatRoot.SetActive(true);
                break;
            case LobbyState.Looby:
                lobbyRoot.SetActive(true);
                chatRoot.SetActive(false);
                break;
        }
    }

    public void SendChat(string message)
    {
        _controller.SendChat(0, message);
    }

    public void SendRequestInviteToBattle(GameUser userMini)
    {
        if (MessageBox.isShow) return;


        if (!GameManager.CheckHeroToBattle()) return;


        //MessageBox.ShowDialog(TextScript.Dialog_Waiting, UINoticeManager.NoticeType.Waiting);
        _controller.SendRequestInviteToBattle(userMini);

    }

    public void OnSelectAvatar(int ID)
    {
        GameManager.GameUser.Base.Avatar = ID;
        avatar.mainTexture = Helper.LoadTextureForAvatar(ID);
        _controller.SendChangeAvatar(ID);
    }
    #endregion
}
