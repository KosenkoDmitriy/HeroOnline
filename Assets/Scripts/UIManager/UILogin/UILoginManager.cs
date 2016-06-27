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
    public UILabel lblLogin_btnRegister;
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


    #region Button
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

    public void OnButtonBackMainMenu_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Login, GameScenes.MyScene.ServerGame);
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
        lblLogin_Username.text = GameManager.localization.GetText("Login_Username");
        lblLogin_Password.text = GameManager.localization.GetText("Login_Password");
        lblLogin_btnLogin.text = GameManager.localization.GetText("Login_btn_Login");
        lblLogin_btnRegister.text = GameManager.localization.GetText("Login_btn_Register");
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
        string password = txtRegister_Password.value.Trim();
        string username = txtRegister_UserName.value.Trim();
        _LoginController.SendSignIn(username, password);
    }
    #endregion 
}
