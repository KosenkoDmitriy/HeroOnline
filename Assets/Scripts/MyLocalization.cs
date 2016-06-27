using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
public class MyLocalization
{

    public class HouseInfo
    {
        public string Name;
        public string Desc;
    }

    public class EffectInfo
    {
        public string Name;
        public string Desc;
    }
    public class SkillInfo
    {
        public string Name;
        public string Type;
        public string Cost;
        public string CostType;
        public string Range;
        public string Desc;
    }

    public class ItemInfo
    {
        public string Name;
        public string Description;

        public ItemInfo()
        {
            Name = "";
            Description = "";
        }
    }

          


    private string _pathLocalization;
    private Dictionary<string, string> _dataEn;
    private Dictionary<string, string> _dataVi;


    private string _pathSkillInfo;
    private string _pathItemInfo;
    private string _pathEffectInfo;
    private string _pathHouseInfo;
    private Dictionary<int, SkillInfo> _skillInfo;
    private Dictionary<int, ItemInfo> _itemInfo;
    private Dictionary<int, EffectInfo> _effectInfo;
    private Dictionary<int, HouseInfo> _houseInfo;

    public MyLocalization()
    {
        _pathLocalization = "Text/localization";
        _pathSkillInfo = "Text/Skillintro";
        _pathItemInfo = "Text/ItemInfo";
        _pathEffectInfo = "Text/EffectInfo";
        _pathHouseInfo = "Text/HouseData";
        _dataEn = new Dictionary<string, string>();
        _dataVi = new Dictionary<string, string>();
        _skillInfo = new Dictionary<int, SkillInfo>();
        _itemInfo = new Dictionary<int, ItemInfo>();
        _effectInfo = new Dictionary<int, EffectInfo>();
        _houseInfo = new Dictionary<int, HouseInfo>();
        LoadLocalization();
        LoadSkillInfor();
        LoaditemInfor();
        LoadEffectInfor();
        LoadHouseInfo();
    }

    private void LoadHouseInfo()
    {
        TextAsset text = Resources.Load(_pathHouseInfo) as TextAsset;
        string[] lines = text.text.Split('\n');
        //Debug.Log("EffectInfo " + lines.Length);

        foreach (string line in lines)
        {
            string[] s = line.Split('\t');
            if (s.Length >= 2)
            {
                int id = int.Parse(s[0]);
                HouseInfo houseInfo = new HouseInfo();


                if (Global.language == Global.Language.VIETNAM)
                {
                    houseInfo.Name = s[2];
                    houseInfo.Desc = s[4];
                }
                else
                {
                    houseInfo.Name = s[1];
                    houseInfo.Desc = s[3];
                }

                _houseInfo[id] = houseInfo;

            }
        }
    }

    private void LoadLocalization()
    {
        TextAsset text = Resources.Load(_pathLocalization) as TextAsset;
        string[] lines = text.text.Split('\n');
        // Debug.Log("LoadLocalization " + lines.Length);

        foreach (string line in lines)
        {
            string[] s = line.Split('\t');
            //Debug.Log(line);
            if (s.Length >= 2)
            {
                _dataEn[s[0].Trim()] = s[1];

                if (s.Length >= 3)
                {
                    _dataVi[s[0].Trim()] = s[2];
                    // Debug.Log(s[0] + " =" + s[2]);
                }
            }
        }
    }
    private void LoadSkillInfor()
    {
        TextAsset text = Resources.Load(_pathSkillInfo) as TextAsset;
        string[] lines = text.text.Split('\n');
        //Debug.Log("Skillintro " + lines.Length);

        foreach (string line in lines)
        {
            string[] s = line.Split('\t');
            if (s.Length >= 2)
            {
                int id = int.Parse(s[0]);
                SkillInfo skill = new SkillInfo();

                if (Global.language != Global.Language.VIETNAM)
                {
                    skill.Name = s[1];
                    skill.Type = s[3];
                    skill.Desc = s[8];
                }
                else
                {
                    skill.Name = s[2];
                    skill.Type = s[4];
                    skill.Desc = s[9];
                }

                skill.Cost = s[5];
                skill.CostType = s[6];
                skill.Range = s[7];


                _skillInfo[id] = skill;

                /*  Debug.Log(string.Format("{0} {1} {2} {3} {4} {5} {6}",
                     id, _skillInfo[id].Name, _skillInfo[id].Type, _skillInfo[id].Cost, _skillInfo[id].CostType,
                      _skillInfo[id].Range, _skillInfo[id].Desc));*/
            }
        }
    }
    private void LoaditemInfor()
    {
        TextAsset text = Resources.Load(_pathItemInfo) as TextAsset;
        string[] lines = text.text.Split('\n');
        // Debug.Log("itemInfo " + lines.Length);

        foreach (string line in lines)
        {
            string[] s = line.Split('\t');
            if (s.Length >= 2)
            {
                int id = int.Parse(s[0]);
                ItemInfo itemInfo = new ItemInfo();

                if (Global.language != Global.Language.VIETNAM)
                {
                    itemInfo.Name = s[1];
                    itemInfo.Description = s[3];
                }
                else
                {
                    itemInfo.Name = s[2];
                    itemInfo.Description = s[4];
                }

                _itemInfo[id] = itemInfo;

                /*  Debug.Log(string.Format("{0} {1} {2} {3} {4} {5} {6}",
                     id, _skillInfo[id].Name, _skillInfo[id].Type, _skillInfo[id].Cost, _skillInfo[id].CostType,
                      _skillInfo[id].Range, _skillInfo[id].Desc));*/
            }
        }
    }

    private void LoadEffectInfor()
    {
        TextAsset text = Resources.Load(_pathEffectInfo) as TextAsset;
        string[] lines = text.text.Split('\n');
        //Debug.Log("EffectInfo " + lines.Length);

        foreach (string line in lines)
        {
            string[] s = line.Split('\t');
            if (s.Length >= 2)
            {
                int id = int.Parse(s[0]);
                EffectInfo effectInfo = new EffectInfo();

                if (Global.language != Global.Language.VIETNAM)
                {
                    effectInfo.Name = s[1];
                    effectInfo.Desc = s[2];
                }
                else
                {
                    effectInfo.Name = s[1];
                    effectInfo.Desc = s[3];
                }

                _effectInfo[id] = effectInfo;

            }
        }
    }

    public string GetText(string key)
    {
        if (Global.language != Global.Language.VIETNAM)
        {
            if (_dataEn.ContainsKey(key))
                return _dataEn[key];

        }
        else
        {
            if (_dataVi.ContainsKey(key))
                return _dataVi[key];
        }
        Debug.Log(key);
        Debug.LogWarning("NULL " + key);
        return string.Empty;
    }

    public SkillInfo getSkill(int skillID)
    {
        if (_skillInfo.ContainsKey(skillID))
            return _skillInfo[skillID];
        Debug.LogError("NULL " + skillID);
        return null;
    }

    public ItemInfo getItem(int itemID)
    {
        if (_itemInfo.ContainsKey(itemID))
            return _itemInfo[itemID];

        Debug.LogError("NULL " + itemID);
        return new ItemInfo();
    }

    public EffectInfo getEffectInfo(int effectID)
    {
        if (_effectInfo.ContainsKey(effectID))
            return _effectInfo[effectID];

        Debug.LogError("NULL Effect " + effectID);
        return new EffectInfo();
    }

    public HouseInfo getHouseInfo(int houseID)
    {
        if (_houseInfo.ContainsKey(houseID))
            return _houseInfo[houseID];

        Debug.LogError("NULL House " + houseID);
        return new HouseInfo();
    }
}
