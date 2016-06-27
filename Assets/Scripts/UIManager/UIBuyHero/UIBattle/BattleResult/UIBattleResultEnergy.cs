using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;

public class UIBattleResultEnergy : MonoBehaviour {

    public UISprite element;
    public UITexture heroIcon;
    public UILabel lblEnergy;
    
    private UserRole _role;

    public void SetRole(UserRole role)
    {
        _role = role;

        element.spriteName = Helper.GetSpriteNameElement(_role);
        heroIcon.mainTexture = Helper.LoadTextureForMiniHero(_role.Base.RoleId);

        lblEnergy.text = string.Format("{0}/{1}", _role.Base.Energy, RoleConfig.ENERGY_MAX);
    }
}
