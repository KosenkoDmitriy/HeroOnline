using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIInformationItem : MonoBehaviour
{

    public UITexture texture;
    public UILabel lblInfor;
    public UILabel lblAttribute;
    public UITexture Border;

    private UserItem _roleItem;
    public void SetItem(UserItem userItem)
    {
        _roleItem = userItem;

        ItemGrade itemGrade = (ItemGrade)userItem.Grade;
        Debug.Log("SetItem " + userItem.Grade + " " + itemGrade);

        Color color = Helper.ItemColor[userItem.Grade];
        Border.color = color;

        texture.mainTexture = Helper.LoadTextureForEquipItem(userItem.ItemId);

        lblInfor.text = string.Format("[{0}]{1}\nLvl: {2}[-]\n", Helper.ColorToHex(color), userItem.Name, Helper.GetLevelItem(userItem.GameItem.Level));
        lblAttribute.text = "";
        foreach(ItemAttrib att in _roleItem.Attribs)
        {
            lblAttribute.text += string.Format("[00F099]{0}[-]: {1}\n", att.Attrib.ToString(), att.Value);
        }
    }



}
