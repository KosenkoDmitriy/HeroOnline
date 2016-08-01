using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using DEngine.Common.GameLogic;

public class UILoginManager : MonoBehaviour {

    public UILabel lblLogin_Header;
    public UILabel lblLogin_Username;
    public UILabel lblLogin_Password;
	public UILabel lblLogin_btnLogin;
	public UILabel lblLogin_btnLoginWebsite;
	public UILabel lblLogin_btnRegister;
	public UILabel lblLogin_btnRegisterWebsite;

    public UILabel lblRegister_Header;
    public UILabel lblRegister_Username;
    public UILabel lblRegister_Password;
    public UILabel lblRegister_ConfirmPass;
    public UILabel lblRegister_Email;
    public UILabel lblRegister_NickName;
    public UILabel lblRegister_btnCancel;
    public UILabel lblRegister_btnRegister;

    public GameObject LoginRoot;
    public GameObject RegisterRoot;

    public UIInput txtLogin_UserName;
    public UIInput txtLogin_Password;
    public UIInput txtRegister_UserName;
    public UIInput txtRegister_Password;
    public UIInput txtRegister_ConfirmPassword;
    public UIInput txtRegister_Email;
    public UIInput txtRegister_FirstName;
    public UIInput txtRegister_LastName;

    private LoginController _LoginController;

	private string key, fullName, email, password;

    void Start()
    {
        //data manager game 
        GameManager.Init();      

        _LoginController = new LoginController(this);

        //Load text for labels button by language selected
        Localization();

         //load user and pass saved
        LoadUserPass();
    }

    void Update()
    {
        //MyInput.CheckInput();
    }

