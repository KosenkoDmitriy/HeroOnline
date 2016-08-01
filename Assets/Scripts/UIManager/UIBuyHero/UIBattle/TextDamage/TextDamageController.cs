using UnityEngine;
using System.Collections;

public class TextDamageController : MonoBehaviour {


	public UILabel label;
	public Vector2 offset;

    public Camera uiCamera;

    private static TextDamageController _instance;
    public static TextDamageController Instance
    {
        get
        {
            return _instance;
        }
    }


    public void Start()
    {
        _instance = this;

        uiCamera = NGUITools.FindCameraForLayer(label.gameObject.layer);
    }

    public void create(Vector3 position, float damage, Color color, bool isUp = false)
    {
        GameObject lblInstance = NGUITools.AddChild(gameObject, label.gameObject);
        UILabel lbl = lblInstance.GetComponent<UILabel>();
        lbl.transform.localPosition = WorldToScreenPos(position, lblInstance.transform);


        lbl.text = damage.ToString();

        lbl.color = Color.white;
        lbl.gradientTop = Color.white;
        lbl.gradientBottom = color;
        lbl.fontSize = 50;

        lblInstance.SetActive(true);

        if (isUp)
        {
            Rigidbody rigid = lblInstance.gameObject.GetComponent<Rigidbody>();
            rigid.drag = 0;
            rigid.useGravity = false;
            lblInstance.GetComponent<TextDamageLableController>().speedUp = 0.6f;
        }       
    }

    public void createText(Vector3 position, string message, Color color)
    {
        GameObject lblInstance = NGUITools.AddChild(gameObject, label.gameObject);
        UILabel lbl = lblInstance.GetComponent<UILabel>();
        lbl.transform.localPosition = WorldToScreenPos(position, lblInstance.transform);

        lbl.text = message.ToString();
        lbl.color = Color.white;
        lbl.gradientTop = Color.white;
        lbl.gradientBottom = color;
        lbl.fontSize = 70;
        lblInstance.SetActive(true);

       
        Rigidbody rigid = lblInstance.gameObject.GetComponent<Rigidbody>();
        rigid.drag = 0;
        rigid.useGravity = false;

        lblInstance.GetComponent<TextDamageLableController>().speedUp = 0.1f;
    }



    private Vector3 WorldToScreenPos(Vector3 worldPos, Transform textTransform)
    {

        Vector3 pos;
        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldPos);

        textTransform.position = uiCamera.ViewportToWorldPoint(viewPos);
        pos = textTransform.localPosition;
        pos.x = Mathf.FloorToInt(pos.x);
        pos.y = Mathf.FloorToInt(pos.y);
        pos.z = 0f;
        return pos;

    }
 
}














