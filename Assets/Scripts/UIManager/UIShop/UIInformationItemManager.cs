using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Linq;

public class UIInformationItemManager : MonoBehaviour {

    [System.Serializable]
    public struct UIMainStats
    {
        public UILabel uiName;
        public UILabel uiValue;
    }

    public UITexture uiIcon;
    public UILabel uiInformation;
    public UIMainStats[] uiMainStats;
    public UILabel uiAttribute;
    public UILabel uiSellValue;
    public UITexture uiBorder;
    public UISprite line2;

    private UserItem _userItem;


    public void SetItem(UserItem item)
    {
        _userItem = item;

//        ItemGrade itemGrade = (ItemGrade)_userItem.Grade;

        Color color = Helper.ItemColor[_userItem.Grade];

        //Debug.Log("itemGrade " + _userItem.Grade + " " + color.ToString());

        uiBorder.color = color;

        if (_userItem.GameItem.Kind == (int)ItemKind.Material || _userItem.GameItem.Kind == (int)ItemKind.Consume)
            uiIcon.mainTexture = Helper.LoadTextureForSupportItem(_userItem.ItemId);
        else
            uiIcon.mainTexture = Helper.LoadTextureForEquipItem(_userItem.ItemId);
        

        string EquipedForRole = "";

        if (GameScenes.currentSence == GameScenes.MyScene.Hero || GameScenes.currentSence == GameScenes.MyScene.ItemUpgrade)
        {
            if (_userItem.RoleUId > 0)
            {
                UserRole role = GameManager.GameUser.UserRoles.FirstOrDefault(p => p.Id == _userItem.RoleUId);
                if (role != null)
                    EquipedForRole = string.Format("[FFFF00]({0})[-]", role.Name);
            }
        }

        string rank = "";
        /*if (_userItem.Rank > 0)
        {
            rank = string.Format("(+{0})", _userItem.Rank);
        }*/

        uiInformation.text = string.Format("[{0}]{1}[-]{2} {3}\n" +
            GameManager.localization.GetText("Global_Level") + "{4}\n" +
            GameManager.localization.GetText("Global_Type") + " {5}", Helper.ColorToHex(color), _userItem.Name, rank, EquipedForRole,
            Helper.GetLevelItem(_userItem.GameItem.Level), (ItemKind)_userItem.GameItem.Kind);


        uiAttribute.text = "";

        int i = 0;
        foreach (ItemAttrib att in _userItem.Attribs)
        {
            if (i < 2)
            {
                FormatMainStatText(att.Value, att.Attrib, uiMainStats[i]);
            }
            else
            {
                string lineColor = Helper.ColorToHex(Helper.ItemColor[att.Grade]);
                uiAttribute.text += FormatAttributeText(lineColor, att.Value, att.Attrib); 
            }
            i++;
        }

        if (_userItem.Attribs.Count <= 2)
            line2.gameObject.SetActive(false);
        else
            line2.gameObject.SetActive(true);

       /* ShopItem shopItem = (ShopItem)GameManager.ChargeShop.FirstOrDefault(p => ((ShopItem)p).ItemKind == (ItemKind)_userItem.GameItem.Kind && ((ShopItem)p).ItemId == _userItem.ItemId);
     
        uiSellValue.text = string.Format("{0:0}", shopItem.PriceSilver / 10.0f);*/
    }

    public void FormatMainStatText(float value, AttribType type, UIMainStats uiAtt)
    {
        string attname = "";
        attname = Helper.FormatAtributeText(type);
        uiAtt.uiName.text = string.Format(attname, value);
    }

    public string FormatAttributeText(string lineColor, float value, AttribType type)
    {
        string result = "";
        string attname = "";

        attname = Helper.FormatAtributeText(type);
        attname = "[" + lineColor + "]" + attname + "[-]\n";
        result = string.Format(attname, value);
   
        return result;
    }

    public void OnButtonClose_Click()
    {
        gameObject.SetActive(false);
    }

    public void OnButtonEquip_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.ChargeShop, GameScenes.MyScene.Hero);
    }
}
