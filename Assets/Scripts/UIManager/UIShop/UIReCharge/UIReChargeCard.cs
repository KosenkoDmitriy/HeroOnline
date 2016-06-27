using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIReChargeCard : MonoBehaviour {

    public UIShopManager manager;

    public UILabel lblHeader;
    public UIPopupList cardType;
    public UIInput series;
    public UIInput number;
    public UILabel lblBtnOK;
    public UILabel lblBtnEnd;
    public UILabel lblResult;
    public GameObject fieldRoot;
    public GameObject finishedRoot;

    public UIInput txtSeries;
    public UIInput txtNumber;

	void Start () {
        Localization();
        fieldRoot.SetActive(true);
        finishedRoot.SetActive(false);
	}
	
	void Update () {
	
	}

    private void Localization()
    {
        lblHeader.text = GameManager.localization.GetText("Shop_Card_Header");
        series.defaultText = GameManager.localization.GetText("Shop_Card_Series");
        number.defaultText = GameManager.localization.GetText("Shop_Card_CardNumber");
        lblBtnOK.text = GameManager.localization.GetText("Shop_Card_Button");
        lblBtnEnd.text = GameManager.localization.GetText("Shop_Card_Response");


        cardType.transform.FindChild("Label").GetComponent<UILabel>().text = GameManager.localization.GetText("Shop_Card_CardType");
        for (int i = 1; i < (int)CardType.Count; i++)
        {
            cardType.AddItem(((CardType)i).ToString());
        }
    }

    public void OnButtonOK_Click()
    {
        CardType type = (CardType)(cardType.items.IndexOf(cardType.value) + 1);

        string series = "", number = "";
        series = txtSeries.value.Trim();
        number = txtNumber.value.Trim();

        manager.SendChargeCashCard(type, series, number);
    }

    public void OnResponeFromServer(int cardValue, int goldValue)
    {
        fieldRoot.SetActive(false);
        finishedRoot.SetActive(true);
        
        lblResult.text = string.Format(GameManager.localization.GetText("Shop_Card_Success"), cardType.value, cardValue, goldValue);
    }

    public void OnButtonEnd_Click()
    {
        fieldRoot.SetActive(true);
        finishedRoot.SetActive(false);

    }

    public void OnPopupCardType_Changed()
    {
        string text = cardType.value;
        if (text == null)
        {
            text = GameManager.localization.GetText("Shop_Card_CardType");
        }
        cardType.transform.FindChild("Label").GetComponent<UILabel>().text = text;
    }

}
