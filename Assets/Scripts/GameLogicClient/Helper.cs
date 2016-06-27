using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common;
using System;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;


public class Helper
{
  //  public static float[] LEVELS_USER_EXP = new float[] {  40, 240, 720, 1600, 3000, 5040, 7840, 11520, 16200, 22000 };
   // public static float[] LEVELS_Hero_EXP = new float[] {  80, 240, 480, 800, 1200, 1680, 2240, 2880, 3600, 4400, 999999 };

    public static Color[] ItemColor = new Color[6]
    {
         Color.white,
         Color.white,
         Color.green,
         new Color(0f,0.5f, 1f,1.0f),
         Color.yellow,        
         new Color(0.722f, 0.525f, 0.043f,1.0f)

    };

    public static Dictionary<int, string> ShopItemToPurchasableItemID = new Dictionary<int, string>()
    {
        {25, "gold20"},
        {26, "gold50"},
        {27, "gold100"},
        {28, "gold200"},
        {29, "gold500"},
        {30, "silver25000"}
    };


    public static string[] ElementSpriteName = { "None", "Metal", "Wood", "Water", "Fire", "Earth" };
    public static string[] RoleClassSpriteName = { "None", "Warior", "Tanker", "Assasin", "Range", "Elf", "Sharpshooter", "Range", "Sorceres", "Healer" };

    
    public static String ColorToHex(Color c)
    {        
        return NGUIText.EncodeColor(c);
    }

    public static Texture2D LoadTextureForAvatar(int id)
    {
        string iconPath = string.Format("Avatar/Avatar_{0:D2}",  id);
        Texture2D icon = Resources.Load(iconPath) as Texture2D;
        return icon;
    }

    public static Texture2D LoadTextureForSkill(int skillID)
    {
        string iconPath = string.Format("SkillIcons/Skill_{0:0000}", skillID);
        //.Log(iconPath);
        Texture2D icon = Resources.Load(iconPath) as Texture2D;
        return icon;
    }
       
    public static Texture2D LoadTextureForSkill(UserRole role, int skillIndex)
    {
        if (role.RoleSkills.Count <= 0) return null;
        return LoadTextureForSkill(role.RoleSkills[skillIndex].SkillId);
    }

    public static Texture2D LoadTextureForHero(int id)
    {
        string iconPath = "HeroIcons";
        Texture2D icon = Resources.Load(string.Format("{0}/Icon_{1:D4}", iconPath, id)) as Texture2D;       
        return icon;
    }

    public static Texture2D LoadTextureForMiniHero(int id)
    {
        string iconPath = "HeroIcons";
        Texture2D icon = Resources.Load(string.Format("{0}/MiniIcon_{1:D4}", iconPath, id)) as Texture2D;
        return icon;
    }

    public static string GetSpriteNameElement(UserRole role)
    {
        return "Grade_" + (int)role.Base.ElemId;
    }

    public static Texture2D LoadTextureElement(int elementID)
    {       
        string iconPath = "HeroIcons";
        Texture2D icon = Resources.Load(string.Format("{0}/Element_{1:D2}", iconPath, elementID)) as Texture2D;
        return icon;
    }

    public static Texture2D LoadTextureForEquipItem(int ItemId)
    {
        GameItem gameItem = (GameItem)GameManager.GameItems[ItemId];
        if (gameItem == null) return null;

        string iconPath = string.Format("EquipmentIcons/{0}_{1:D2}", ((ItemKind)gameItem.Kind).ToString(), gameItem.Level);

        Texture2D icon = Resources.Load(iconPath) as Texture2D;
        return icon;
    }

    public static int GetLevelItem(int level)
    {
        return (level - 1) * 5 + 1;
    }

    public static Texture2D LoadTextureForSupportItem(int ItemId)
    {
        //GameItem gameItem = (GameItem)GameManager.GameItems[ItemId];
        //if (gameItem == null) return null;

        string iconPath = string.Format("SupportIcons/Support_{0:D4}", ItemId);//((ItemKind)gameItem.Kind).ToString(), gameItem.Id);

        Texture2D icon = Resources.Load(iconPath) as Texture2D;
        return icon;
    }

    public static Texture2D LoadTextureSilver()
    {        
        string iconPath = string.Format("SupportIcons/Silver");
        Texture2D icon = Resources.Load(iconPath) as Texture2D;
        return icon;
    }

    public static Texture2D LoadTextureGold()
    {
        string iconPath = string.Format("SupportIcons/Gold");
        Texture2D icon = Resources.Load(iconPath) as Texture2D;
        return icon;
    }

    public static Sprite LoadSpriteForBuilding(int id)
    {
        string iconPath = string.Format("Images/Buidings/Building_{0:D3}", id);
        Sprite icon = Resources.Load<Sprite>(iconPath);
        return icon;
    }


    public static string FormatAtributeText(AttribType type)
    {
        return GameManager.localization.GetText("Att_" + type.ToString());        
    }

    public static string GetSpriteNameOfElement(ElemType type)
    {
        return ElementSpriteName[(int)type];
    }

    public static string GetSpriteNameOfRoleClass(RoleClass roleClass)
    {
        return RoleClassSpriteName[(int)roleClass];
    }

    public static string GetTagEnemy(Controller controller)
    {
        if (controller == null) return GameManager.EnemyTagName;
        if (controller.tag == GameManager.EnemyTagName)
            return GameManager.PlayerTagName;
        return GameManager.EnemyTagName;
    }

    public static string FloatToTime(int time)
    {
        int s, m, h;
        h = time / 3600;
        m = (time / 60) % 60;
        s = time % 60;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
    }

    public static float DateTimeToFloat(DateTime date)
    {
        float result = 0;

        TimeSpan timeSpan = date - DateTime.Now;

        result = (float)timeSpan.TotalSeconds;

        return result;
    }

