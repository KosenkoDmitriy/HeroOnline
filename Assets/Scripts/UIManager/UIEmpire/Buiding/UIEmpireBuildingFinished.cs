using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DEngine.Common.GameLogic;

public class UIEmpireBuildingFinished : MonoBehaviour {

    public Image icon;
    public CanvasGroup canvasGroup;

    public Vector2 size = new Vector2(40, 40);


    //[HideInInspector]
    //public UIEmpireBuilding _empireBuilding;
    //[HideInInspector]
    //public Vector2 posInMap;
    [HideInInspector]
    public UserHouse userHouse;

    public void Init(UserHouse _userHouse)
    {
        userHouse = _userHouse;
       // _empireBuilding = empireBuilding;

        Sprite sprite = Helper.LoadSpriteForBuilding(userHouse.HouseId);
        icon.sprite = sprite;

        float widthRatio = (float)sprite.texture.width / (float)sprite.texture.height;

        float sizeRatio = (userHouse.SizeX + userHouse.SizeY) / 2.0f;

        Vector2 sizeResult = new Vector2(size.x * sizeRatio * widthRatio, size.y * sizeRatio);

        RectTransform rect = icon.GetComponent<RectTransform>();
        rect.sizeDelta = sizeResult;
        Vector3 pos = rect.localPosition;
        pos.y = sizeResult.y / 4f;
        rect.localPosition = pos;

        if (UIEmpireManager.Instance.togCustomBuilding.isOn)
        {
            OnCustom();
        }
        else
        {
            OnDeCustom();
        }
    }

    public void OnCustom()
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDeCustom()
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDragStart()
    {
        Debug.Log("OnDragStart");
        icon.color = new Color(1, 1, 1, 0);
        UIEmpireManager.Instance.OnDragBuildingFinished(this);
    }

    public void OnDropEnd()
    {
        if (UIEmpireManager.Instance.DroptoTrash) return;

        Debug.Log("OnDrop");
        
        bool isCreated = UIEmpireManager.Instance.OnDropBuilding();

        if (!isCreated)
        {
            icon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            OnDestroyBuilding();
        }
    }

    public void OnDestroyBuilding()
    {
        UIEmpireManager.Instance.RemoveBuilding(this);
        Destroy(gameObject);
    }
}
