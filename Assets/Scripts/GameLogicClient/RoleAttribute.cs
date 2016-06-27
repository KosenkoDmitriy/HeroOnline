using UnityEngine;
using System.Collections;

[System.Serializable]
public class RoleAttribute
{
    public int roleID { get; set; }
    public float strength { get; set; }
    public float agility { get; set; }
    public float intelligent { get; set; }

    public float maxHP { get; set; }
    public float maxMP { get; set; }

    public float physicAttack { get; set; }
    public float physicDefence { get; set; }

    public float attackSpeed { get; set; }

    public float critPower { get; set; }
    public float critRate { get; set; }
    public float magicAttack { get; set; }
    public float magicDefence { get; set; }
    
    public float evasion { get; set; }
    public float hitRate { get; set; }
    public float hpRegen { get; set; }
    public float mpRegen { get; set; }
    public float moveMentSpeed { get; set; }
    public float range { get; set; }

    public RoleAttribute(int _roleID)
    {
        roleID = _roleID;
    }

    public void SetBaseAttribute(int _str, int _agi, int _int)
    {
        strength = _str;
        agility = _agi;
        intelligent = _int;
        Update();
    }

    public void Update()
    {
        maxHP = strength * 50;
        physicDefence = strength * 2f;
        critPower = 1.5f + (strength * 0.05f);

        attackSpeed = 1.5f - (agility * 0.05f);
        physicAttack = 10 + (agility * 10);
        critRate = 3 + (agility * 0.5f);

        magicAttack = intelligent * 10;
        magicDefence = intelligent * 2f;
        maxMP = intelligent * 30;

        evasion = 5;
        hitRate = 75;
        hpRegen = 1;
        mpRegen = 1;

        switch((RoleType)roleID)
        {
            case RoleType.Shaolin:
                range = 6;
                break;
            case RoleType.Hulk:
                range = 1;
                break;
            case RoleType.Valkyrie:
                range = 6;
                break;
            case RoleType.Hecate:
                range = 6;
                break;
            case RoleType.Earthquake:
                range = 1;
                break;
            case RoleType.Gunner:
                range = 6;
                break;
            case RoleType.WoodElf:
                range = 6;
                break;           
            case RoleType.Songoku:
                range = 1.5f;
                break;
            case RoleType.Garuda:
                range = 1;
                break;
            default:
                break;
        }
    }
}

public enum RoleType
{
    None,
    Shaolin,
    Hulk,
    Valkyrie,
    Hecate,
    Earthquake,
    Gunner,
    WoodElf,
    Songoku,
    Garuda,
    Building

}