    public static string StringToMultiLine(string s)
    {
        string result = "";
        string[] infos = s.Split('|');


        result = infos[0];
        for (int i = 1; i < infos.Length; i++)
        {
            result += "\n" + infos[i];
        }
        return result;
    }

    #region fade alpha

    public static void FadeIn(UIWidget uiWidget, float fadeTime, AnimationCurve fadeCurve, float startAlpha, System.Action onComplete)
    {
        uiWidget.StartCoroutine(DoFadeIn(uiWidget, fadeTime, fadeCurve, startAlpha, onComplete));
    }

    static System.Collections.IEnumerator DoFadeIn(UIWidget uiWidget, float fadeTime, AnimationCurve fadeCurve, float startAlpha, System.Action onComplete)
    {
        Color endCol = uiWidget.color;
        endCol.a = 1f;
        Color startCol = uiWidget.color;

        if (startAlpha >= 0)
        {
            startCol.a = startAlpha;
        }

        float fTimer = 0;
        while (fTimer < fadeTime)
        {
            fTimer += Time.deltaTime;
            uiWidget.color = Color.Lerp(startCol, endCol, fadeCurve.Evaluate(fTimer / fadeTime));
            yield return null;
        }

        if (onComplete != null)
        {
            onComplete();
        }
    }

    public static void FadeOut(UIWidget uiWidget, float fadeTime, AnimationCurve fadeCurve, System.Action onComplete)
    {
        uiWidget.StartCoroutine(DoFadeOut(uiWidget, fadeTime, fadeCurve, onComplete));
    }

    static System.Collections.IEnumerator DoFadeOut(UIWidget uiWidget, float fadeTime, AnimationCurve fadeCurve, System.Action onComplete)
    {
        Color endCol = uiWidget.color;
        endCol.a = 0f;
        Color startCol = uiWidget.color;

        float fTimer = 0;
        while (fTimer < fadeTime)
        {
            fTimer += Time.deltaTime;
            uiWidget.color = Color.Lerp(startCol, endCol, fadeCurve.Evaluate(fTimer / fadeTime));
            yield return null;
        }

        if (onComplete != null)
        {
            onComplete();
        }
    }
    #endregion

    #region HashData
    public static string Encrypt(string toEncrypt, bool useHashing)
    {
        byte[] keyArray;
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

        string key = "SecurityKey1234567890123";

        //System.Windows.Forms.MessageBox.Show(key);
        //If hashing use get hashcode regards to your key
        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            hashmd5.Clear();
        }
        else
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //set the secret key for the tripleDES algorithm
        tdes.Key = keyArray;
        //mode of operation. there are other 4 modes.
        //We choose ECB(Electronic code Book)
        tdes.Mode = CipherMode.ECB;
        //padding mode(if any extra byte added)

        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateEncryptor();
        //transform the specified region of bytes array to resultArray
        byte[] resultArray =
          cTransform.TransformFinalBlock(toEncryptArray, 0,
          toEncryptArray.Length);
        //Release resources held by TripleDes Encryptor
        tdes.Clear();
        //Return the encrypted data into unreadable string format
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    public static string Decrypt(string cipherString, bool useHashing)
    {
        byte[] keyArray;
        //get the byte code of the string

        byte[] toEncryptArray = Convert.FromBase64String(cipherString);

        //Get your key from config file to open the lock!
        string key = "SecurityKey1234567890123";

        if (useHashing)
        {
            //if hashing was used get the hash code with regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //release any resource held by the MD5CryptoServiceProvider

            hashmd5.Clear();
        }
        else
        {
            //if hashing was not implemented get the byte code of the key
            keyArray = UTF8Encoding.UTF8.GetBytes(key);
        }

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //set the secret key for the tripleDES algorithm
        tdes.Key = keyArray;
        //mode of operation. there are other 4 modes. 
        //We choose ECB(Electronic code Book)

        tdes.Mode = CipherMode.ECB;
        //padding mode(if any extra byte added)
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(
                             toEncryptArray, 0, toEncryptArray.Length);
        //Release resources held by TripleDes Encryptor                
        tdes.Clear();
        //return the Clear decrypted TEXT
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
    #endregion

    public static Vector3 GetCursorPos()
    {
        Vector3 pos;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            pos = Input.GetTouch(0).position;
        }
        else
        {
            pos = Input.mousePosition;
        }
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = 0;
        return pos;
    }

    public static Vector3 GetScreenPosOfWorldPos(Vector3 worldPos, Transform transform, Camera uiCamera)
    {
        Vector3 posResult = Vector3.zero;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldPos);

        transform.position = uiCamera.ViewportToWorldPoint(viewPos);
        posResult = transform.localPosition;
        posResult.x = Mathf.FloorToInt(posResult.x);
        posResult.y = Mathf.FloorToInt(posResult.y);
        posResult.z = 0f;

        return posResult;
    }

    #region HandleCashInsufficient
    public static void HandleCashInsufficient()
    {
        UINoticeManager.OnButtonOK_click += ConfirmBuyGold;
        MessageBox.ShowDialog(GameManager.localization.GetText("Shop_CashInsufficient"), UINoticeManager.NoticeType.YesNo);
    }
    private static void ConfirmBuyGold()
    {
        UIShopManager.BUYGOLD = true;
        GameScenes.ChangeScense(GameScenes.currentSence, GameScenes.MyScene.ChargeShop);
    }
    #endregion

    public static int SumItemCountInList(UserItem[] items)
    {
        if (items == null) return 0;
        int result = 0;
        foreach (UserItem item in items)
        {
            result += item.Count;
        }
        return result;
    }
}
