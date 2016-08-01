using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITextManager : MonoBehaviour {


    public GameObject Root;
    public GameObject TextPrefab;

    private static UITextManager _instance;
    public static UITextManager Instance
    {
        get { return _instance; }
    }

    private float _preTime;
    private Queue<string> textSet;

	void Start () {
        _instance = this;
        _preTime = Time.time;
        textSet = new Queue<string>();
	}

    void Update()
    {
        if (textSet.Count > 0)
        {
            float eslapTime = Time.time - _preTime;
            if (eslapTime >= 2f)
            {
                GameObject go = NGUITools.AddChild(Root, TextPrefab);
                go.GetComponent<UILabel>().text = textSet.Dequeue();
                go.SetActive(true);

                _preTime = Time.time;
            }
        }
    }

    public void createText(string s)
    {
        textSet.Enqueue(s);
    }

   

   

}
