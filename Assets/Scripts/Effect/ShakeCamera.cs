using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour {

    public Transform camTransform;

    public float shake = 0f;
    
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    private Vector3 originalPos;
    private bool playing;

    void Awake()
    {
        playing = false;
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (playing)
        {
            if (shake > 0)
            {
                camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

                shake -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shake = 0f;
                camTransform.localPosition = originalPos;
                playing = false;
            }
        }
    }

    public void Play(float time, float amout)
    {
        shake = time;
        shakeAmount = amout;
        playing = true;
    }
}
