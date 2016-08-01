using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIHeroUpgradeInfoStat : MonoBehaviour
{

    public UITexture qualityHeroBG;
    public UITexture icon;
    public UILabel lblName;
    public UILabel lblLvel;
    public UILabel lblClass;
    public UILabel lblHP;
    public UILabel lblMana;
    public UILabel lblPAttack;
    public UILabel lblMAttack;
    public UILabel lblPDef;
    public UILabel lblMDef;
    public UILabel lblARate;
    public UILabel lblStr;
    public UILabel lblAgi;
    public UILabel lblInt;
    public UILabel lblspeed;

    public UITexture uiSkill1;
    public UITexture uiSkill2;

    public UISkillInformationManager uiSkill;
    public UIHeroStarManager starManager;

    private UserRole role;

    public void ShowItem(UserRole _userRole)
    {
        role = _userRole;
        qualityHeroBG.mainTexture = Helper.LoadTextureElement((int)_userRole.Base.ElemId);
        icon.mainTexture = Helper.LoadTextureForHero(role.Id);
        lblName.text = string.Format("{0}", role.Name);
        lblClass.text = string.Format(GameManager.localization.GetText("Global_Class") + "{0} ", (RoleClass)role.GameRole.Class);

        lblHP.text = _userRole.Attrib.MaxHP.ToString("0");
        lblMana.text = _userRole.Attrib.MaxMP.ToString("0");
        lblPAttack.text = _userRole.Attrib.AttackValue.ToString("0");
        lblPDef.text = _userRole.Attrib.DefenceValue.ToString("0");
        //lblMAttack.text = _userRole.Attrib.MAttack.ToString("0");
        //lblMDef.text = _userRole.Attrib.MDefence.ToString("0");
        lblspeed.text = _userRole.Attrib.MoveSpeed.ToString("0");
        lblARate.text = _userRole.Attrib.AttackSpeed.ToString("0");

        starManager.SetStart(_userRole.Base.Grade);

        uiSkill1.mainTexture = Helper.LoadTextureForSkill(_userRole.RoleSkills[1].SkillId);
        uiSkill2.mainTexture = Helper.LoadTextureForSkill(_userRole.RoleSkills[1].SkillId);
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
}
