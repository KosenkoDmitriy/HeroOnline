using DEngine.Common.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIServerMenuManager : MonoBehaviour
{
    public GameObject ServerPrefabs;
    public GameObject ServerRoot;
    
    private string lolServiceUrl = "";

    public string serverApplication = "HeroWorld";

    private List<GameObject> _zones = new List<GameObject>();
    private int _zoneCount = 0;
    private ServerGameController _ServerGameController;
    private bool _sendRequestZones;

    //Get list of master from web service ,  Add list of master to GUI
    public IEnumerator Start()
    {
        if(Global.LOCAL)
			lolServiceUrl = "http://lol.yourplaceforfun.com/Game/GetWorlds"; // "http://home.blueskysoft.vn/lolservices/game/getworlds";
        else
			lolServiceUrl = "https://ho.yourplaceforfun.com/Game/GetWorlds"; //"http://service.leagueoflords.org/Game/GetWorlds";// change serviceen.leagueoflords.vn to yourWebserviceURL 

        _sendRequestZones = false;
        _ServerGameController = new ServerGameController(this);

        if (_ServerGameController.IsConnected)
            _ServerGameController.Disconnect();


        WWW www = new WWW(lolServiceUrl);
        yield return www;

        WorldInfoList worldList = WorldInfoList.FromXML(www.text);
        foreach (var worldInfo in worldList.Worlds)
        {
            AddWorld(worldInfo);
        }
    }

    void Update()
    {
        //MyInput.CheckInput();

        //when connected requies channel zone list from master
        if (_ServerGameController.IsConnected)
        {
            if (!_sendRequestZones)
            {
                _ServerGameController.SendRequestZones();
                _sendRequestZones = true;
            }
        }
    }
    //add master to GUI
    public void AddWorld(WorldInfo worldInfo)
    {
        GameObject go = NGUITools.AddChild(ServerRoot, ServerPrefabs);
        go.transform.localPosition = new Vector3(0, -(_zoneCount * 60 + _zoneCount * 3), 0);
        go.name = worldInfo.Name;

        UIServerButtonManager uiServerButtonManager = go.GetComponent<UIServerButtonManager>();
        uiServerButtonManager.serverAddress = worldInfo.ServiceAddress;
        uiServerButtonManager.lblName.text = string.Format("{0}", worldInfo.Name);
        uiServerButtonManager.uiServerManager = this;
        uiServerButtonManager.worldInfo = worldInfo;
        _zones.Add(go);
        _zoneCount++;
    }

    public void DestroyZones()
    {
        for (int i = 0; i < _zones.Count; i++)
            Destroy(_zones[i]);
    }

    public void OnButtonBack_Click()
    {
        Debug.Log("OnButtonBack_Click");
        GameScenes.ChangeScense(GameScenes.MyScene.ServerGame, GameScenes.MyScene.MainMenu);
    }

    //select server , and connect
    // masterlist btn is created in runtime by  UIServerPrefabs(UIServerButtonManager.cs script)
    public void OnSelectedServer(string serverAddress, WorldInfo worldInfo)
    {
        if (worldInfo.Version != Global.version)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_ErrorVersion"), UINoticeManager.NoticeType.Message);
            return;
        }
        _ServerGameController.Connect(serverAddress, serverApplication);
        MessageBox.ShowDialog(GameManager.localization.GetText("Dialog_Waiting"), UINoticeManager.NoticeType.Waiting);
    }

    public void OnResponseZoneList()
    {
        MessageBox.CloseDialog();
        GameScenes.ChangeScense(GameScenes.MyScene.Login, GameScenes.MyScene.Zone);
    }

    private void ChangeSceneOnline()
    {
        if (_ServerGameController.IsConnected)
        {
            _ServerGameController.SendRequestZones();
        }
        else
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("MainMenu_NoConnectToServer"), UINoticeManager.NoticeType.Message);
            Debug.Log("no connect");
        }
    }

}
