using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DEngine.Common.GameLogic;
using DEngine.Common.Config;

public class UIEmpireBuilding : MonoBehaviour {

    public Image icon;
    public Text txtSize;
    public HouseData houseData;
    public int ID;
    private List<HouseData> _subBuildings;

    public void Init(HouseData house,int id)
    {
        _subBuildings = new List<HouseData>();
        ID = id;
        houseData = house;

        icon.sprite = Helper.LoadSpriteForBuilding(ID);
        //icon.SetNativeSize();
        txtSize.text = string.Format("{0}x{1}", houseData.SizeX, houseData.SizeY);
    }

    public void addSubBuilding(HouseData subBuilding)
    {
        _subBuildings.Add(subBuilding);
    }

    public bool isContainSubBuilding()
    {
        if (_subBuildings == null) return false;
        return _subBuildings.Count > 0;
    }

    public void OnDragStart()
    {
        if (isContainSubBuilding())
        {
            UIEmpireManager.Instance.ShowSubPanel();
            UIEmpireManager.Instance.buildingSubPanel.Init(UIEmpireManager.Instance, _subBuildings);

            return;
        }
        UIEmpireManager.Instance.OnDragBuilding(houseData);
    }

    public void OnDragEnd()
    {
        UIEmpireManager.Instance.OnDropBuilding();
    }

    public void OnClick()
    {
        if (isContainSubBuilding())
        {
            UIEmpireManager.Instance.ShowSubPanel();
            UIEmpireManager.Instance.buildingSubPanel.Init(UIEmpireManager.Instance, _subBuildings);
        }

    }

    public void ShowInfo()
    {
        UIEmpireManager.Instance.ShowInfo(this);
    }

}
