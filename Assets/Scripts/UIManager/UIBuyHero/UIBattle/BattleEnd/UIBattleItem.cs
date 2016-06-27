using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIBattleItem : MonoBehaviour {

    public UISprite background;
    public UITexture texture;
    public UILabel lblAmount;
    public UISprite chest;

    private int grade;
    private float waitingTime;
    public void SetItem(int id, int amount, int index)
    {

        Texture2D icon = null;

        icon = Helper.LoadTextureForSupportItem(id);

        if (icon == null)
            icon = Helper.LoadTextureForEquipItem(id);


        texture.mainTexture = icon;
        string s = "x" + amount;
        if (icon == null)
            s = id.ToString();
        lblAmount.text = s;

        background.gameObject.SetActive(false);
        texture.gameObject.SetActive(false);
        lblAmount.gameObject.SetActive(false);
        chest.gameObject.SetActive(true);

        grade = ((GameItem)(GameManager.GameItems[id])).Level;

        showChest();

        waitingTime = index * 0.7f + 1;
        Invoke("OnOpenChest", waitingTime);
    }
    private void showChest()
    {
        string spriteName = "";
        if (grade == 3)
        {
            spriteName = "ruong_vang";
        }
        else if (grade == 2)
        {
            spriteName = "ruong_bac";
        }
        else
        {
            spriteName = "ruong_gho";
        }
        chest.spriteName = spriteName;
    }


    private void OnOpenChest()
    {
        string spriteName = "";
        if (grade == 3)
        {
            spriteName = "ruong_vang_open";
        }
        else if (grade == 2)
        {
            spriteName = "ruong_bac_open";
        }
        else 
        {
            spriteName = "ruong_gho_open";
        }
        chest.spriteName = spriteName;
        Invoke("ShowItem", 0.5f);
    }

    private void ShowItem()
    {
        background.gameObject.SetActive(true);
        texture.gameObject.SetActive(true);
        lblAmount.gameObject.SetActive(true);
        chest.gameObject.SetActive(false);
    }

}
