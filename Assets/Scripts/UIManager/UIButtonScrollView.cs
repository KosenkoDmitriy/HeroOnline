using UnityEngine;
using System.Collections;

public class UIButtonScrollView : MonoBehaviour {

    public UIScrollView scrollView;
    public Vector3 direction = Vector3.zero;
    public float Speed = 2;
    private bool _isPress;
    public bool verticle = true;

    void Update()
    {
        if (_isPress)
        {           
            scrollView.MoveRelative(direction * Speed);
            scrollView.RestrictWithinBounds(false, !verticle, verticle);         
        }
    }

    void OnPress(bool isPress)
    {
        _isPress = isPress;
    }
}
