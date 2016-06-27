using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIPacketHeroSlot : MonoBehaviour 
{
    public UITexture quality;
    public UITexture icon;
    public UISprite roleClass;
    public UIHeroStarManager starManager;
    public UILabel lblName;
    public UILabel lblLevel;
    public GameObject selected;

    public void InitUser(UserRole _userRole)
    {
        quality.mainTexture = Helper.LoadTextureElement((int)_userRole.Base.ElemId);
        icon.mainTexture = Helper.LoadTextureForHero(_userRole.Base.RoleId);
        starManager.SetStart(_userRole.Base.Grade);
        lblName.text = _userRole.Name;
        lblLevel.text = GameManager.localization.GetText("Global_Lvl") + _userRole.Base.Level;
        roleClass.spriteName = Helper.GetSpriteNameOfRoleClass((RoleClass)_userRole.Base.Class);
    }
}
