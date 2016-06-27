using UnityEngine;
using System.Collections;
using System;

public class UINoticeManager : MonoBehaviour {

    public enum NoticeType
    {
        Waiting,
        Message,
        YesNo
    }

    public TweenScale tweenScale;
    public UILabel lblHeader;
    public UILabel lblOk;
    public UILabel lblCancel;

    public UILabel lblText;
    public UIButton btnOk;
    public UIButton btnCancel;
    public NoticeType noticetype;

    public delegate void NoticeHandle();
    public static event NoticeHandle OnButtonOK_click;
    public static event NoticeHandle OnButtonCancel_click;

    private int _id;

    void Start()
    {
        lblHeader.text = GameManager.localization.GetText("Dialog_Notice");
        lblOk.text = GameManager.localization.GetText("Global_btn_OK");
        lblCancel.text = GameManager.localization.GetText("Global_btn_Cancel");
    }


    void OnDestroy()
    {
        StopAllCoroutines();
    }
    public void SetNotice(NoticeType type, string message,int id)
    {
        noticetype = type;
        lblText.text = message;
        _id = id;
        switch (type)
        {
            case NoticeType.Waiting:
                btnOk.gameObject.SetActive(false);
               // btnCancel.gameObject.SetActive(false);
                btnCancel.transform.localPosition = Vector3.zero;
                StartCoroutine(AnimationText());
                break;
            case NoticeType.Message:
                btnCancel.gameObject.SetActive(false);
                btnOk.transform.localPosition = Vector3.zero;
                GetComponent<UIPanel>().depth += 1;
                break;
            default:
                GetComponent<UIPanel>().depth += 2;
                break;
        }
    }

    private IEnumerator AnimationText()
    {
        yield return new WaitForSeconds(0.0f);
        int index = 0;
        string text = lblText.text;
        string oldText = text;
        int length = text.Length;
        string format = "[b][u][i][00FFFF]";
        while (true)
        {
            text = oldText;
            text = text.Insert(index, format);
            text = text.Insert(index + format.Length + 1, "[-][/i][/u][b]");
            lblText.text = text;

            index++;
            if (index == length) index = 0;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void OnButtonOkClick()
    {
        if (OnButtonOK_click != null)
            OnButtonOK_click.Invoke();

        if (OnButtonOK_click != null)
            OnButtonOK_click -= OnButtonOK_click;

        if (OnButtonCancel_click != null)
            OnButtonCancel_click -= OnButtonCancel_click;

        tweenScale.duration = tweenScale.duration / 2;
        MessageBox.CloseDialog(_id);

    }
    public void OnButtonCancelclick()
    {
        if (OnButtonCancel_click != null)
            OnButtonCancel_click.Invoke();
       
        if (OnButtonOK_click != null)
            OnButtonOK_click -= OnButtonOK_click;

        if (OnButtonCancel_click != null)
            OnButtonCancel_click -= OnButtonCancel_click;

        tweenScale.duration = tweenScale.duration / 2;
        MessageBox.CloseDialog(_id);

    }
}
