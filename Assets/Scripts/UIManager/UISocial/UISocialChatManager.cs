using UnityEngine;
using System.Collections;

public class UISocialChatManager : MonoBehaviour {

    public UITextList textList;
    public UISocialManager manager;

    private UIInput _Input;


    void Start()
    {
        _Input = GetComponent<UIInput>();
        _Input.label.maxLineCount = 1;
        if (GameManager.oldChat.Count > 0)
        {
            foreach (string s in GameManager.oldChat)
                textList.Add(s);
        }
    }

    void OnDestroy()
    {
    }

    public void OnReciveChat(int senderID, string chatMessage)
    {
        if (senderID == 0)
        {
            chatMessage = string.Format(GameManager.localization.GetText("SystemMessage_System"), GameManager.getFormatMessageSystem(chatMessage));
        }
        textList.Add(chatMessage);
    }

    public void OnSubmit()
    {
        if (textList != null)
        {
            string text = NGUIText.StripSymbols(_Input.value);

            if (!string.IsNullOrEmpty(text))
            {

                string s = string.Format("[FFFF00]{0}[-]: {1}", GameManager.GameUser.Base.NickName, text);

                manager.OnSendChat(s);

                _Input.value = "";
                _Input.isSelected = false;
            }
        }
    }
}
