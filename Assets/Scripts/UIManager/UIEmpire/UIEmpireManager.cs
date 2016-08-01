using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;

public class UIEmpireManager : MonoBehaviour {

    [System.Serializable]
    public struct UIHouseInfo
    {
        public GameObject root;
        public Image icon;
        public Text txtName;
        public Text txtSize;
        public Text txtDesc;
        public Text txtGold;
        public Text txtSilver;
        public Text txtMax;
        public GameObject goldRoot;
        public GameObject silverRoot;
        public RectTransform rectDesc;
    }


    [System.Serializable]
    public struct UIContaint
    {
        public Text txtWinCore;
        public Text txtLoseCore;
        public Text txtToggleEdit;
        public Text txtButtonExpand;
        public GameObject infoRoot;
        public Text txtInfo;

        public GameObject silverOfBank;
        public Text txtSilverOfBank;

        public GameObject panelInfo;
    }

    [System.Serializable]
    public struct UIUserInfor
    {
        public UILabel lvlLevel;
        public UILabel lvlNickName;
        public UILabel lvlEXP;
        public UIProgressBar exp;
        public UITexture avatar;

        public UILabel lblGold;
        public UILabel lblSilver;

    }
           
    public static bool isFriend = false;
    public static UserLand friendLand;

    public GameObject BuidingFinishedPrefab;
    public GameObject BuidingFinishedRoot;
    public UIEmpireMap empireMap;
    public UIEmpireBuildingPanel buildingPanel;
    public UIEmpireBuildingPanel buildingSubPanel;
    public Slider slideZoom;
    public Toggle togCustomBuilding;
    public Image recycle;
    public UIContaint uiContaint;
    public Slider slidePoint;
    public Image imageSlidePoint;
    public UIHouseInfo houseInfo;
    public Button btnExpand;
    public UIUserInfor uiUserInfo;
    public static UIEmpireManager Instance {get;private set;}

    [HideInInspector]
    public HouseData houseDataSelected;
    [HideInInspector]
    public bool DroptoTrash = false;

    public UserLand curLand;

    private List<UIEmpireBuildingFinished> listBuidingFinished;
    private UIEmpireBuildingFinished buildingFinishedDrag;
    private EmpireController _controller;
    private UIEmpireCell _cellSelected;
    private RectTransform rectMap;
    private float _timerRequestSilverBank;

    IEnumerator Start()
    {
        Instance = this;

        Localization();
        _timerRequestSilverBank = Time.time;
        _controller = new EmpireController(this);
        houseDataSelected = null;

        rectMap = empireMap.GetComponent<RectTransform>();
        listBuidingFinished = new List<UIEmpireBuildingFinished>();

        InitLand();

        yield return StartCoroutine(empireMap.Init());

        StartCoroutine(InitOldBuiding());


      
    }

    void Update()
    {
       // silverBank += Time.deltaTime * curLand.Houses.Count(p => p.HouseId == 2); 
     //   uiContaint.txtSilverOfBank.text = ((int)silverBank).ToString();

        if (!isFriend)
        {
            if (Time.time - _timerRequestSilverBank > 10)
            {
                _controller.SendCheckBank(false);
                _timerRequestSilverBank = Time.time;
            }
        }
    }

    #region public

    public void ClearDrag()
    {
        UIEmpireCursor.Instance.Hide(true);
        houseDataSelected = null;
        empireMap.CanNotBuilding();
    }

    public bool HasDragBuilding()
    {
        return houseDataSelected != null;
    }

    public void OnDragBuildingFinished(UIEmpireBuildingFinished building)
    {
        buildingFinishedDrag = building;
        UIEmpireCursor.Instance.Show(Helper.LoadSpriteForBuilding(building.userHouse.HouseId));
       // OnDragBuilding(LandConfig.HOUSE_DATA[building.userHouse.HouseId]);
    }
    
    public void OnDragBuilding(HouseData houseData)
    {
        houseDataSelected = houseData;
        UIEmpireCursor.Instance.Show(Helper.LoadSpriteForBuilding(houseData.Id));
    }

    public bool OnDropBuilding()
    {
        bool b = false;
        if (!empireMap.isCanBuilding)
        {
            ClearDrag();
        }
        else
        {
            Vector2 sizeOfBuilding = new Vector2(houseDataSelected.SizeX, houseDataSelected.SizeY);
            Vector2 posOfCell = empireMap.cellMouseOver.position;
            Vector2 posInServer = PosClientToServer(posOfCell, sizeOfBuilding);
            _controller.SendBuildHouse(houseDataSelected.Id, posInServer);
            b = true;
        }
     
        return b;
    }

