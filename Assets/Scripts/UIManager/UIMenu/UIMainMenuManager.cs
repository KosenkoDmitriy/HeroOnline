using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using System;
using DEngine.Common.GameLogic;

public class UIMainMenuManager : MonoBehaviour
{
    public UILabel lblStart;
  
    void Awake()
    {
        GameManager.Init();
    }

    void Start()
    {
      
        SoundManager.Instance.PlayAmbient("SoundMission");
        GameScenes.currentSence = GameScenes.MyScene.MainMenu;          
    }

    void OnDestroy()
    {            
    }

    void Update()
    {
        //MyInput.CheckInput();
    }

    #region button click   

    //Select English language
    public void OnEnglishLanguage_Click()
    {
        Global.language = Global.Language.ENGLISH;
        UIMusicSetting.Instance.Localization();
        GameScenes.ChangeScense(GameScenes.MyScene.MainMenu, GameScenes.MyScene.ServerGame);
    }
    //Select VietNam language
    public void OnVietNamLanguage_Click()
    {
        Global.language = Global.Language.VIETNAM;
        UIMusicSetting.Instance.Localization();
        GameScenes.ChangeScense(GameScenes.MyScene.MainMenu, GameScenes.MyScene.ServerGame);
    }
   

    public void OnButtonSetting_Click()
    {
        if (MessageBox.isShow) return;
    }

    public void OnButtonCredit_Click()
    {
        if (MessageBox.isShow) return;
    }

    #endregion

   
    
   
}
