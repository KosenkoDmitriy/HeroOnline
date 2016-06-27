using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UISkillInformationManager : MonoBehaviour {

    public UITexture texture;
    public UILabel lblName;
    public UILabel lblType;
    public UILabel lblCost;
    public UILabel lblRange;
    public UILabel lblDes;

    public void SetSkill(UserRole role, int skillIndex, Texture icon)
    {        
        int skillID = role.RoleSkills[skillIndex].SkillId;

        MyLocalization.SkillInfo skill = GameManager.localization.getSkill(skillID);

        if (icon != null)
            texture.mainTexture = icon;
        else
            texture.mainTexture = Helper.LoadTextureForSkill(skillID);

        lblName.text = skill.Name;
        lblType.text = GameManager.localization.GetText("Shop_SkillType") + " " + skill.Type;
        lblCost.text = GameManager.localization.GetText("Shop_SkillCost") + " " + role.RoleSkills[skillIndex].CostValue + " " + skill.CostType;
        lblRange.text = GameManager.localization.GetText("Shop_SkillRange") + " " + skill.Range;
        lblDes.text = "[i]" + skill.Desc + "[-]";

    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    
}
