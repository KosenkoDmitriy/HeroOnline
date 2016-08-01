using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.Config;

public class UIEmpireBuildingPanel : MonoBehaviour
{
    [System.Serializable]
    public struct BuildingObject
    {
        public GameObject buidingRootBackground;
        public GameObject buidingRoot;
        public GameObject buidingPrefab;

        [HideInInspector]
        public RectTransform rectTranformBackground;
    }

    public Vector2 buildingSize = new Vector2(64, 64);

    public UIEmpireManager manager;
    public BuildingObject buildingObject;
    public bool SubPanel = false;

    private Dictionary<int, UIEmpireBuilding> buildings = new Dictionary<int, UIEmpireBuilding>();

    public void Init(UIEmpireManager manager)
    {
        int numhouse = LandConfig.HOUSE_DATA.Count;

        int numHouseView = 0;
        int i = 0;
        foreach (var house in LandConfig.HOUSE_DATA)
        {
            int id = house.Key;

            if (id <= 65)
            {
                GameObject buildingObj = GameObject.Instantiate(buildingObject.buidingPrefab) as GameObject;
                buildingObj.SetActive(true);
                buildingObj.transform.SetParent(buildingObject.buidingRoot.transform);
                buildingObj.transform.localPosition = new Vector3(0, -buildingSize.y * i);
                buildingObj.transform.localScale = new Vector3(1, 1, 1);
                buildingObj.transform.localRotation = Quaternion.identity;
                UIEmpireBuilding building = buildingObj.GetComponent<UIEmpireBuilding>();
                building.Init(house.Value, id);
                buildings[id] = building;
                numHouseView++;
            }

            if (id >= 65 && id <= 90)
                buildings[65].addSubBuilding(house.Value);     

            i++;
        }

        buildingObject.rectTranformBackground = buildingObject.buidingRootBackground.GetComponent<RectTransform>();
        buildingObject.rectTranformBackground.sizeDelta = new Vector2(80, buildingSize.y * (numHouseView + 1));
        buildingObject.rectTranformBackground.localPosition = new Vector3(0, -1000, 0);
    }


    public void Init(UIEmpireManager manager, List<HouseData> listbuilding)
    {
        if (buildings.Count > 0)
        {
            foreach (var building in buildings)
            {
                GameObject.Destroy(building.Value.gameObject);            
            }
            buildings.Clear();
        }
        for (int i = 0; i < listbuilding.Count; i++)
        {
            HouseData buildingtemp = listbuilding[i];
            GameObject buildingObj = GameObject.Instantiate(buildingObject.buidingPrefab) as GameObject;
            buildingObj.SetActive(true);
            buildingObj.transform.SetParent(buildingObject.buidingRoot.transform);
            buildingObj.transform.localPosition = new Vector3(0, -buildingSize.y * i);
            buildingObj.transform.localScale = new Vector3(1, 1, 1);
            buildingObj.transform.localRotation = Quaternion.identity;
            UIEmpireBuilding building = buildingObj.GetComponent<UIEmpireBuilding>();
            building.Init(buildingtemp, buildingtemp.Id);

            buildings[buildingtemp.Id] = building;
        }

        buildingObject.rectTranformBackground = buildingObject.buidingRootBackground.GetComponent<RectTransform>();
        buildingObject.rectTranformBackground.sizeDelta = new Vector2(80, buildingSize.y * listbuilding.Count);
        buildingObject.rectTranformBackground.localPosition = new Vector3(0, -1000, 0);
    }


    public UIEmpireBuilding GetBuilding(int id)
    {
        if(buildings.ContainsKey(id))
            return buildings[id];
        return null;
    }

    #region ButtonUpDown
    public void OnButtonUpPress()
    {
        Vector3 pos = buildingObject.rectTranformBackground.position;
        pos.y -= 80 * Time.deltaTime;
        buildingObject.rectTranformBackground.position = pos;

    }
    public void OnButtonDownPress()
    {
        Vector3 pos = buildingObject.rectTranformBackground.position;
        pos.y += 80 * Time.deltaTime;
        buildingObject.rectTranformBackground.position = pos;

    }
    #endregion
}