	#region Facebook Login
	void Awake ()
	{
		Debug.Log("Awake()");
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
			Debug.Log("FB.Init()");
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
			Debug.Log("FB.ActivateApp()");
		}
	}

	private void InitCallback ()
	{
		Debug.Log("InitCallback()");
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			Debug.Log("start ActivateApp()");
			FB.ActivateApp();
			Debug.Log("end ActivateApp()");
			
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		Debug.Log("OnHideUnity()");
		
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void FacebookLogin() {
		Debug.Log("FacebookLogin()");
		//var perms = new System.Collections.Generic.List<string>(){"public_profile"};
		//FB.LogInWithReadPermissions(perms, AuthCallback);
		FB.Login("email,public_profile", AuthCallback);
	}
	
	public void FacebookLogout() {
		Debug.Log("FacebookLogout()");
		FB.Logout();
	}
	
	private void AuthCallback (FBResult result) {
		//Debug.Log("AuthCallback()");
		
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var dict = Facebook.MiniJSON.Json.Deserialize(result.Text) as Dictionary<string,object>;
			string aToken = (string)dict["access_token"];
			string uId = (string)dict["user_id"];
			//bool isLogined = (bool)dict["is_logged_in"];
			//Debug.Log(string.Format("{0} err: {1} texture:{2}", result.Text, result.Error, result.Texture));

			string query = string.Format("/{0}?fields=token_for_business&access_token={1}", uId, aToken);
			//Debug.Log(query);
			var form = new WWWForm();
//			form.AddField("fields", "token_for_business");
//			form.AddField("access_token", aToken.TokenString);
			FB.API(query, Facebook.HttpMethod.GET, BusinessTokenCallback, form);
		} else {
			string msg = "User cancelled login";
			Debug.Log(msg);
		}
	}

	private void BusinessTokenCallback(FBResult result){
		//Debug.Log("BusinessTokenCallback()");
		//Debug.Log(string.Format("{0} err: {1} texture:{2}", result.Text, result.Error, result.Texture));

		var dict = Facebook.MiniJSON.Json.Deserialize(result.Text) as Dictionary<string,object>;
		string businessToken = (string)dict["token_for_business"];
		string uid = (string)dict["id"];
		//Texture2D texture = (Texture2D)dict["texture"];
		//Rect rect = new Rect(0, 0, texture.width, texture.height);
		//Global.avatar = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);

		string url = string.Format("{0}/{1}", Global.host, Global.actionFacebookLogin);
		//Debug.Log(url);
		
		WWWForm form = new WWWForm();
		form.AddField("u", businessToken);
		form.AddField("p", "facebook");
		form.AddField("r", "1");

		password = businessToken;

		WWW www = new WWW(url, form);
		StartCoroutine(WaitForLogin(www));
	}

	#endregion

    #region Button
	
	public void OnButtonBackMainMenu_Click()
	{
		GameScenes.ChangeScense(GameScenes.MyScene.Login, GameScenes.MyScene.ServerGame);
	}
		
	public void OnButtonWebsiteVisit_Click()
	{
		string url = string.Format ("{0}/{1}", Global.host, Global.actionSignUp);
		#if UNITY_WEBPLAYER || UNITY_WEBGL
		UnityEngine.Application.ExternalEval(string.Format("window.open('{0}', '_blank')", url)); // open url in new tab
		#else
		UnityEngine.Application.OpenURL(url);
		#endif
	}

	public void OnButtonWebsiteReg_Click(){
		Dictionary<byte, object> parameter = new Dictionary<byte, object>();
		parameter.Add((byte)ParameterCode.UserName, email);
		parameter.Add((byte)ParameterCode.Password, password);
		parameter.Add((byte)ParameterCode.Email, email);
		parameter.Add((byte)ParameterCode.NickName, fullName);
		_LoginController.SendRegister(parameter);
	}

	public void OnButtonWebsiteLogin_Click()
	{
		if (txtLogin_UserName.value == string.Empty || txtLogin_Password.value == string.Empty)
		{
			MessageBox.ShowDialog(GameManager.localization.GetText("Login_IncorrectUserNamePass"), UINoticeManager.NoticeType.Message);
			return;
		}

		if (txtLogin_UserName.value == string.Empty || !txtLogin_UserName.value.Contains(".") || !txtLogin_UserName.value.Contains("@"))
		{
			MessageBox.ShowDialog(GameManager.localization.GetText("Register_IncorrectEmail"), UINoticeManager.NoticeType.Message);
			return;
		}

		string url = string.Format("{0}/{1}", Global.host, Global.actionLogin);
		WWWForm form = new WWWForm();

		password = txtLogin_Password.value.Trim();
		email = txtLogin_UserName.value.Trim();

		form.AddField("e", email);
		form.AddField("p", password);
		form.AddField("r", "1");


		WWW www = new WWW(url, form);
		StartCoroutine(WaitForLogin(www));

		MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_Waiting"), UINoticeManager.NoticeType.Waiting);
	}

	public IEnumerator WaitForLogin(WWW www)
	{
		yield return www;
		// check for errors
		if (string.IsNullOrEmpty(www.error))
		{
			//Debug.Log(www.text);
			var dict = Facebook.MiniJSON.Json.Deserialize(www.text) as Dictionary<string,object>;
			key = (string)dict["key"];
			email = (string)dict["email"];
			fullName = (string)dict["full_name"];
			_LoginController.SendSignIn(email, password);
		}
		else 
		{
			string msg = ""; 
			if (string.IsNullOrEmpty(www.text))
				msg = "please login on website first and try again";
			else {
				msg = www.text;
			}
			MessageBox.ShowDialog(msg, UINoticeManager.NoticeType.Message);
		}
	}
	#endregion

	#region Disabled Buttons
	public void OnButtonLogin_Click()
	{
		if (txtLogin_UserName.value == string.Empty || txtLogin_Password.value == string.Empty)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Login_IncorrectUserNamePass"), UINoticeManager.NoticeType.Message);
            return;
        }
        _LoginController.SendSignIn(txtLogin_UserName.value, txtLogin_Password.value);

        MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_Waiting"), UINoticeManager.NoticeType.Waiting);
    }

    public void OnButtonRegister_Click()
    {
        LoginRoot.SetActive(false);
        RegisterRoot.SetActive(true);
    }

    public void OnButtonLoginFacebook_Click()
    {
        if (MessageBox.isShow) return;
    }
    
    public void OnButtonBackLogin_Click()
    {
        LoginRoot.SetActive(true);
        RegisterRoot.SetActive(false);
    }
    
    public void OnButtonRegisterConfirm_Click()
    {
        string password = txtRegister_Password.value.Trim();
        string confirmPassword = txtRegister_ConfirmPassword.value.Trim();
        string username = txtRegister_UserName.value.Trim();
        string email = txtRegister_Email.value.Trim();
        string firstName = txtRegister_FirstName.value.Trim();
        //string lastName = txtRegister_LastName.value.Trim();
        
        if (password != confirmPassword)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Register_IncorrectConfirmPassword"), UINoticeManager.NoticeType.Message);
            return;
        }

        if (username == string.Empty || password == string.Empty || firstName == string.Empty)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Register_IncorrectInformationNull"), UINoticeManager.NoticeType.Message);
            return;
        }

        if (email == string.Empty || !email.Contains(".") || !email.Contains("@"))
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Register_IncorrectEmail"), UINoticeManager.NoticeType.Message);
            return;
        }
        
        Dictionary<byte, object> parameter = new Dictionary<byte, object>();
        parameter.Add((byte)ParameterCode.UserName, username);
        parameter.Add((byte)ParameterCode.Password, password);
        parameter.Add((byte)ParameterCode.Email, email);
        parameter.Add((byte)ParameterCode.NickName, firstName);
        //parameter.Add((byte)ParameterCode.LastName, lastName);
        _LoginController.SendRegister(parameter);
    }
	#endregion

    #region Respone

    //check current step of tutorial and to world map scene
    public void OnPhotonLoginSuccess()
    {
        GameManager.tutorial.Init();
        MessageBox.CloseDialog();
        GameManager.logined = true;
        GameScenes.ChangeScense(GameScenes.MyScene.MainMenu, GameScenes.MyScene.WorldMap);

        if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
        {
        }
        else
        {
            PlayerPrefs.SetString("UsernameLOL", txtLogin_UserName.value);

            string pass = txtLogin_Password.value;
            pass = Helper.Encrypt(pass, false);
            PlayerPrefs.SetString("PassWordLOL", pass);
        }
    }

    public void OnResponseZoneList()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Login, GameScenes.MyScene.Zone);
    }

    public void OnResponeDuplicate()
    {
        MessageBox.ShowDialog(GameManager.localization.GetText("Login_DuplicateLogin"), UINoticeManager.NoticeType.Waiting);
        if (txtLogin_UserName.value == string.Empty || txtLogin_Password.value == string.Empty)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Login_IncorrectUserNamePass"), UINoticeManager.NoticeType.Message);
            return;
        }
        StartCoroutine(ReLogin());
    }
    private IEnumerator ReLogin()
    {
        yield return new WaitForSeconds(0.5f);
        _LoginController.SendSignIn(txtLogin_UserName.value, txtLogin_Password.value);
    }
    #endregion

    

    #region private methods

    private void LoadUserPass()
    {
        if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
        {
        }
        else
        {
            string username = "";
            string password = "";
            if (PlayerPrefs.HasKey("UsernameLOL"))
                username = PlayerPrefs.GetString("UsernameLOL");
            if (PlayerPrefs.HasKey("PassWordLOL"))
                password = PlayerPrefs.GetString("PassWordLOL");


            if (username != "")
            {
                txtLogin_UserName.value = username;
            }

            if (password != "")
            {

                password = Helper.Decrypt(password, false);
                txtLogin_Password.value = password;
            }
        }
    }

    private void Localization()
    {
        lblLogin_Header.text = GameManager.localization.GetText("Login_Header");
		lblLogin_Username.text = GameManager.localization.GetText("Register_Email");//Login_Username");
        lblLogin_Password.text = GameManager.localization.GetText("Login_Password");
		lblLogin_btnLogin.text = GameManager.localization.GetText("Login_btn_Login");
		lblLogin_btnLoginWebsite.text = GameManager.localization.GetText("Login_btn_Login");
		lblLogin_btnRegister.text = GameManager.localization.GetText("Login_btn_Register");
		lblLogin_btnRegisterWebsite.text = GameManager.localization.GetText("Login_btn_Register");
		lblRegister_Header.text = GameManager.localization.GetText("Register_Header");
        lblRegister_Username.text = GameManager.localization.GetText("Login_Username");
        lblRegister_Password.text = GameManager.localization.GetText("Login_Password");
        lblRegister_ConfirmPass.text = GameManager.localization.GetText("Register_Confirm_Pass");
        lblRegister_Email.text = GameManager.localization.GetText("Register_Email");
        lblRegister_NickName.text = GameManager.localization.GetText("Register_Nickname");
        lblRegister_btnCancel.text = GameManager.localization.GetText("Global_btn_Cancel");
        lblRegister_btnRegister.text = GameManager.localization.GetText("Register_btnRegister");
    }

    internal void OnRegisterSuccess()
    {
        // password = txtRegister_Password.value.Trim();
        //string username = txtRegister_UserName.value.Trim();
        //_LoginController.SendSignIn(username, password);
		OnResponeDuplicate ();
    }
    #endregion 
}
