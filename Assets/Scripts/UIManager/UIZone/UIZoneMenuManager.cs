using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System;

public class UIZoneMenuManager : MonoBehaviour {

    public GameObject zonePrefabs;
    public GameObject zoneRoot;

    private List<GameObject> _zones = new List<GameObject>();
    private int _zoneCount = 0;
    private ZoneController _zoneController;//network event
    public void Start()
    {
        _zoneController = new ZoneController(this);
        int i = 1;
        foreach (GameZone gameZone in GameManager.GameZones)
        {
            AddZone(gameZone, i);
            i++;
        }
    }

    void Update()
    {
        //MyInput.CheckInput();
    }
    //add channel zone to GUI
    public void AddZone(GameZone gameZone,int index)
    {
        GameObject go = NGUITools.AddChild(zoneRoot, zonePrefabs);
        go.transform.localPosition = new Vector3(0, -(_zoneCount * 60 + _zoneCount * 3), 0);
        go.name = gameZone.ZoneName;

        UIZoneButtonManager uiZoneButtonManager = go.GetComponent<UIZoneButtonManager>();
        uiZoneButtonManager.gameZone = gameZone;


        float percent = ((float)gameZone.ZoneCurCCU) / (float)gameZone.ZoneMaxCCU;
        Color c = Color.green;
        if (percent >= 0.5f)
            c = Color.yellow;
        if (percent >= 0.8f)
            c = Color.red;

        uiZoneButtonManager.lblName.text = string.Format("[{0}]{1}[-]", Helper.ColorToHex(c), GameManager.serverName + " " + index);

        uiZoneButtonManager.uiZoneManager = this;

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
        GameScenes.ChangeScense(GameScenes.MyScene.Zone, GameScenes.MyScene.ServerGame);
    }

    //click zone
    public void OnSelectedZone(Guid zoneID)
    {
        GameManager.ZoneID = zoneID;
        GameScenes.ChangeScense(GameScenes.MyScene.Zone, GameScenes.MyScene.Login);
    }

  
}
