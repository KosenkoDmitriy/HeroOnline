using UnityEngine;
using System.Collections;
using Xft;

public class Songoku_SkillKamejoko : MonoBehaviour
{
    enum SkillState
    {
        None,
        directionTarget,
        Colission,
        Scaling,
        Explosion
    }
    public float speed;
    public XffectComponent MagicChain;
    public GameObject explosionPrefab;
    public GameObject bottomWindTObject;
    public Transform startTarget;
    public Transform endTarget;
    public Transform bottomTransform;
    public GameObject lineObject;
    public float width = 2f;
    public float timeWaitPlayExplosion = 1;
    public float Force = 500;
    public float ForceHeight = 2;

    public int skillIndex { get; set; }

    public float ShakeCameraTime_Start = 0f;
    public float ShakeCameraAmount_Start = 0f;
    public float ShakeCameraTime_Explosion = 0f;
    public float ShakeCameraAmount_Explosion = 0f;

    private Vector3 virtualTarget = Vector3.zero;
    private float startTimer;
    private float journeyLength;
    private bool isCollision;
    private bool isPlay = false;
    private SkillState curState;
    private GameObject otherCollision;

    public void StartSkill()
    {
        if (endTarget == null) return;

        curState = SkillState.directionTarget;
        transform.LookAt(endTarget);       
        isCollision = false;
        startTimer = Time.time;
        virtualTarget = startTarget.position;
        journeyLength = Vector3.Distance(startTarget.position, endTarget.position);
        isPlay = true;
        StartCoroutine(UpdateSkill(0.016f));

        ShakeCamera shakeCamera = Camera.main.GetComponent<ShakeCamera>();
        if (shakeCamera != null)
        {
            if (ShakeCameraTime_Start > 0)
                shakeCamera.Play(ShakeCameraTime_Start, ShakeCameraAmount_Start);
        }
    }

    void OnDestroy()
    {
        StopCoroutine("UpdateSkill");
    }

    private void directionTarget()
    {
        if (endTarget == null)
        {
            curState = SkillState.Colission;
            isCollision = true;
            return;
        }
        transform.position = startTarget.position;
        float distCovered = (Time.time - startTimer) * speed;
        float fracJourney = distCovered / journeyLength;

        virtualTarget = Vector3.Lerp(startTarget.position, endTarget.position + Vector3.up, fracJourney);

        bottomTransform.position = virtualTarget;

        float distance = (virtualTarget - startTarget.position).magnitude;
        Vector2 scale = new Vector2(width, distance);

        MagicChain.SetScale(scale, "line");

        MagicChain.SetScale(scale, "lightn");


        Vector3 direction = endTarget.position - startTarget.position;
        MagicChain.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        float distanceToEndTarget = Vector3.Distance(virtualTarget, endTarget.position + Vector3.up);

        if (distanceToEndTarget <= 0.5f && !isCollision)
        {
            OnCollisionEndTarget(endTarget.gameObject);
            isCollision = true;
        }
    }

    private void Colission()
    {
        lineObject.SetActive(false);
        bottomWindTObject.SetActive(false);
        curState = SkillState.Scaling;
    }

    private void Scaling()
    {
        if (endTarget == null)
        {
            bottomTransform.gameObject.SetActive(false);
            curState = SkillState.Explosion;
            return;
        }
        bottomTransform.position = endTarget.position;
        ParticleSystem particleSystem = bottomTransform.GetComponent<ParticleSystem>();
        particleSystem.startSize -= Time.deltaTime * 4;

        if (particleSystem.startSize <= 0.4f)
        {
            bottomTransform.gameObject.SetActive(false);
            curState = SkillState.Explosion;
        }
    }
    private void Explosion()
    {
        ShakeCamera shakeCamera = Camera.main.GetComponent<ShakeCamera>();
        if (shakeCamera != null)
        {
            if (ShakeCameraTime_Explosion > 0)
                shakeCamera.Play(ShakeCameraTime_Explosion, ShakeCameraAmount_Explosion);
        }

        GameObject go = GameObject.Instantiate(explosionPrefab, otherCollision.transform.position, Quaternion.identity) as GameObject;
        Destroy(go, 2);
       // Vector3 addForce = ((endTarget.position - startTarget.position).normalized + Vector3.up * ForceHeight).normalized;

        //otherCollision.GetComponent<Rigidbody>().AddForce(addForce * Force);
        //curState = SkillState.None;
        Destroy(gameObject);


    }

    IEnumerator UpdateSkill(float waitTime)
    {

        while (true)
        {
            if (!isPlay) yield return null;

            switch (curState)
            {
                case SkillState.directionTarget:
                    directionTarget();
                    break;
                case SkillState.Colission:
                    Colission();
                    break;
                case SkillState.Scaling:
                    Scaling();
                    break;
                case SkillState.Explosion:
                    Explosion();
                    break;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }


    void OnCollisionEndTarget(GameObject other)
    {      
        otherCollision = other;
        curState = SkillState.Colission;
        SkillCollider.HandleSkillHit(other, startTarget.GetComponent<Controller>(), skillIndex, DEngine.Common.GameLogic.TargetType.EnemyOne);
    }

   
}
