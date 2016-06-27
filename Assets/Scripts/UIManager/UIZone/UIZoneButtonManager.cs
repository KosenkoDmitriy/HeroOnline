using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIZoneButtonManager : MonoBehaviour
{

    public GameZone gameZone;
    public UILabel lblName;

    public UIZoneMenuManager uiZoneManager { get; set; }

    void OnClick()
    {
        if (uiZoneManager != null)
            uiZoneManager.OnSelectedZone(gameZone.ServerId);
    }

}
