using UnityEngine;
using System.Collections;

public class NhaSu_Skill2Controller : MonoBehaviour {

    public Transform bellRoot;
    public Transform[] wayPoints;
    public int wayPointIndex = 0;
    public float speed=5;
    public bool isPlay = false;
    
    //private Transform mTransform;
    public void StartSkill()
    {
       // mTransform = transform;
        isPlay = true;
        StartCoroutine(UpdateSkill(0.016f));
	}

    void OnDestroy()
    {
        StopCoroutine("UpdateSkill");
    }

    IEnumerator UpdateSkill(float waitTime)
    {
        while (true)
        {
            if (!isPlay) yield return null;

            float distance = Vector3.Distance(wayPoints[wayPointIndex].position, bellRoot.position);
            if (distance <= 0.1f)
                wayPointIndex++;
            if (wayPointIndex >= wayPoints.Length) wayPointIndex = 0;

            Quaternion rotation = Quaternion.LookRotation(wayPoints[wayPointIndex].position - bellRoot.position);
            rotation = Quaternion.Lerp(bellRoot.rotation, rotation, Time.deltaTime * 5);
            bellRoot.rotation = rotation;
            bellRoot.Translate(Vector3.forward * Time.deltaTime * speed);

            yield return new WaitForSeconds(waitTime);
        }
	}

    public Transform GetTransform()
    {
        return bellRoot.transform;
    }
}
