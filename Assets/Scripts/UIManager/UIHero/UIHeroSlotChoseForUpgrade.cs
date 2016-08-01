using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIHeroSlotChoseForUpgrade : MonoBehaviour {

    public UITexture texture;
    public UITexture border;
    public UIHeroStarManager starManager;
    public UILabel lblName;
    public UILabel lblLevel;
    public GameObject selected;

    public UserRole _userRole;
    private MonoBehaviour _manager;

    public void SetUser(UserRole userRole, MonoBehaviour manager)
    {
        _userRole = userRole;
        _manager = manager;

        lblName.text = _userRole.Name;
        lblLevel.text = GameManager.localization.GetText("Global_Lvl") + _userRole.Base.Level;
        starManager.SetStart(_userRole.Base.Grade);
        border.mainTexture = Helper.LoadTextureElement((int)_userRole.Base.ElemId);
        texture.mainTexture = Helper.LoadTextureForHero(_userRole.Base.RoleId);
    }

    public void OnSelected()
    {
        if (_manager is UIUpStarManager)
            ((UIUpStarManager)_manager).OnSelectedRoleForUpgrade(this);
        else
            ((UIHeroUpgradeManager)_manager).OnSelectedRoleForUpgrade(this);
        selected.SetActive(true);
    }

    public void OnDeSelected()
    {
        selected.SetActive(false);
    }
}
