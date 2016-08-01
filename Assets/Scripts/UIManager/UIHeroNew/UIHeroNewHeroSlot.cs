using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;
using System.Linq;

public class UIHeroNewHeroSlot : MonoBehaviour {

    public UITexture icon;
    public UISprite grade;
    public GameObject active;
    public GameObject selected;
    public GameObject disable;
    public UISprite element;
    public UISprite uiClass;
    public UILabel lblLevel;
    public UserRole userRole { get; set; }
    public UIHeroStarManager starManager;
    public GameObject hireIcon;
    private UIHeroNewManager _manager;

    public void SetHero(UserRole role, UIHeroNewManager manager)
    {
        userRole = role;
        _manager = manager;
        Show();
    }

    public void Selected()
    {
        if (_manager != null)
            _manager.OnSelectedHeroSlot(this);
        selected.SetActive(true);
    }

    public void DeSelected()
    {
        selected.SetActive(false);
    }

    internal void Refresh()
    {
        Show();
    }

    private void Show()
    {
        icon.mainTexture = Helper.LoadTextureForMiniHero(userRole.Base.RoleId);

        string gradeName = Helper.GetSpriteNameElement(userRole);
        grade.spriteName = gradeName;
        starManager.SetStart(userRole.Base.Grade);

        if (userRole.Base.Status == RoleStatus.Active)
            active.SetActive(true);
        else
            active.SetActive(false);

        if (userRole.Base.Energy < RoleConfig.ENERGY_MIN)
        {
            disable.SetActive(true);
        }
        else
        {
            disable.SetActive(false);
        }

        element.spriteName = Helper.GetSpriteNameOfElement(userRole.Base.ElemId);
        uiClass.spriteName = Helper.GetSpriteNameOfRoleClass(userRole.Base.Class);

        lblLevel.text = GameManager.localization.GetText("Global_Lvl") + userRole.Base.Level;

        if (GameManager.GameUser.HireRoles.Count > 0)
        {
            UserRoleHire hireRole = GameManager.GameUser.HireRoles.FirstOrDefault(p => p.Id == userRole.Id);
            if (hireRole != null)
            {
                if (hireRole.HireMode != 0)
                {
                    hireIcon.gameObject.SetActive(true);
                }
                else
                {
                    hireIcon.gameObject.SetActive(false);
                }
            }
            else
            {
                hireIcon.gameObject.SetActive(false);
            }
        }
    }


}
