using UnityEngine;
using System.Collections;

public class UITextController : MonoBehaviour {

    public float timeLife = 8;
    public float speed = 0.5f;

    private Transform _transform;

	void Start () {
        Destroy(gameObject, timeLife);
        _transform = transform;
	}
	
	void Update () {
        _transform.Translate(Vector3.left * Time.deltaTime * speed);
	}
}
