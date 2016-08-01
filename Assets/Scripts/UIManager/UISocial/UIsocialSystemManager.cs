using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIsocialSystemManager : MonoBehaviour {

    public UILabel lblHeader;
    public UILabel lblTime;
    public GameObject selected;

    public UserMail _userMail;
    private UISocialManager _manager; 

    public void SetSystem(UISocialManager manager, UserMail userMail)
    {
        _manager = manager;
        _userMail = userMail;
        lblTime.text = userMail.SendTime.ToString(GameManager.localization.GetText("Arena_LastMatchFormatDate"));
        lblHeader.text = _userMail.Title;
    }

    public void OnSelected()
    {
        _manager.OnSelectedSytem(this);
        selected.SetActive(true);
    }

    public void OnDeSelected()
    {
        selected.SetActive(false);
    }
}
