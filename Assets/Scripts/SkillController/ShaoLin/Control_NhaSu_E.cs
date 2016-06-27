using UnityEngine;
using System.Collections;

public class Control_NhaSu_E : MonoBehaviour {

    public Vector3 HitPoint;
    public float TimeLife=3;
    private float TimeCount;

   // private ParticleSystem icon;

    void Awake() {
       // icon = transform.FindChild("Icon").GetComponent<ParticleSystem>();
    }

	public void StartSkill () {
        transform.position = HitPoint;
	}

    void OnTriggerEnter(Collider other)
    {
       

    }

	void Update () {
	if((TimeCount+=Time.deltaTime)>=TimeLife)
        {
            Destroy(gameObject);
        }
	}
}
