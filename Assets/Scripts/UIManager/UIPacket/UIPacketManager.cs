using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPacketManager : MonoBehaviour {

    [System.Serializable]
    public struct Chest
    {
        public UISprite chest;
        public AnimationCurve openChestCurve;
        public UISprite openChestEffect;
    }
    [System.Serializable]
    public struct UIResult
    {
        public GameObject Root;
        public UIGrid heroRoot;
        public UIGrid itemRoot;
        public GameObject heroPrefab;
        public GameObject itemPrefab;
        public GameObject panelItem;
        public UIScrollView scrollViewHero;
        public UIScrollView scrollViewitem;
    }

    public UIGrid rootPacket;
    public GameObject packetPrefab;
    public UISprite header;
    public UILabel lblButton;
    public UILabel lblButtonPacket;
    public GameObject detail;
    public UILabel lblDetail;
    public Chest chest;
    public UIWorldMapManager worldmapManager;
    public UIResult uiResult;

    private UIPacketSlot _selectedSlot;
    private int _newHeroCount;
    private int _newItemCount;
    private List<GameObject> listOldObject = new List<GameObject>();

    void Start()
    {
        Localization();
        InitSlot();
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished)
        {
            lblButtonPacket.transform.parent.GetComponent<UIButton>().enabled = false;
            lblButtonPacket.transform.parent.GetComponent<UIPlayTween>().enabled = false;
        }
    }

    private void InitSlot()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = NGUITools.AddChild(rootPacket.gameObject, packetPrefab);
            UIPacketSlot slot = go.GetComponent<UIPacketSlot>();
            slot.Init(i + 1, this);
        }
        rootPacket.Reposition();
    }

    #region Button
    public void OnOpenChest_Click()
    {
        if (_selectedSlot == null) return;
        worldmapManager.SendBuyPackage(_selectedSlot.slotIndex);
    }
    public void OnCloseResult_Click()
    {
        uiResult.Root.SetActive(false);
    }
    public void ClodeDetail()
    {
        detail.SetActive(false);
    }
    #endregion

    #region public
    public void SelectedSlot(UIPacketSlot slot)
    {
        detail.SetActive(true);
        lblDetail.text = Helper.StringToMultiLine(GameManager.localization.GetText(string.Format("Package_VIP{0}", slot.slotIndex)));
        _selectedSlot = slot;
    }
    
    public void OnResponseBuyPackage(int itemNewCount, int heroNewCount)
    {
        _newHeroCount = heroNewCount;
        _newItemCount = itemNewCount;
        detail.SetActive(false);
        StartCoroutine(ShowChest());
    }
    #endregion

    #region private
    private void Localization()
    {
        if (Global.language == Global.Language.VIETNAM)
        {
            header.spriteName = "ruongvip";
        }
        else
        {
            header.spriteName = "packet-vip";
        }

        lblButton.text = GameManager.localization.GetText("Package_ButtonOpenChest");
        lblButtonPacket.text = GameManager.localization.GetText("Package_ButtonPackage");
    }

    private IEnumerator ShowChest()
    {
        ResetChest();

        Hashtable hash = new Hashtable();
        hash["y"] = 0;
        hash["time"] = 0.5f;
        hash["islocal"] = true;
        hash["easetype"] = iTween.EaseType.easeInCubic;
        chest.chest.gameObject.SetActive(true);
        iTween.MoveTo(chest.chest.gameObject, hash);
        Helper.FadeIn(chest.chest, 0.8f, chest.openChestCurve, 0, null);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(OnOpenChest());
    }

    private IEnumerator OnOpenChest()
    {
        chest.chest.GetComponent<UIPlayTween>().Play(true);
        yield return new WaitForSeconds(0.1f);
        chest.chest.GetComponent<UISprite>().spriteName = "ruong_nau_open";
        yield return new WaitForSeconds(0.3f);
        //chest.openChestEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        chest.chest.gameObject.SetActive(false);
        ShowResult();
    }

    private void ResetChest()
    {
        chest.chest.transform.localPosition = new Vector3(0, -600, 0);
        chest.chest.GetComponent<UISprite>().spriteName = "ruong_nau";
        chest.openChestEffect.color = new Color(1, 1, 1, 1);
        chest.openChestEffect.transform.localScale = new Vector3(1, 1, 1);
    }

    private void ShowResult()
    {
        if (listOldObject.Count > 0)
        {
            foreach (GameObject go in listOldObject)
            {
                NGUITools.Destroy(go);
            }
            uiResult.heroRoot.Reposition();
            uiResult.itemRoot.Reposition();
            listOldObject.Clear();
        }

        uiResult.Root.SetActive(true);

        for (int i = GameManager.GameUser.UserRoles.Count -_newHeroCount ; i < GameManager.GameUser.UserRoles.Count; i++)
        {
            GameObject go = NGUITools.AddChild(uiResult.heroRoot.gameObject, uiResult.heroPrefab);
            go.GetComponent<UIPacketHeroSlot>().InitUser(GameManager.GameUser.UserRoles[i]);
            listOldObject.Add(go);
        }
        uiResult.scrollViewHero.ResetPosition();
        uiResult.heroRoot.Reposition();

       // if (_newHeroCount <= 0) uiResult.panelItem.transform.localPosition = new Vector3(0, 0, 0);
       // else uiResult.panelItem.transform.localPosition = new Vector3(0, -269.4f, 0);

        for (int i = GameManager.GameUser.UserItems.Count - _newItemCount; i < GameManager.GameUser.UserItems.Count; i++)
        {
            GameObject go = NGUITools.AddChild(uiResult.itemRoot.gameObject, uiResult.itemPrefab);
            go.GetComponent<UIPacketItemSlot>().SetItem(GameManager.GameUser.UserItems[i]);
            go.transform.localScale = new Vector3(2, 2, 2);
            listOldObject.Add(go);
        }
        uiResult.scrollViewitem.ResetPosition();
        uiResult.itemRoot.Reposition();
    }
    #endregion
}
