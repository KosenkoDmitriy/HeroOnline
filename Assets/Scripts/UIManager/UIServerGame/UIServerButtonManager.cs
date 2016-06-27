using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using DEngine.Common.Services;

public class UIServerButtonManager : MonoBehaviour
{
    public UILabel lblName;
    public string serverAddress;
    public WorldInfo worldInfo;
    public UIServerMenuManager uiServerManager { get; set; }

    void OnClick()
    {
        GameManager.serverName = lblName.text;
        if (uiServerManager != null)
            uiServerManager.OnSelectedServer(serverAddress, worldInfo);
    }
}
