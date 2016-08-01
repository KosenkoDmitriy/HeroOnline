using UnityEngine;
using System.Collections;

public class UIEffectEvent : MonoBehaviour {

    public int effectID;

    private float _longClickDuration = 1f;
    float time = 0f;

    void OnPress(bool pressed)
    {
        //if (pressed)
        //{
        //    time += Time.deltaTime;

        //    if (time >= _longClickDuration)
        DoLongPress();
        //}
        //else
        //    time = 0;
    }

    private void DoLongPress()
    {
        UIEffectInfoManager.Instance.Add(effectID);
        Debug.Log("DoLongPress");
    }
}
