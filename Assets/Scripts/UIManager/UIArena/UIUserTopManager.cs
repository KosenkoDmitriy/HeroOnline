using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIUserTopManager : MonoBehaviour {

    public UILabel lblRank;
    public UILabel lblUsername;
    public UILabel lblHonor;
    public GameObject selectedBorder;

    public UISprite topIcon;

    private UIArenaManager _manager;
    public GameUser _miniUser;

    public void SetUser(GameUser miniUser, UIArenaManager manager)
    {
        _manager = manager;
        _miniUser = miniUser;

        lblRank.text = _miniUser.Base.HonorRank.ToString();
        lblUsername.text = _miniUser.Base.NickName;
        lblHonor.text = _miniUser.Base.Honor.ToString();

        topIcon.gameObject.SetActive(true);
        if (_miniUser.Base.HonorRank == 1)
            topIcon.spriteName = "Top1";
        else if (_miniUser.Base.HonorRank == 2)
            topIcon.spriteName = "Top2";
        else if (_miniUser.Base.HonorRank == 3)
            topIcon.spriteName = "Top3";
        else
            topIcon.gameObject.SetActive(false);
    }

    public void OnButton_Click()
    {
        _manager.OnSlected(this);
    }

    public void OnSelected()
    {
        selectedBorder.SetActive(true);
    }

    public void OnDeSelected()
    {
        selectedBorder.SetActive(false);
    }

    public void OnButtonAttack_Click()
    {
        _manager.OnClickAttack(this);
    }
}
