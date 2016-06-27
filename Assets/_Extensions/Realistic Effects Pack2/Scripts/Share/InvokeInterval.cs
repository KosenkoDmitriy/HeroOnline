using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class InvokeInterval : MonoBehaviour
{

    public GameObject GO;
    public float Interval = 0.3f;
    public float Duration = 3;

    private List<GameObject> goInstances;
    private EffectSettings effectSettings;
    private int goIndexActivate, goIndexDeactivate;
    private bool isInitialized;
    private int count;
    // Use this for initialization

    void GetEffectSettingsComponent(Transform tr)
    {
        var parent = tr.parent;
        if (parent != null)
        {
            effectSettings = parent.GetComponentInChildren<EffectSettings>();
            if (effectSettings == null)
                GetEffectSettingsComponent(parent.transform);
        }
    }

    void Start()
    {
        StartCoroutine(CreateBullet());
    }

    private IEnumerator CreateBullet()
    {
        GetEffectSettingsComponent(transform);
        goInstances = new List<GameObject>();
        count = (int)(Duration / Interval);
        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(GO, transform.position, new Quaternion()) as GameObject;
            go.transform.parent = transform;
            var es = go.GetComponent<EffectSettings>();
            es.Target = effectSettings.Target;
            es.IsHomingMove = effectSettings.IsHomingMove;
            es.MoveDistance = effectSettings.MoveDistance;
            es.MoveSpeed = effectSettings.MoveSpeed;
            es.DeactivateAfterCollision = effectSettings.DeactivateAfterCollision;
            es.DeactivateTimeDelay = effectSettings.DeactivateTimeDelay;
            es.ColliderRadius = effectSettings.ColliderRadius;
            es.EffectRadius = effectSettings.EffectRadius;
            es.EffectDeactivated += effectSettings_EffectDeactivated;

            es.gameSkill = effectSettings.gameSkill;
            es.controller = effectSettings.controller;
            es.skillIndex = effectSettings.skillIndex;
            es.audioEffect = effectSettings.audioEffect;
            es.ShakeCameraAmount = effectSettings.ShakeCameraAmount;
            es.ShakeCameraTime = effectSettings.ShakeCameraTime;

            if (i > 0) es.noCollider = true;
            goInstances.Add(go);
            go.SetActive(false);
            yield return null;
        }
        StartCoroutine(InvokeAll());
        isInitialized = true;
    }

    private IEnumerator InvokeAll()
    {
        for (int i = 0; i < count; i++)
        {
            Invoke("InvokeInstance", i * Interval);
            yield return null;
        }
    }

    void InvokeInstance()
    {
        goInstances[goIndexActivate].SetActive(true);
        if (goIndexActivate >= goInstances.Count - 1)
            goIndexActivate = 0;
        else
            goIndexActivate++;
    }

    void effectSettings_EffectDeactivated(object sender, EventArgs e)
    {
        var go = sender as EffectSettings;
        go.transform.position = transform.position;
        if (goIndexDeactivate >= count - 1)
        {
            effectSettings.OnCollisionHandler(new CollisionInfo());
            goIndexDeactivate = 0;
        }
        else
            goIndexDeactivate++;
    }


    private void OnEnable()
    {
        if (isInitialized)
        {
            InvokeAll();
        }
    }

    private void OnDisable()
    {

    }
}
