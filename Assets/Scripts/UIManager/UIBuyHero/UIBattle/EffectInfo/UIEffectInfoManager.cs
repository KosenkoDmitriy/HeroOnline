using UnityEngine;
using System.Collections;

public class UIEffectInfoManager : MonoBehaviour {

    public UIEffectInfoInstance effectInstance;
    public UIGrid root;

    private static UIEffectInfoManager _instance;
    public static UIEffectInfoManager Instance { get { return _instance; } }

    private UIEffectInfoInstance oldObject;

	void Start () {
        _instance = this;
	}

    public void Add(int effectID)
    {

        if (oldObject != null)
        {
            if (oldObject._effectID == effectID) return;
            oldObject.OnDestroy();
        }

        GameObject go = NGUITools.AddChild(root.gameObject, effectInstance.gameObject);
        UIEffectInfoInstance uiEffect = go.GetComponent<UIEffectInfoInstance>();
        go.SetActive(true);
        uiEffect.SetEffect(effectID);
        oldObject = uiEffect;
    }


    public void Add(string s)
    {
        if (oldObject != null)
        {
            oldObject.OnDestroy();
        }
        GameObject go = NGUITools.AddChild(root.gameObject, effectInstance.gameObject);
        UIEffectInfoInstance uiEffect = go.GetComponent<UIEffectInfoInstance>();
        go.SetActive(true);
        uiEffect.AddText(s);
        oldObject = uiEffect;
    }

}
