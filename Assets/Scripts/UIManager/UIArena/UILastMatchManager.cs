using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UILastMatchManager : MonoBehaviour {


    public UILabel lblDate;
    public UILabel lblName;
    public UILabel lblReplay;

    private PvPLog _log;
    private UIArenaManager _arena;

    public void SetLog(PvPLog log,UIArenaManager arena)
    {
        _log = log;
        _arena = arena;

        lblDate.text = log.LogTime.ToString(GameManager.localization.GetText("Arena_LastMatchFormatDate"));

        string s = "";

        if (log.Result < 0)
        {
            s = string.Format(GameManager.localization.GetText("Arena_LastMatchWin"), log.Opponent.Base.NickName);
        }
        else if (log.Result == 0)
        {
            s = string.Format(GameManager.localization.GetText("Arena_LastMatchDraw"), log.Opponent.Base.NickName);
        }
        else
        {
            s = string.Format(GameManager.localization.GetText("Arena_LastMatchLose"), log.Opponent.Base.NickName);
        }

        lblName.text = s;

        lblReplay.text = GameManager.localization.GetText("Arena_btnReplay");
    }

    public void OnButtonReplay_Click()
    {
        _arena.OnButtonReplay_Click(_log);
    }
}
