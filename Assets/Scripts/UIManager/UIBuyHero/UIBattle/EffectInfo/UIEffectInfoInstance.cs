using UnityEngine;
using System.Collections;

public class UIEffectInfoInstance : MonoBehaviour {

    public UILabel lblName;
    public UILabel lblDesc;
    public UISprite icon;

    public UIPlayTween playTween;

    public int _effectID;

    public void AddText(string s)
    {
        icon.gameObject.SetActive(false);
        lblDesc.text = string.Format(s);
        StartCoroutine(Close());
        lblName.text = "";
    }

    public void SetEffect(int effectID)
    {
        icon.gameObject.SetActive(true);

        _effectID = effectID;
        string effectPrefix = string.Format("effect{0:D2}_", effectID);
        icon.spriteName = effectPrefix + "1";

        MyLocalization.EffectInfo effectInfo = GameManager.localization.getEffectInfo(effectID);
        lblName.text = effectInfo.Name;
        lblDesc.text = string.Format("[i]{0}[/i]", effectInfo.Desc);

        StartCoroutine(Close());        
    }

    public void OnDestroy()
    {
        StopCoroutine(Close());
        NGUITools.Destroy(gameObject);
    }

    public IEnumerator Close()
    {
        yield return new WaitForSeconds(2);
        playTween.Play(true);
    }

    public void CloseFinished()
    {
        NGUITools.Destroy(gameObject);
    }
}
