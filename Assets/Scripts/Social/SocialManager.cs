using UnityEngine;
using System.Collections;
using System.IO;

public class SocialManager : MonoBehaviour {
    //CAAF68krFORsBAL2wUCn4iMZCIjdCLbMNKUWYWdjooYoP9ALxQBsDvGmGUvPGZA746UF0ZA7nusZCYoZBAfHqZCkvrPxDV63cIbiQwP6ZAtxkPbPv6RaxNXOz2WJEa4tHqlDWP1gj2PZCkxzIvKuJ6v0igLu20rgkDpBJcyiGner8iFoGkFjfGqhynEZCSQNwZBojkOnGOPjCJgQ3mucCD093Tp

    #region event
    public delegate void EventHandle();
    public event EventHandle OnLoginSuccess;
    public event EventHandle OnLoginFail;
    public event EventHandle OnShareFail;
    public event EventHandle OnShareSuccess;
    #endregion

    public static SocialManager Instance { get; private set; }

    private FacebookManager facebookManager;
    private bool _isConnected;
    private bool _isLogin { get { return FB.IsLoggedIn; } }

	void Start () {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        facebookManager = new FacebookManager(this);
        _isConnected = false;
    }
	
	void Update () {

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(TakeScreenshot());
        }
	}

    #region public methods
    public void Connect()
    {
        if (!facebookManager.isInit)
            facebookManager.Connect();
    }
    public void ShareScreenShot()
    {
        if (!_isLogin)
        {
            OnLoginSuccess += ShareScreenShot;
            Connect();
        }
        else
        {
            Debug.Log("TakeScreenshot");          
            StartCoroutine(TakeScreenshot());
        }
    }
    #endregion

    #region Response
    public void OnConnectSuccess()
    {
        facebookManager.Login();
        _isConnected = true;
    }
    public void OnLoginResponse()
    {
        if (!FB.IsLoggedIn)
        {
            Debug.Log("Login fail");
            if(OnLoginFail!=null)
                OnLoginFail.Invoke();
        }
        else
        {
            Debug.Log("Login success");
            if (OnLoginSuccess != null)
            {
                OnLoginSuccess.Invoke();
                OnLoginSuccess -= OnLoginSuccess;
            }
        }
    }
    #endregion

    #region Private
    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();


        byte[] screenshot = tex.EncodeToPNG();

        var wwwForm = new WWWForm();
        //wwwForm.AddBinaryData("image", screenshot, "LeagueOfLords.png");
        wwwForm.AddBinaryData("source", screenshot, "LeagueOfLords.png","image/png");
        FB.API("me/photos", Facebook.HttpMethod.POST, Callback, wwwForm);

        //string url = "https://graph.facebook.com/me/photos?access_token=" + FB.AccessToken;

        //var w = new WWW(url, wwwForm);

        //yield return w;

        //if (string.IsNullOrEmpty(w.error))
        //{
        //    Debug.Log("Picture posted to facebook");
        //    MessageBox.ShowDialog("Share picture success", UINoticeManager.NoticeType.Message);
        //    if (OnShareSuccess != null)
        //    {
        //        OnShareSuccess.Invoke();
        //        OnShareSuccess -= OnShareSuccess;
        //    }
        //}
        //else
        //{
        //    Debug.LogError(w.error);
        //    MessageBox.ShowDialog(w.error, UINoticeManager.NoticeType.Message);
        //}
    }

    private void Callback(FBResult result)
    {
        if (result.Error != null)
        {
            Debug.LogWarning("FacebookManager-publishActionCallback: error: " + result.Error);
            MessageBox.ShowDialog(result.Text, UINoticeManager.NoticeType.Message);
            if (OnShareSuccess != null)
            {
                OnShareSuccess.Invoke();
                OnShareSuccess -= OnShareSuccess;
            }
        }
        else
        {
            Debug.Log("FacebookManager-publishActionCallback: success: " + result.Text);
            MessageBox.ShowDialog("Share picture success", UINoticeManager.NoticeType.Message);
            if (OnShareFail != null)
            {
                OnShareFail.Invoke();
                OnShareFail -= OnShareFail;
            }
        }
    }    
    private void SaveTextureToFile(Texture2D texture, string fileName)
    {
        var bytes = texture.EncodeToPNG();
        var file = File.Open(Application.dataPath + "/" + fileName, FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
    }
    #endregion
}
