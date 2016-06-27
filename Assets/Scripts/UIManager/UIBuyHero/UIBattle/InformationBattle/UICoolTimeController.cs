using UnityEngine;
using System.Collections;

public class UICoolTimeController : MonoBehaviour {
    	
    public void OnFinishedAnim()
    {
        Destroy(gameObject);
    }
}
