using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIRandomHeroSkill : MonoBehaviour {


    public UITexture uiSkillDef;
    public UITexture uiSkill1;
    public UITexture uiSkill2;

    public UISkillInformationManager uiSkill;
    private UserRole role;

    public void SetRole(UserRole _userRole)
    {
        role = _userRole;
        uiSkillDef.mainTexture = Helper.LoadTextureForSkill(role, 0);
        uiSkill1.mainTexture = Helper.LoadTextureForSkill(role, 1);
        uiSkill2.mainTexture = Helper.LoadTextureForSkill(role, 2);
    }

    public void OnClickSkill1()
    {
     
        uiSkill.SetSkill(role, 1, uiSkill1.mainTexture);
        uiSkill.gameObject.SetActive(true);
    }

    public void OnClickSkill2()
    {
        
        uiSkill.SetSkill(role, 2, uiSkill2.mainTexture);
        uiSkill.gameObject.SetActive(true);
    }

    public void OnClickSkillDefine()
    {
        
        uiSkill.SetSkill(role, 0, uiSkillDef.mainTexture);
        uiSkill.gameObject.SetActive(true);
    }

}
