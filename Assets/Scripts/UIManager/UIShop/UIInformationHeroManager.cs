using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIInformationHeroManager : MonoBehaviour {

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
    public UISprite element;
    public UISprite roleClass;
    public UILabel lblElement;

    public UITexture uiSkillDef;
    public UITexture uiSkill1;
    public UITexture uiSkill2;

    public UISkillInformationManager uiSkill;
    public UIHeroStarManager starManager;

    public MonoBehaviour manager;

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


        string elementLocalKey = "HeroNew_Element_" + _userRole.Base.ElemId;
        lblElement.text = GameManager.localization.GetText("HeroNew_Element") + GameManager.localization.GetText(elementLocalKey);

        element.spriteName = Helper.GetSpriteNameOfElement(_userRole.Base.ElemId);
        roleClass.spriteName = Helper.GetSpriteNameOfRoleClass(_userRole.Base.Class);

        starManager.SetStart(_userRole.Base.Grade);

        uiSkillDef.mainTexture = Helper.LoadTextureForSkill(role, 0);
        uiSkill1.mainTexture = Helper.LoadTextureForSkill(role, 1);
        uiSkill2.mainTexture = Helper.LoadTextureForSkill(role, 2);
    }
      
    public void OnClickSkill1()
    {
        if (manager as UIRandomHeroManager)
            ((UIRandomHeroManager)manager).OnButtonSkill1_Click();

        if (manager as UIHeroManager)
            ((UIHeroManager)manager).OnButtonSkill1_Click();

        uiSkill.SetSkill(role, 1, uiSkill1.mainTexture);
        uiSkill.gameObject.SetActive(true);        
    }

    public void OnClickSkill2()
    {
        if (manager as UIRandomHeroManager)
            ((UIRandomHeroManager)manager).OnButtonSkill2_Click();

        if (manager as UIHeroManager)
            ((UIHeroManager)manager).OnButtonSkill2_Click();

        uiSkill.SetSkill(role, 2, uiSkill2.mainTexture);
        uiSkill.gameObject.SetActive(true);
    }

    public void OnClickSkillDefine()
    {
        if (manager as UIRandomHeroManager)
            ((UIRandomHeroManager)manager).OnButtonSkill1_Click();

        if (manager as UIHeroManager)
            ((UIHeroManager)manager).OnButtonSkill1_Click();

        uiSkill.SetSkill(role, 0, uiSkillDef.mainTexture);
        uiSkill.gameObject.SetActive(true);
    }

    public void OnExit()
    {
        if (manager as UIRandomHeroManager)
            ((UIRandomHeroManager)manager).OnButtonExitHeroInfo_Click();

        if (manager as UIHeroManager)
            ((UIHeroManager)manager).OnButtonExitHeroInfo_Click();
    }
}
