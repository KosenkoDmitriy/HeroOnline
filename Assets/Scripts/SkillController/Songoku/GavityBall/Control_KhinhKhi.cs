using UnityEngine;
using System.Collections;

public class Control_KhinhKhi : MonoBehaviour {
    public GravilyBallController control_e;
	// Use this for initialization
	void Start () {
	
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameManager.EnemyTagName)
        {
            control_e.setTrangThai(TrangThai.No);
            control_e.setHitPoint(transform.position);
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
