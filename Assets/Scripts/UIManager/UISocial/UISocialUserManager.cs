using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UISocialUserManager : MonoBehaviour
{

    public UILabel lblName;
    public GameObject selected;
    public UserFriend _gameUser;
    public UITexture avartar;
    private UISocialManager _manager;
    private bool _enemy;

    public void SetUser(UserFriend gameUser, UISocialManager manager, bool enemy)
    {
        _gameUser = gameUser;
        _manager = manager;
        _enemy = enemy;
        avartar.mainTexture = Helper.LoadTextureForAvatar(gameUser.Opponent.Base.Avatar);

        if (_gameUser != null)
        {
            lblName.text = _gameUser.Opponent.Base.NickName;
            if (_gameUser.Mode == 1)
            {
                lblName.alpha = 0.5f;
            }
            else
            {
                lblName.alpha = 1;
                lblName.color = Color.yellow;
            }
        }
    }

    public void OnSelected()
    {
        _manager.OnSelectedUser(this);
        selected.SetActive(true);
    }

    public void OnDeSelected()
    {
        selected.SetActive(false);
    }
}
