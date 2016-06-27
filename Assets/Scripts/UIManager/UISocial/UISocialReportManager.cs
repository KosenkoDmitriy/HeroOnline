using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public struct UIReport
{
    public int type;
    public int id;
    public string nickName;
    public string status;
    public string time;
    public string where;
}

public class UISocialReportManager : MonoBehaviour {

   


    public UILabel lblUserName;
    public UILabel lblStatus;
    public UILabel lblTime;
    public UILabel lblWhere;
    public UISprite spriteWhere;

    public GameObject selected;

    private UISocialManager _manager;

    public UIReport _report;

    public void SetReport(UISocialManager manager, UIReport report)
    {
        _manager = manager;
        _report=report;

        lblUserName.text = _report.nickName;
        lblStatus.text = _report.status;
        lblTime.text = _report.time;
        lblWhere.text = _report.where;

        if (report.type == 0)
        {
            spriteWhere.spriteName = "bac";
        }
        else
        {
            spriteWhere.spriteName = "lobby_Batle";
        }
    }

    public void OnSelected()
    {
        _manager.OnSelectedReport(this);
        selected.SetActive(true);
    }

    public void OnDeSelected()
    {
        selected.SetActive(false);
    }
}
