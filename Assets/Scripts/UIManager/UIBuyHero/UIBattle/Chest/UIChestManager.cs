using UnityEngine;
using System.Collections;

public class UIChestManager : MonoBehaviour {

    public UILabel lblAmount;
    public UISprite sprite;
    public UISpriteAnimation animation;
    public GameObject chestDrop;
    public Camera uiCamera;


    private static UIChestManager _instance;
    public static UIChestManager Instance { get { return _instance; } }

    private int _amount = 0;
    void Start()
    {
        _amount = 0;
        _instance = this;

        uiCamera = NGUITools.FindCameraForLayer(chestDrop.layer);
    }

   

    public void CreateDropChest(int type, Vector3 pos)
    {
        string spriteName="";

        if (type == 3)
            spriteName = "ruong_vang";
        else if (type == 2)
            spriteName = "ruong_bac";
        else
            spriteName = "ruong_gho";



      

        GameObject go = NGUITools.AddChild(gameObject, chestDrop);
        go.transform.localPosition = WorldToScreenPos(pos, go.transform);
        go.transform.GetChild(0).GetComponent<UISprite>().spriteName = spriteName;
        go.SetActive(true);
    }


    public void OnDestroyChest()
    {
        _amount++;
        animation.enabled = true;
        lblAmount.text = string.Format("x{0}", _amount);
        Invoke("StopSharkChest", 0.5f);
    }

    private void StopSharkChest()
    {
        animation.enabled = false;
        sprite.spriteName = "ruonglac_1";
    }

    private Vector3 WorldToScreenPos(Vector3 worldPos,Transform chestTransform)
    {

        Vector3 pos;
        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        
        chestTransform.position = uiCamera.ViewportToWorldPoint(viewPos);
        pos = chestTransform.localPosition;
        pos.x = Mathf.FloorToInt(pos.x);
        pos.y = Mathf.FloorToInt(pos.y);
        pos.z = 0f;
        return pos;

    }
}

