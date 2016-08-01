using UnityEngine;
using System.Collections;

public class TextDamageLableController : MonoBehaviour
{

    private Transform mTransform;
    public float speedUp = 2;
    public float timeDestroy=0.7f;
    void Start()
    {
        mTransform = transform;
        Destroy(gameObject, timeDestroy);
        if (speedUp <= 0)
        {
            rigidbody.AddForce((Vector3.up + Vector3.right) * 200f);
        }
    }

    void Update()
    {
        mTransform.Translate(Vector3.up * Time.deltaTime * speedUp);
    }
}
