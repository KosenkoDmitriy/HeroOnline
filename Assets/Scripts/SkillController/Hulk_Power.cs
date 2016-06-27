using UnityEngine;
using System.Collections;

public class Hulk_Power : MonoBehaviour {
    public float TimeDestroy=3;
    public RenderSettings halo;

    public float ShakeCameraTime = 0f;
    public float ShakeCameraAmount = 0f;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, TimeDestroy);
        ShakeCamera shakeCamera = Camera.main.GetComponent<ShakeCamera>();
        if (shakeCamera != null)
        {
            if (ShakeCameraTime > 0)
                shakeCamera.Play(ShakeCameraTime, ShakeCameraAmount);
        }
	}
	
	// Update is called once per frame
	void Update () {

	}
}
