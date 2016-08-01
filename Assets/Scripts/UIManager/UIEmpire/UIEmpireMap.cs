using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DEngine.Common.Config;

public class UIEmpireMap : MonoBehaviour {

    public Vector2 sizeMap = new Vector2(10, 10);
    public Vector2 sizeCell = new Vector2(30, 30);

    public GameObject cellRoot;
    public GameObject cellPrefab;
    public Image map;
    public Image mapBoder;

    [HideInInspector]
    public UIEmpireCell cellMouseOver;
    [HideInInspector]
    public bool isCanBuilding = false;

    private Dictionary<Vector2, UIEmpireCell> cells = new Dictionary<Vector2, UIEmpireCell>();

    public IEnumerator Init()
    {
        cellMouseOver = null;

        Vector2 sizeOfMap = Vector2.Scale(sizeCell, sizeMap) + sizeCell;
        map.GetComponent<RectTransform>().sizeDelta = sizeOfMap;
        
        mapBoder.GetComponent<RectTransform>().sizeDelta = Vector3.Scale(sizeOfMap, new Vector2(800.0f / 520.0f, 800.0f / 520.0f));


        for (int x = 0; x < sizeMap.x; x++)
        {
            for (int y = 0; y < sizeMap.y; y++)
            {
                GameObject cellObject = GameObject.Instantiate(cellPrefab) as GameObject;
                cellObject.name = string.Format("Cell_{0}_{1}", x, y);
                cellObject.SetActive(true);
                cellObject.transform.SetParent(cellRoot.transform);
                cellObject.transform.localPosition = new Vector3(x * sizeCell.x, -y * sizeCell.y);
                cellObject.transform.localScale = new Vector3(1, 1, 1);
                cellObject.transform.localRotation = Quaternion.identity;
                UIEmpireCell cell = cellObject.GetComponent<UIEmpireCell>();
                int status = (int)UIEmpireManager.Instance.curLand.LandData[x, y];
                cell.Init(this, new Vector2(x, y), (UIEmpireCell.State)status);
                cells[new Vector2(x, y)] = cell;
            }
            yield return null;
        }
    }

    #region public methods
    public void Clear()
    {
        foreach (var item in cells)
        {
            Destroy(item.Value.gameObject);
        }

        cells.Clear();
    }
    public IEnumerator RefreshCells()
    {

        for (int x = 0; x < sizeMap.x; x++)
        {
            for (int y = 0; y < sizeMap.y; y++)
            {
                UIEmpireCell cell = cells[new Vector2(x, y)];
                int status = (int)UIEmpireManager.Instance.curLand.LandData[x, y];
                cell.ChangeState((UIEmpireCell.State)status);
            }
            yield return null;
        }
    }
    public Vector3 GetCenterPosition(UIEmpireCell cell, Vector2 sizeBuilding)
    {
        Vector2 startpos = Vector2.zero;
        Vector2 endPos = Vector2.zero;

        GetRectOfBuiding(cell, sizeBuilding, out startpos, out endPos);

        Vector3 pos1 = cells[startpos].transform.localPosition;
        Vector3 pos2 = cells[endPos].transform.localPosition;

        Vector3 centerPos = (pos1 + pos2) / 2.0f;

        return centerPos;
    }
    public void OnMouseOverCellWithBuiding(UIEmpireCell cell, HouseData houseData)
    {
        cellMouseOver = cell;
        Vector2 startpos = Vector2.zero;
        Vector2 endPos = Vector2.zero;
        Vector2 buildingSize =new Vector2(houseData.SizeX, houseData.SizeY);
        GetRectOfBuiding(cell, buildingSize, out startpos, out endPos);

        isCanBuilding = true;

        UIEmpireCursor.Instance.SetColor(Color.white);

        for (int x = (int)startpos.x; x <= endPos.x; x++)
        {
            for (int y = (int)startpos.y; y <= endPos.y; y++)
            {
                Vector2 curCellPos = new Vector2(x, y);
                if (cells.ContainsKey(curCellPos))
                {
                    UIEmpireCell curCell = cells[curCellPos];
                    if (!curCell.CheckBuiding())
                    {
                       CanNotBuilding();
                    }
                }
                else
                {
                    CanNotBuilding();
                }
            }
        }

    }
    public void CanNotBuilding()
    {
        isCanBuilding = false;
        UIEmpireCursor.Instance.SetColor(Color.red);
    }
    public void OnClickCell(UIEmpireCell cell)
    {
        UIEmpireManager.Instance.OnOpenCell(cell);
        UIEmpireCursor.Instance.Hide(true);        
        
    }
    public void OnDrop()
    {
        Debug.Log("OnDrop");
    }
    public void OnFinishedBuilding()
    {
        cellMouseOver.FinishedBuilding();
    }
    public void OnFinishedBuilding(Vector2 pos, UIEmpireBuilding building)
    {
        OnMouseOverCellWithBuiding(cells[pos], building.houseData);
        cells[pos].FinishedBuilding();    
    }
    public void OnRemoveBuilding(UIEmpireBuildingFinished building)
    {
        Vector2 startpos = Vector2.zero;
        Vector2 endPos = Vector2.zero;

       // GetRectOfBuiding(cells[building.posInMap], building._empireBuilding, out startpos, out endPos);

        for (int x = (int)startpos.x; x <= endPos.x; x++)
        {
            for (int y = (int)startpos.y; y <= endPos.y; y++)
            {
                Vector2 curCellPos = new Vector2(x, y);
                if (cells.ContainsKey(curCellPos))
                {
                    UIEmpireCell curCell = cells[curCellPos];
                    curCell.ChangeState(UIEmpireCell.State.Free);
                }
            }
        }

    }
    public UIEmpireCell GetCell(Vector2 pos)
    {
        return cells[pos];
    }    
    #endregion


    #region Event Dynamic
    private void AddEventForCell(GameObject cell)
    {
        EventTrigger eventTrigger = cell.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback = new EventTrigger.TriggerEvent();
        UnityAction<BaseEventData> callBack = new UnityAction<BaseEventData>(OnCellClick);
        entry.callback.AddListener(callBack);
        eventTrigger.delegates.Add(entry);
    }
    private void OnCellClick(UnityEngine.EventSystems.BaseEventData baseEvent)
    {
        PointerEventData e = (PointerEventData)baseEvent;
        Debug.Log(e.pointerPress.gameObject.name);
    }
    #endregion

    #region private     
 
    private void GetRectOfBuiding(UIEmpireCell cell, Vector2 sizeBuilding, out Vector2 leftTop, out Vector2 rightBottom)
    {
        Vector2 buildingSize = new Vector2(sizeBuilding.x, sizeBuilding.y);
        Vector2 cellPos = cell.position;

        Vector2 startpos = Vector2.zero;
        startpos.x = cellPos.x - (int)(buildingSize.x / 2);
        startpos.y = cellPos.y - (int)(buildingSize.y / 2);
        Vector2 endPos = startpos + buildingSize - new Vector2(1, 1);

        leftTop = startpos;
        rightBottom = endPos;
    }
    #endregion
}