    public void MoveMap(Vector3 dir)
    {


        float speed = 6;


        Vector3 endPos = rectMap.position + dir * speed;

       // endPos = Vector3.Lerp(rectMap.position, endPos, Time.deltaTime * 2);

        rectMap.position = endPos;


        //    Vector3 endPos = rect.position + dir;

        //    Hashtable hash = new Hashtable();
        //    hash["position"] = endPos;
        //    hash["time"] = 0.5f;
        //    hash["islocal"] = true;
        //    hash["easetype"] = iTween.EaseType.easeOutExpo;

        //    iTween.MoveTo(rect.gameObject, hash);


    }

    public void OnSliderZoomChanged()
    {
        float valueSlide = slideZoom.value;
        float scale = Mathf.Lerp(0.5f, 1.5f, valueSlide);

        empireMap.transform.localScale = new Vector3(scale, scale, scale);

    }

    public void OnToggleCustomBuilding()
    {
        if (togCustomBuilding.isOn)
        {
            recycle.gameObject.SetActive(true);
            foreach (UIEmpireBuildingFinished building in listBuidingFinished)
            {
                building.OnCustom();
            }
        }
        else
        {
            recycle.gameObject.SetActive(false);
            foreach (UIEmpireBuildingFinished building in listBuidingFinished)
            {
                building.OnDeCustom();
            }
        }
    }

    public void RemoveBuilding(UIEmpireBuildingFinished building)
    {
        empireMap.OnRemoveBuilding(building);
        listBuidingFinished.Remove(building);
    }

    public void OnDropToRecycle()
    {
        if (buildingFinishedDrag != null)
        {
            UINoticeManager.OnButtonOK_click += OnAcceptRemoveBuilding;
            MessageBox.ShowDialog(GameManager.localization.GetText("Land_RemoveHosueConfirm"), UINoticeManager.NoticeType.YesNo);          
        }        
    }

    public void ShowSubPanel()
    {
        buildingSubPanel.gameObject.SetActive(true);
        btnExpand.gameObject.SetActive(false);
    }

    public void HideSubPanel()
    {
        buildingSubPanel.gameObject.SetActive(false);
        btnExpand.gameObject.SetActive(true);
    }

    public void OnOpenCell(UIEmpireCell cell)
    {
        _cellSelected = cell;
        _controller.SendOpenLandCell(_cellSelected.position);
    }

