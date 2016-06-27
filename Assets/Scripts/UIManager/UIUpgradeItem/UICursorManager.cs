using UnityEngine;
using System.Collections;

public class UICursorManager : MonoBehaviour {

    static UICursorManager mInstance;
	public static UICursorManager Instance {
		get {return mInstance;}
	}

    Transform mTrans;
    public UISprite mSprite;

    void Awake() { mInstance = this; }
    void OnDestroy() { mInstance = null; }
    public Camera uiCamera;

    void Start()
    {
        mTrans = transform;
        mSprite.depth = 100;
        if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
    }

    void Update()
    {
        if (mSprite == null) return;
        if (mSprite.mainTexture != null)
        {
            Vector3 pos = Input.mousePosition;
                        
            pos.x = Mathf.Clamp01(pos.x / Screen.width);
            pos.y = Mathf.Clamp01(pos.y / Screen.height);
            mTrans.position = uiCamera.ViewportToWorldPoint(pos);
        }
    }

    static public void Clear()
    {
        if (mInstance != null)
        {
            mInstance.mSprite.gameObject.SetActive(false);
        }
    }



    static public void Set(string spriteName)
    {
        if (mInstance != null)
        {
            mInstance.mSprite.gameObject.SetActive(true);
            mInstance.mSprite.spriteName = spriteName;
            mInstance.Update();
        }
    }
}
