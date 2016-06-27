using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIStatsManager : MonoBehaviour {

    public UILabel lblHP;
    public UILabel lblMP;
    public UILabel lblHPRegen;
    public UILabel lblMPRegen;
    public UILabel lblPhysical_attack;
    public UILabel lblPhysical_defense;
    public UILabel lblMagical_attack;
    public UILabel lblMagical_defense;
    public UILabel lblAtkSpeed;
    public UILabel lblHitRate;
    public UILabel lblEvade;
    public UILabel lblMovement_speed;
    public UILabel lblCritical_Power;
    public UILabel lblCritical_Rate;

    public void SetRole(UserRole oldRole, UserRole newRole)
    {
        int HP = newRole.Attrib.MaxHP - oldRole.Attrib.MaxHP;
        int MP = newRole.Attrib.MaxMP - oldRole.Attrib.MaxMP;
        int HPRegen = newRole.Attrib.HPRegen - oldRole.Attrib.HPRegen;
        int MPRegen = newRole.Attrib.MPRegen - oldRole.Attrib.MPRegen;
        int Physical_attack = newRole.Attrib.AttackValue - oldRole.Attrib.AttackValue;
        int Physical_defense = newRole.Attrib.DefenceValue - oldRole.Attrib.DefenceValue;
        //int Magical_attack = newRole.Attrib.MAttack - oldRole.Attrib.MAttack;
        //int Magical_defense = newRole.Attrib.MDefence - oldRole.Attrib.MDefence;
        int AtkSpeed = newRole.Attrib.AttackSpeed - oldRole.Attrib.AttackSpeed;
        float HitRate = newRole.Attrib.HitRate - oldRole.Attrib.HitRate;
        float Evade = newRole.Attrib.EvasRate - oldRole.Attrib.EvasRate;
        int Movement_speed = newRole.Attrib.MoveSpeed - oldRole.Attrib.MoveSpeed;
        float Critical_Power = newRole.Attrib.CritPower - oldRole.Attrib.CritPower;
        float Critical_Rate = newRole.Attrib.CritRate - oldRole.Attrib.CritRate;

        lblHP.text = string.Format("+{0}", HP);
        lblMP.text = string.Format("+{0}", MP);

        if (HPRegen != 0)
            lblHPRegen.text = string.Format("+{0}", HPRegen);
        else
            lblHPRegen.transform.parent.gameObject.SetActive(false);

        if (MPRegen != 0)
            lblMPRegen.text = string.Format("+{0}", MPRegen);
        else
            lblMPRegen.transform.parent.gameObject.SetActive(false);

        lblPhysical_attack.text = string.Format("+{0}", Physical_attack);
        lblPhysical_defense.text = string.Format("+{0}", Physical_defense);
        //lblMagical_attack.text = string.Format("+{0}", Magical_attack);
        //lblMagical_defense.text = string.Format("+{0}", Magical_defense);
        lblAtkSpeed.text = string.Format("+{0}", AtkSpeed);

        if (HitRate != 0)
            lblHitRate.text = string.Format("+{0}", HitRate);
        else
            lblHitRate.transform.parent.gameObject.SetActive(false);

        if (Evade != 0)
            lblEvade.text = string.Format("+{0}", Evade);
        else
            lblEvade.transform.parent.gameObject.SetActive(false);

        lblMovement_speed.text = string.Format("+{0}", Movement_speed);

        if (Critical_Power != 0)
            lblCritical_Power.text = string.Format("+{0:0.0}%", Critical_Power);
        else
            lblCritical_Power.transform.parent.gameObject.SetActive(false);

        if (Critical_Rate != 0)
            lblCritical_Rate.text = string.Format("+{0:0.0}%", Critical_Rate);
        else
            lblCritical_Rate.transform.parent.gameObject.SetActive(false);


        transform.GetChild(0).GetComponent<UIGrid>().Reposition();
    }
	
}