    public void OnButtonBack_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Empire, GameScenes.previousSence);
    }

    public void OnButtonExtend_Click()
    {
        int maxSize = 40;

        if (curLand.LandSizeX >= maxSize || curLand.LandSizeY >= maxSize)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Land_MaxSizeExpand"), UINoticeManager.NoticeType.Message);
            return;
        }

        UINoticeManager.OnButtonOK_click += OnExtend;
        MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Land_Extend"), LandConfig.EXPAND_GOLD), UINoticeManager.NoticeType.YesNo);
    }

    public void OpenInfo()
    {
        uiContaint.infoRoot.SetActive(true);
    }

    public void CloseInfo()
    {
        uiContaint.infoRoot.SetActive(false);
    }

    public void OnCheckBank_Click()
    {
        _controller.SendCheckBank(true);
    }

    #endregion
    
    #region Response from server
    public IEnumerator OnResponseExpandServer()
    {
        empireMap.Clear();
        empireMap.sizeMap = new Vector2(curLand.LandSizeX, curLand.LandSizeY);

        yield return StartCoroutine(empireMap.Init());
        
        setGoldSivler();
    }

    public void OnResponseDestroyHouse()
    {
        UIEmpireCursor.Instance.Hide(false);
        buildingFinishedDrag.OnDestroyBuilding();
        buildingFinishedDrag = null;
        houseDataSelected = null;
        empireMap.CanNotBuilding();

        UIEmpireCursor.Instance.Hide(false);
        houseDataSelected = null;
        empireMap.CanNotBuilding();

        //foreach (UIEmpireBuildingFinished building in listBuidingFinished)
        //{
        //    GameObject.Destroy(building.gameObject);
        //}
        //listBuidingFinished.Clear();
        //StartCoroutine(InitOldBuiding());
        OnResponseOpenCell();
        DroptoTrash = false;
    }

    public void OnResponseCheckBank(float deltaSilver)
    {
        if (deltaSilver <= 0)
        {
        }

        if (curLand.Houses.Count(p => p.HouseId == 1) > 0 && !uiContaint.silverOfBank.activeInHierarchy)
        {
            uiContaint.silverOfBank.SetActive(true);
        }

        uiContaint.txtSilverOfBank.text = ((int)GameManager.GameUser.Land.BankSilver).ToString();

        setGoldSivler();
    }

    public void OnBuilHouseSuccess()
    {
        //if (empireMap.cellMouseOver != null && houseDataSelected != null)
        //{
        //    Vector2 buildingSize = new Vector2(houseDataSelected.SizeX, houseDataSelected.SizeY);
        //    Vector3 centerPos = empireMap.GetCenterPosition(empireMap.cellMouseOver, buildingSize);
        //    CreateBuilddingAtPos(centerPos, houseDataSelected, empireMap.cellMouseOver.position);
        //    empireMap.OnFinishedBuilding();
        //}

        UIEmpireCursor.Instance.Hide(false);
        houseDataSelected = null;
        empireMap.CanNotBuilding();

        //foreach (UIEmpireBuildingFinished building in listBuidingFinished)
        //{
        //    GameObject.Destroy(building.gameObject);
        //}
        //listBuidingFinished.Clear();

        // StartCoroutine(InitOldBuiding());
        //foreach (UserHouse userHouse in curLand.Houses)
        //{
        //    if (listBuidingFinished.FirstOrDefault(p => p.userHouse.UID == userHouse.UID) == null)
        //    {
        UserHouse userHouse = curLand.Houses[curLand.Houses.Count - 1];
        Vector2 sizeBuilding = new Vector2(userHouse.SizeX, userHouse.SizeY);
        Vector2 posBuilding = new Vector2(userHouse.PosX, userHouse.PosY);
        posBuilding = PosServerToClient(posBuilding, sizeBuilding);
        Vector3 centerPos = empireMap.GetCenterPosition(empireMap.GetCell(posBuilding), sizeBuilding);
        CreateBuilddingAtPos(centerPos, userHouse, posBuilding);

        //}
        //}

        empireMap.OnFinishedBuilding();
        OnResponseOpenCell();

    }

    public void OnResponseOpenCell()
    {
        //_cellSelected.UnLock();
        StartCoroutine(empireMap.RefreshCells());

        setGoldSivler();
    }
    #endregion
    
    #region private
    private void InitLand()
    {
        if (isFriend)
        {
            curLand = friendLand;
            slidePoint.gameObject.SetActive(false);
            buildingPanel.gameObject.SetActive(false);
            togCustomBuilding.gameObject.SetActive(false);
            btnExpand.gameObject.SetActive(false);
            uiContaint.silverOfBank.SetActive(false);
            uiContaint.panelInfo.SetActive(false);
        }
        else
        {
            curLand = GameManager.GameUser.Land;

            if (curLand.Houses.Count(p => p.HouseId == 1) > 0)
            {
                uiContaint.silverOfBank.SetActive(true);
                uiContaint.txtSilverOfBank.text = curLand.BankSilver.ToString();
            }

            AnimationSilverBank(uiContaint.silverOfBank);

            slidePoint.gameObject.SetActive(true);
            buildingPanel.Init(this);
            _controller.SendCheckBank(false);

            LoadUserInfo();

        }

        slidePoint.value = Mathf.Lerp(0, 1, ((float)(30 - curLand.ClosePoint)) / 30.0f);
        Color c = Color.green;
        if (slidePoint.value < 0.3)
        {
            c = Color.red;
        }
        else if (slidePoint.value < 0.6)
        {
            c = Color.yellow;
        }
        imageSlidePoint.color = c;
        empireMap.sizeMap = new Vector2(curLand.LandSizeX, curLand.LandSizeY);
    }

    private void AnimationSilverBank(GameObject obj)
    {
        Hashtable hash = new Hashtable();
        hash["scale"] = new Vector3(1.1f, 1.1f, 1.1f);
        hash["time"] = 1.0f;
        hash["islocal"] = true;
        hash["easetype"] = iTween.EaseType.linear;
        hash["looptype"] = iTween.LoopType.pingPong;
        
        iTween.ScaleTo(obj, hash);
    }

    private void OnAcceptRemoveBuilding()
    {
        if (buildingFinishedDrag != null)
        {
            DroptoTrash = true;
            _controller.SendDestroyHouse(buildingFinishedDrag.userHouse.UID, new Vector2(buildingFinishedDrag.userHouse.PosX, buildingFinishedDrag.userHouse.PosY));
        }
        
    }

    private void Localization()
    {
        uiContaint.txtToggleEdit.text = GameManager.localization.GetText("Land_ButtonEdit");
        uiContaint.txtButtonExpand.text = GameManager.localization.GetText("Land_Expand");
        uiContaint.txtInfo.text = Helper.StringToMultiLine(GameManager.localization.GetText("Land_Help"));
    }

    private IEnumerator InitOldBuiding()
    {
        int i = 0;
        foreach (UserHouse userHouse in curLand.Houses)
        {
            Vector2 sizeBuilding = new Vector2(userHouse.SizeX, userHouse.SizeY);
            Vector2 posBuilding = new Vector2(userHouse.PosX, userHouse.PosY);
            posBuilding = PosServerToClient(posBuilding, sizeBuilding);
            Vector3 centerPos = empireMap.GetCenterPosition(empireMap.GetCell(posBuilding), sizeBuilding);
            CreateBuilddingAtPos(centerPos, userHouse, posBuilding);
            // empireMap.OnFinishedBuilding(oldBuilding.posCell, uibuilding);
            i++;

            if (i % 10 == 0)
                yield return null;
        }
        yield return null;
    }

    private void CreateBuilddingAtPos(Vector3 pos, UserHouse userHouse, Vector2 posInMap)
    {
        GameObject go = GameObject.Instantiate(BuidingFinishedPrefab) as GameObject;
        go.transform.SetParent(BuidingFinishedRoot.transform);
        go.transform.localPosition = pos;
        go.transform.localScale = new Vector3(1, 1, 1);

        float depth = Mathf.Abs(Vector3.Distance(go.transform.position, Camera.main.transform.position));
        go.name = string.Format("Building_{0:D4}", (int)(depth));

        UIEmpireBuildingFinished bdf = go.GetComponent<UIEmpireBuildingFinished>();
        go.SetActive(true);



        bdf.Init(userHouse);

        listBuidingFinished.Add(bdf);

        SortDepthListBuilding();


        if (userHouse.HouseId == 1)
        {
            uiContaint.silverOfBank.transform.position = go.transform.position + Vector3.up * 50;
        }
    }

    private void SortDepthListBuilding()
    {
        listBuidingFinished.Sort((t1, t2) => t2.name.CompareTo(t1.name));
        for (int i = 0; i < listBuidingFinished.Count; i++)
        {
            listBuidingFinished[i].transform.SetSiblingIndex(i);

        }
    }

    private void OnExtend()
    {
        _controller.SendExpandLand();
    }

    private void LoadUserInfo()
    {
        uiUserInfo.lvlNickName.text = GameManager.GameUser.Base.NickName;
        uiUserInfo.lvlLevel.text = GameManager.GameUser.Base.Level.ToString("D2");

        float exp = (float)GameManager.GameUser.Base.Exp / (float)UserConfig.LEVELS_EXP[GameManager.GameUser.Base.Level];

        uiUserInfo.lvlEXP.text = string.Format("{0:0}%", exp * 100.0f);
        uiUserInfo.exp.value = exp;

        uiUserInfo.avatar.mainTexture = Helper.LoadTextureForAvatar(GameManager.GameUser.Base.Avatar);

        setGoldSivler();

    }

    private void setGoldSivler()
    {
        if (GameManager.GameUser != null)
        {
            uiUserInfo.lblGold.text = GameManager.GameUser.Base.Gold.ToString();
            uiUserInfo.lblSilver.text = GameManager.GameUser.Base.Silver.ToString();
        }
    }
    #endregion

    #region Static
    public static Vector2 PosClientToServer(Vector2 posOfCell, Vector2 sizeOfBuilding)
    {
        return new Vector2(posOfCell.x - (int)(sizeOfBuilding.x / 2), posOfCell.y - (int)(sizeOfBuilding.y / 2));
    }
    public static Vector2 PosServerToClient(Vector2 posOfCell, Vector2 sizeOfBuilding)
    {
        return new Vector2(posOfCell.x + (int)(sizeOfBuilding.x / 2), posOfCell.y + (int)(sizeOfBuilding.y / 2));
    }
    #endregion

    #region UIInfoBuilding
    public void ShowInfo(UIEmpireBuilding building)
    {
        houseInfo.icon.sprite = Helper.LoadSpriteForBuilding(building.houseData.Id);

        MyLocalization.HouseInfo houseInfoText = GameManager.localization.getHouseInfo(building.houseData.Id);

        houseInfo.root.SetActive(true);

        houseInfo.txtName.text = houseInfoText.Name;

        houseInfo.txtMax.text = string.Format("Max: {0} House", building.houseData.MaxCount);

        if (building.houseData.Gold > 0)
        {
            houseInfo.silverRoot.SetActive(false);
            houseInfo.goldRoot.SetActive(true);
            houseInfo.txtGold.text = building.houseData.Gold.ToString();
        }

        if (building.houseData.Silver > 0)
        {
            houseInfo.goldRoot.SetActive(false);
            houseInfo.silverRoot.SetActive(true);
            houseInfo.txtSilver.text = building.houseData.Silver.ToString();
        }

        houseInfo.txtSize.text = string.Format("Size: {0}x{1}", building.houseData.SizeX, building.houseData.SizeY);


        houseInfo.txtDesc.text = Helper.StringToMultiLine(string.Format(houseInfoText.Desc, building.houseData.Lost));

        houseInfo.rectDesc.localPosition = new Vector3(0, -1000, 0);
    }
    public void OnButtonCloseInfo_Click()
    {
        houseInfo.root.SetActive(false);
    }
    #endregion
}
