using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIHeroUpgradeHeroSlot : MonoBehaviour {

    public GameObject selected;
    public UITexture icon;
    public UITexture quality;
    public bool IsSelected;
    public UIHeroStarManager uistar;
    public UILabel lblLevel;
    public UILabel lblName;


    public UserRole userRole;
    private UIHeroUpgradeManager _manager;

    public void SetRole(UserRole role, UIHeroUpgradeManager manager)
    {
        _manager = manager;
        userRole = role;
        IsSelected = false;
        if (userRole != null)
        {
            icon.mainTexture = Helper.LoadTextureForHero(role.Base.RoleId);
            quality.mainTexture = Helper.LoadTextureElement((int)role.Base.ElemId);
            uistar.SetStart(role.Base.Grade);
            lblLevel.text = GameManager.localization.GetText("Global_Lvl") + role.Base.Level;
            lblName.text = role.Name;
        }
    }   

    public void OnSelected()
    {
        if (_manager.IsLock()) return;

        if (!IsSelected)
        {
            //Check trang bi....

            if (_manager.feeds.Count >= 5)
            {
                _manager.OnShowWarning(GameManager.localization.GetText("Hero_Upgrade_More5Slot"), this);
                return;
            }
           else if (CheckEquipItem())
            {
                _manager.OnShowWarning(GameManager.localization.GetText("Hero_Upgrade_EquipItem"), this);
                return;
            }
            else if (userRole.Base.Grade >= 3)
            {
                _manager.OnShowYesNo(GameManager.localization.GetText("Hero_Upgrade_AreYouSureSelectedHero"), this);
                return;
            }
            else
            {
                OnOKButtonClick();
            }
        }
        else
        {
            IsSelected = false;
            selected.SetActive(false);
            _manager.OnSelectedHero(userRole, IsSelected);
        }
    }

    public void OnOKButtonClick()
    {
        IsSelected = true;
        selected.SetActive(true);
        _manager.OnSelectedHero(userRole, IsSelected);
    }

    private bool CheckEquipItem()
    {
        if (GameManager.GameUser.UserItems.FirstOrDefault(p => p.RoleUId == userRole.Id) != null) return true;
        return false;
    }

}
