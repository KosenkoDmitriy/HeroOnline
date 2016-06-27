using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;

public class UIBattleResultNewHeroSlot : MonoBehaviour {

    public UISprite element;
    public UITexture heroIcon;
    public UIProgressBar expBar;
    public UISprite expThump;
    public UILabel lblExp;

    private UserRole _role;

    public void SetRole(UserRole role)
    {
        _role = role;

        element.spriteName = Helper.GetSpriteNameElement(_role);
        heroIcon.mainTexture = Helper.LoadTextureForMiniHero(_role.Base.RoleId);

        expBar.value = (float)_role.Base.Exp / (float)RoleConfig.LEVELS_EXP[_role.Base.Level];
    }

    public void ShowThump()
    {
        expThump.gameObject.SetActive(true);
    }

    public void HideThump()
    {
        expThump.gameObject.SetActive(false);
    }
    public void AddExp(int exp)
    {
        _role.AddExp((int)exp);
        expBar.value = (float)_role.Base.Exp / (float)RoleConfig.LEVELS_EXP[_role.Base.Level];
    }
}
