using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIHireHeroSlot : MonoBehaviour {

    public UserRoleHire userRoleHire;
    public UIHireHeroManager hireManager;
    public UITexture quality;
    public UITexture icon;
    public UISprite roleClass;
    public UIHeroStarManager starManager;
    public UILabel lblName;
    public UILabel lblLevel;
    public GameObject selected;
    public UILabel lblNickName;

    public void InitUser(UserRoleHire _userRoleHire,UIHireHeroManager _hireManager)
    {
        userRoleHire = _userRoleHire;
        hireManager = _hireManager;

        quality.mainTexture = Helper.LoadTextureElement((int)userRoleHire.ElemId);
        icon.mainTexture = Helper.LoadTextureForHero(userRoleHire.RoleId);
        starManager.SetStart(userRoleHire.Grade);
        lblName.text = userRoleHire.Name;
        lblLevel.text = GameManager.localization.GetText("Global_Lvl") + userRoleHire.RoleLevel;    
        roleClass.spriteName = Helper.GetSpriteNameOfRoleClass((RoleClass)userRoleHire.Class);
        lblNickName.text = userRoleHire.NickName;
    }

    public void OnSlected()
    {
        if (userRoleHire.UserId == GameManager.GameUser.Id) return;
        selected.SetActive(true);
        hireManager.OnSelectedSlot(this);
    }

    public void OnDeSlected()
    {
        selected.SetActive(false);
    }
}
