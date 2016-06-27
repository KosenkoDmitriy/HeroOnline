using UnityEngine;
using System.Collections;

public class UIBackgroundDrop : MonoBehaviour {



    void OnClick()
    {
        Debug.Log("OnClick");
        UIUpgradeItemSlot.OnDragRelease();
    }

    void OnDrop(GameObject go)
    {
        Debug.Log("OnDrop");
        UIUpgradeItemSlot.OnDragRelease();
    }

}
