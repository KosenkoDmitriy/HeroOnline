using UnityEngine;
using System.Collections;

public class FacebookManager 
{
 

    public IDictionary requestFormFacebook;

    public bool isInit = false;

    private SocialManager _socialManager;

    public FacebookManager(SocialManager socialManager)
    {
        _socialManager = socialManager;
    }

    public void Connect()
    {
        FB.Init(OnInitComplete, OnHideUnity);
    }

    public void Login()
    {
        if (!FB.IsLoggedIn)
        {
            FB.Login("email,publish_actions,user_friends", LoginCallback);//user_likes,email, "email,publish_actions"
        }
        else
        {
            _socialManager.OnLoginResponse();
           // FB.API("me?fields=id,first_name,last_name", Facebook.HttpMethod.GET, UserCallBack);//"me?fields=id,email,first_name,last_name"
        }

    }

    public void GetInformation()
    {
        FB.API("me?fields=id,email,first_name,last_name", Facebook.HttpMethod.GET, UserCallBack);
    }
       
    #region Facebook Response

    private void OnInitComplete()
    {
        Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
        isInit = true;
        _socialManager.OnConnectSuccess();
    }

    private void OnHideUnity(bool isGameShown)
    {
        Debug.Log("Is game showing? " + isGameShown);
    }

    private void LoginCallback(FBResult result)
    {
        _socialManager.OnLoginResponse();
    }

    private void UserCallBack(FBResult result)
    {
        string get_data;
        if (result.Error != null)
        {
            get_data = result.Text;
            Debug.Log(get_data);
        }
        else
        {
            get_data = result.Text;
            requestFormFacebook = Facebook.MiniJSON.Json.Deserialize(get_data) as IDictionary;
            Debug.Log(get_data);
        }

    }

    #endregion


}
