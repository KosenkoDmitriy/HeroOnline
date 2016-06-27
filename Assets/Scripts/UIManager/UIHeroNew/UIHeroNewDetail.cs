using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIHeroNewDetail : MonoBehaviour {

    [System.Serializable]
    public struct UIHero
    {
        public UISprite quality;
        public UITexture icon;  
        public UILabel lblName;
        public UILabel lblLevel;
        public UILabel lblGrade;
        public UIHeroStarManager starManage;
        public UILabel lblClass;
        public UILabel lblElement;
        public UISprite element;
        public UISprite roleClass;
                
        public UILabel lblHP;
        public UILabel lblDamage;
        public UILabel lblAccuracy;
        public UILabel lblCriticleRate;
        public UILabel lblAttackRate;

        public UILabel lblMP;
        public UILabel lblDefence;
        public UILabel lblEvas;
        public UILabel lblCriticlePower;
        public UILabel lblMoveSpeed;

        public UILabel lblHPRegen;
        public UILabel lblMPRegen;
        public UILabel lblBlockRate;
        public UILabel lblFeedbackDamage;
        public UILabel lblIgnorDefence;

        public UILabel lblDefenceMetal;
        public UILabel lblDefenceWood;
        public UILabel lblDefenceWater;
        public UILabel lblDefenceFire;
        public UILabel lblDefenceEarth;
    }

    public UIHero uiHero;



    public void SetHero(UserRole userRole)
    {
        uiHero.icon.mainTexture = Helper.LoadTextureForMiniHero(userRole.Base.RoleId);
        string gradeName = Helper.GetSpriteNameElement(userRole);
        uiHero.quality.spriteName = gradeName;

        uiHero.lblName.text = string.Format(GameManager.localization.GetText("HeroNew_Name"), userRole.Name); 
        uiHero.lblLevel.text = GameManager.localization.GetText("Global_Level") + userRole.Base.Level.ToString();


        Color colorGrade = Helper.ItemColor[userRole.Base.Grade];
        string grade = GameManager.localization.GetText("Global_Grade");// string.Format("{0}[{1}]{2}[-]",
            //GameManager.localization.GetText("Global_Grade"),
            //Helper.ColorToHex(colorGrade),
            //GameManager.localization.GetText("Global_Grade" + userRole.Base.Grade));

        uiHero.lblGrade.text = grade;
        uiHero.starManage.SetStart(userRole.Base.Grade);
        uiHero.lblClass.text = GameManager.localization.GetText("Global_Class") + userRole.Base.Class;


        string hpAdd = "";
        string damageAdd = "";
        string accuracyAdd = "";
        string criticleRateAdd = "";
        string attackRateAdd = "";
        string mpAdd = "";
        string Defence = "";
        string Evas = "";
        string CriticlePower = "";
        string MoveSpeed = "";
        string HPRegen = "";
        string MPRegen = "";
        string BlockRate = "";
        string FeedbackDamage = "";
        string IgnorDefence = "";
        string DefenceMetal = "";
        string DefenceWood = "";
        string DefenceWater = "";
        string DefenceFire = "";
        string DefenceEarth = "";




        uiHero.lblHP.text = string.Format(GameManager.localization.GetText("HeroNew_HP"), userRole.Attrib.MaxHP, hpAdd);
        uiHero.lblDamage.text = string.Format(GameManager.localization.GetText("HeroNew_Damage"), userRole.Attrib.AttackValue, damageAdd);
        uiHero.lblAccuracy.text = string.Format(GameManager.localization.GetText("HeroNew_Accuracy"), userRole.Attrib.HitRate, accuracyAdd);
        uiHero.lblCriticleRate.text = string.Format(GameManager.localization.GetText("HeroNew_CriticleRate"), userRole.Attrib.CritRate, criticleRateAdd);
        uiHero.lblAttackRate.text = string.Format(GameManager.localization.GetText("HeroNew_AttackSpeed"), userRole.Attrib.AttackSpeed, attackRateAdd);

        uiHero.lblMP.text = string.Format(GameManager.localization.GetText("HeroNew_MP"), userRole.Attrib.MaxMP, mpAdd);
        uiHero.lblDefence.text = string.Format(GameManager.localization.GetText("HeroNew_Defence"), userRole.Attrib.DefenceValue, Defence);
        uiHero.lblEvas.text = string.Format(GameManager.localization.GetText("HeroNew_Evas"), userRole.Attrib.EvasRate, Evas);
        uiHero.lblCriticlePower.text = string.Format(GameManager.localization.GetText("HeroNew_CriticlePower"), userRole.Attrib.CritPower, CriticlePower);
        uiHero.lblMoveSpeed.text = string.Format(GameManager.localization.GetText("HeroNew_MovementSpeed"), userRole.Attrib.MoveSpeed, MoveSpeed);

        uiHero.lblHPRegen.text = string.Format(GameManager.localization.GetText("HeroNew_HPRegen"), userRole.Attrib.HPRegen, HPRegen);
        uiHero.lblMPRegen.text = string.Format(GameManager.localization.GetText("HeroNew_MPRegen"), userRole.Attrib.MPRegen, MPRegen);
        uiHero.lblBlockRate.text = string.Format(GameManager.localization.GetText("HeroNew_Block"), userRole.Attrib.BlockRate, BlockRate);
        uiHero.lblFeedbackDamage.text = string.Format(GameManager.localization.GetText("HeroNew_FeedbackDamage"), userRole.Attrib.FeedbackDamage, FeedbackDamage);
        uiHero.lblIgnorDefence.text = string.Format(GameManager.localization.GetText("HeroNew_IgnorDefence"), userRole.Attrib.IgnoreDefence, IgnorDefence);

        uiHero.lblDefenceMetal.text = string.Format(GameManager.localization.GetText("HeroNew_DefenceMetal"), userRole.Attrib.DefenceElems[(int)ElemType.Metal], DefenceMetal);
        uiHero.lblDefenceWood.text = string.Format(GameManager.localization.GetText("HeroNew_DefenceWood"), userRole.Attrib.DefenceElems[(int)ElemType.Wood], DefenceWood);
        uiHero.lblDefenceWater.text = string.Format(GameManager.localization.GetText("HeroNew_DefenceWater"), userRole.Attrib.DefenceElems[(int)ElemType.Water], DefenceWater);
        uiHero.lblDefenceFire.text = string.Format(GameManager.localization.GetText("HeroNew_DefenceFire"), userRole.Attrib.DefenceElems[(int)ElemType.Fire], DefenceFire);
        uiHero.lblDefenceEarth.text = string.Format(GameManager.localization.GetText("HeroNew_DefenceEarth"), userRole.Attrib.DefenceElems[(int)ElemType.Earth], DefenceEarth);



        string element = GameManager.localization.GetText("HeroNew_Element");
        string elementLocalKey = "HeroNew_Element_" + userRole.Base.ElemId;
        uiHero.lblElement.text = element + GameManager.localization.GetText(elementLocalKey);

        uiHero.element.spriteName = Helper.GetSpriteNameOfElement(userRole.Base.ElemId);
        uiHero.roleClass.spriteName = Helper.GetSpriteNameOfRoleClass(userRole.Base.Class);
    }
}
