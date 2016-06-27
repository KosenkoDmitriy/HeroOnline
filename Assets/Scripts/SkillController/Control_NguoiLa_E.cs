using UnityEngine;
using System.Collections;

public class Control_NguoiLa_E : MonoBehaviour {
    public float lifeTime=3;
    public Vector3 hitPoint;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, lifeTime);
        transform.position = hitPoint;
	}
//    void OnTriggerEnter(Collider other)
//    {
//		if(other.GetComponent<Character>())
//		{		
//			other.GetComponent<Character>().CollideWithSkills(transform.root);
//		}
//    }
	// Update is called once per frame
	void Update () {
	
	}
}
