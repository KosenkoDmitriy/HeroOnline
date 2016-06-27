using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;
using DEngine.Common.Config;

public class UIHeroSlotManager : MonoBehaviour {

    public UILabel LBLNameHero;
    public UITexture IMGHero;
    public UIButton HeroSlot;
    public UISprite UIDisbale;
    public UIToggle btnToggle;

    public UserRole userRole { get; set; }
    public UIHeroManager uiHeroManager {get;set;}

    public void SetGameRole(UserRole _userRole)
    {
        userRole = _userRole;
       // Debug.Log("SetGameRole " + userRole);
        LBLNameHero.text = userRole.Name;
        IMGHero.mainTexture = Helper.LoadTextureForHero(userRole.Base.RoleId);


        SetStatusRoleUI();

        if (userRole.Base.Energy < RoleConfig.ENERGY_MIN)
        {
            UIDisbale.gameObject.SetActive(true);
        }
    }

    public void OnClick()
    {
        uiHeroManager.OnClickSlot(this);
        HeroSlot.normalSprite = "hero_view_over";
    }

    public void OnToggleActiveHero()
    {

        if (btnToggle.value)
        {
            if (userRole.Base.Energy < RoleConfig.ENERGY_MIN)
            {
                MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_NotEnoughtEnergy"), UINoticeManager.NoticeType.Message);
                btnToggle.value = false;
                return;
            }

            int activeHeroCount = GameManager.GameUser.UserRoles.Count(p => p.Base.Status == RoleStatus.Active);
            
            if (activeHeroCount >= 3)
            {
                MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_FullHeroEquip"), UINoticeManager.NoticeType.Message);
                btnToggle.value = false;
                return;
            }

            userRole.Base.Status = RoleStatus.Active;

          

        }
        else
        {
            //Debug.Log("OnToggleActiveHero " + btnToggle.value);
            userRole.Base.Status = RoleStatus.Default;
            btnToggle.value = false;
        }
       
        uiHeroManager.OnChangedStatusRole(userRole);
    }

    public void OnDeselect()
    {
        HeroSlot.normalSprite = "hero_view";
    }

    public void SetStatusRoleUI()
    {
        //Debug.Log("SetStatusRoleUI");
             
        if (userRole.Base.Status == RoleStatus.Active)
        {
            btnToggle.startsActive = true;
        }
        else
        {
            btnToggle.startsActive = false;
        }
    }
}
