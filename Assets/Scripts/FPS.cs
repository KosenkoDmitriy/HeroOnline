using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {


    private float _fps;

    void OnGUI()
    {
        //GUI.color = Color.red;
        //GUI.Label(new Rect(0, 0, 100, 20), string.Format("FPS: {0}", _fps));
        //GUI.color = Color.white;
    }

    void Update()
    {       
        _fps = 1.0f / Time.smoothDeltaTime;
        //Debug.Log(_fps);
    }
}
