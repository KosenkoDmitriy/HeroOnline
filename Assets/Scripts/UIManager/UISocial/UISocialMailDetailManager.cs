using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;

public class UISocialMailDetailManager : MonoBehaviour
{
    [System.Serializable]
    public struct UILables
    {
        public UILabel lblTakeAll;
        public UILabel lblTakeDelete;
        public UILabel lblTakeAddFriend;
    }

    public UILables uiLables;
    public UISocialManager manager;
    public UILabel lblFrom;
    public UILabel lblSubject;
    public UITextList textList ;
    public UIGrid giftRoot;
    public GameObject giftRefab;
    public UIButton btnTakeAll;
    public UIButton btnAddFriend;


    private UserMail _userMail;
    private string text;
    private List<GameObject> _gifts = new List<GameObject>();

    void Start()
    {
        uiLables.lblTakeAddFriend.text = GameManager.localization.GetText("Social_Mail_AddFriend");
        uiLables.lblTakeDelete.text = GameManager.localization.GetText("Social_btnDelete");
        uiLables.lblTakeAll.text = GameManager.localization.GetText("Social_Mail_TakeAll");
    }
    void OnDisable()
    {
     
        textList.Clear();
    }
    public void SetDetail(UserMail userMail)
    {
        _userMail = userMail;
        lblFrom.text = GameManager.localization.GetText("Social_Form") + " " + _userMail.Sender.Base.NickName;
        lblSubject.text = GameManager.localization.GetText("Social_Subject") + " " + _userMail.Title;
        Invoke("SetTextMail", 0.5f);       
        if (_userMail.SenderId == 0)
        {
            btnTakeAll.gameObject.SetActive(false);
            btnAddFriend.gameObject.SetActive(false);
        }
        else
        {
            btnTakeAll.gameObject.SetActive(false);
            btnAddFriend.gameObject.SetActive(true);
        }
        SetItem(userMail.Items);

        //userMail.
       
    }
    private void SetTextMail()
    {
        textList.Clear();
        textList.Add(_userMail.Message);
    }

    public void SetDetail(UIReport report)
    {
        btnTakeAll.gameObject.SetActive(false);
        if (report.type == 0)
        {
            PvALog log = GameManager.pvaLogs.FirstOrDefault(p => p.LogId == report.id);
            if (log != null)
            {
                lblFrom.text = log.Opponent.Base.NickName;
                lblSubject.text = "";
                text = "";

                string time = log.LogTime.Hour + ":" + log.LogTime.Minute;
                string day = log.LogTime.ToString(GameManager.localization.GetText("Global_ShortDay"));
                string enemy = string.Format("[FF0000]{0}[-]", log.Opponent.Base.NickName);
                string SilverEarn = string.Format("[00FF00]{0}[-]", Mathf.Abs(log.Silver));
                string SilverDrop = string.Format("[00FF00]{0}[-]", Mathf.Abs(log.Silver/10));             
                              
                string s;
                if (log.Mode == 0)
                {
                    if (log.Result < 0)
                    {
                        s = GameManager.localization.GetText("Social_MailDetail_Pillage_WinAttack");
                        text = string.Format(s, time, day, enemy, SilverEarn, SilverDrop);
                    }
                    else
                    {
                        s = GameManager.localization.GetText("Social_MailDetail_Pillage_LoseAttack");
                        text = string.Format(s, time, day, enemy);
                    }
                }
                else
                {
                    if (log.Result < 0)
                    {
                        s = GameManager.localization.GetText("Social_MailDetail_Pillage_WinDefence");                       
                    }
                    else
                    {
                        s = GameManager.localization.GetText("Social_MailDetail_Pillage_LoseDefence");
                    }
                    text = string.Format(s, time, day, enemy, SilverEarn);
                }


                Invoke("SetTexreport", 0.5f);
            }
        }
        else
        {
            PvPLog log = GameManager.pvpLogs.FirstOrDefault(p => p.LogId == report.id);
            if (log != null)
            {
                lblFrom.text = log.Opponent.Base.NickName;
                lblSubject.text = "";
                text = "";


                string s;
                if (log.Result < 0)
                {
                    s = GameManager.localization.GetText("Social_MailDetail_WinPVP");
                }
                else
                {
                    s = GameManager.localization.GetText("Social_MailDetail_LosePVP");
                }

                string time = log.LogTime.Hour + ":" + log.LogTime.Minute;
                string day = log.LogTime.ToString(GameManager.localization.GetText("Global_ShortDay"));
                string enemy = string.Format("[FF0000]{0}[-]", log.Opponent.Base.NickName);
                string HonorEarn = string.Format("[FFFF00]{0}[-]",Mathf.Abs(log.HonorAdd));
                string curHonor = string.Format("[FFFF00]{0}[-]", Mathf.Abs(log.HonorTotal));
                string curRank = string.Format("[00FF00]{0}[-]", GameManager.GameUser.Base.HonorRank);

                text = string.Format(s, time, day, enemy, HonorEarn, curHonor, curRank);

                Invoke("SetTexreport", 0.5f);
            }
        }
    }
    private void SetTexreport()
    {
        textList.Clear();

        string[] texts = text.Split('|');


        for (int i = 0; i < texts.Length; i++)
        {
            textList.Add(texts[i]);
        }

    }

    private void SetItem(GameObjList items)
    {
        if (items.Count <= 0)
            btnTakeAll.gameObject.SetActive(false);
        else
            btnTakeAll.gameObject.SetActive(true);

        foreach (GameObject go in _gifts)
            NGUITools.Destroy(go);
        _gifts.Clear();


        foreach (GameObj obj in items)
        {
            UserItem userItem = (UserItem)obj;
            userItem.GameItem = (GameItem)GameManager.GameItems[userItem.ItemId];
            
            GameObject go = NGUITools.AddChild(giftRoot.gameObject, giftRefab);
            go.SetActive(true);

            UITexture icon = go.transform.FindChild("Texture").GetComponent<UITexture>();
            UILabel label = go.transform.FindChild("Label").GetComponent<UILabel>();
            
            if (userItem.GameItem.SubKind == (int)ItemSubKind.Equipment)
                icon.mainTexture = Helper.LoadTextureForEquipItem(userItem.ItemId);
            else
                icon.mainTexture = Helper.LoadTextureForSupportItem(userItem.ItemId);
            
            label.text = "x" + userItem.Count;
            _gifts.Add(go);
        }

        giftRoot.Reposition();
        giftRoot.transform.parent.GetComponent<UIScrollView>().ResetPosition();
    }

    public void OnButtonTakeAll_Click()
    {
        if (_userMail == null) return;
        manager.SendRequestReadMail(_userMail.MailId);
    }

    public void OnButtonDelete_Click()
    {
        if (_userMail == null) return;
        manager.SendRequestReadMail(_userMail.MailId);
        gameObject.SetActive(false);
    }

    public void OnButtonAddFriend_Click()
    {
        manager._curNicknameAdd = _userMail.Sender.Base.NickName;
        manager.friendActiveType = UISocialManager.FriendActiveType.AddFriend;
        manager.SendRequestAddFriend(_userMail.Sender.Base.NickName);
        manager.SendRequestReadMail(_userMail.MailId);
        gameObject.SetActive(false);
    }
}
