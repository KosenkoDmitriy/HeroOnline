using UnityEngine;
using System.Collections;

public class Control_Boom : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 3);
	}
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit Control_Boom_Sogoku_E");
    }
	// Update is called once per frame
	void Update () {
	
	}
}
