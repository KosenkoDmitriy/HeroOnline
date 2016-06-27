using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIEmpireCursor : MonoBehaviour {

    public Image image;

    private Vector2 _startPos;
    private bool _isDrag = false;

    public static UIEmpireCursor Instance { get; private set; }

    void Start()
    {
        Instance = this;
    }

    public void Show(Sprite icon)
    {
        _isDrag = true;
        image.gameObject.SetActive(true);
        image.sprite = icon;
        //image.SetNativeSize();
        _startPos = Helper.GetCursorPos();
    }

    public void Hide(bool reverse)
    {
        _isDrag = false;
        if (!reverse)
            image.gameObject.SetActive(false);
        else
            StartCoroutine(MoveToStartPos());
    }

    public void SetColor(Color c)
    {
        c.a = 0.4f;
        image.color = c;
    }

    private IEnumerator MoveToStartPos()
    {
        Debug.Log("MoveToStartPos");

        while (Vector3.Distance(_startPos, image.gameObject.transform.position) > 10)
        {
            Vector3 pos = Vector3.Lerp(image.gameObject.transform.position, _startPos, Time.deltaTime * 4);
            image.gameObject.transform.position = pos;
            yield return null;
        }

        image.gameObject.SetActive(false);
    }

        

	void Update () {
        if (_isDrag)
        {
            Vector3 pos = Helper.GetCursorPos();

            image.transform.position = pos;
        }
	}

   
}
