using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventPress : MonoBehaviour {


    public UnityEvent onLongPress = new UnityEvent();

    private bool isPointerDown = false;


    private void Update()
    {
        if (isPointerDown)
        {
            onLongPress.Invoke();
        }
    }

    public void OnPointerDown()
    {
        isPointerDown = true;
    }

    public void OnPointerUp()
    {
        isPointerDown = false;
    }
    
    public void OnPointerExit()
    {
        isPointerDown = false;
    }
}
