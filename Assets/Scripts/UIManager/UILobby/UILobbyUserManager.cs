using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UILobbyUserManager : MonoBehaviour {

    public UILabel label;
    [HideInInspector]
    public GameUser userMini;

    [HideInInspector]
    public UILobbyManager uiLobbyManager;

    public UITexture icon;
    public GameObject selectedUserIcon;

    public UISprite iconBattle;
    public UIButton btnInviteBattle;


    void Start()
    {
        icon.mainTexture = Helper.LoadTextureForAvatar(userMini.Base.Avatar);
       /* if(((GameUserClient)(userMini.Tag)).avatar ==null)
            uiLobbyManager.StartCoroutine(getImageFormFace());
        else
            icon.mainTexture = ((GameUserClient)(userMini.Tag)).avatar;*/
    }

    public void Refresh()
    {      
        //if (userMini.Status == UserStatus.InBattle)
        //{
        //    iconBattle.gameObject.SetActive(true);
        //    btnInviteBattle.gameObject.SetActive(false);
        //}
        //else
        //{
        //    iconBattle.gameObject.SetActive(false);
        //    btnInviteBattle.gameObject.SetActive(true);
        //}
    }

    void OnDestroy()
    {
        CancelInvoke("Refresh");
    }

    void OnClick()
    {
        uiLobbyManager.OnSelectedUser(this);
        selectedUserIcon.SetActive(true);
    }

    IEnumerator getImageFormFace()
    {
        string path = "https" + "://graph.facebook.com/" + userMini.Name + "/picture?type=small";
        //Debug.Log(path);
        WWW url = new WWW(path);
        yield return url;

        try
        {         
            Texture2D avatar = new Texture2D(128, 128, TextureFormat.DXT1, false);
            url.LoadImageIntoTexture(avatar);
            icon.mainTexture = avatar;
            ((GameUserClient)(userMini.Tag)).avatar = avatar;
        }
        catch
        {
            Debug.Log("Can't download avatar");
        }
    }

    public void OnDeSelect()
    {
        selectedUserIcon.SetActive(false);
    }

    public void OnButtonInviteBattle_Click()
    {
        //Debug.Log("OnButtonInviteBattle_Click");
        uiLobbyManager.SendRequestInviteToBattle(userMini);
    }

    public void SetUser(GameUser _gameUser)
    {
        userMini =_gameUser;
        label.text = string.Format("{0}", userMini.Base.NickName);
    }
}
