using DEngine.Common.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectSettings : MonoBehaviour
{

    public float ColliderRadius = 0.2f;
    public float EffectRadius = 0;
    public GameObject Target;
    public float MoveSpeed = 1;
    public float MoveDistance = 20;
    public bool IsHomingMove;
    public bool IsVisible = true;
    public bool DeactivateAfterCollision = true;
    public float DeactivateTimeDelay = 4;

    public event EventHandler<CollisionInfo> CollisionEnter;
    public event EventHandler EffectDeactivated;

    private Dictionary<GameObject, float> activeGo = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> inactiveGo = new Dictionary<GameObject, float>();
    private int currentActiveGo;
    private int currentInactiveGo;
    private bool deactivatedIsWait;


    public RoleSkill gameSkill { get; set; }
    public Controller controller { get; set; }
    public AudioClip audioEffect { get; set; }
    public int skillIndex { get; set; }

    public bool noCollider = false;

    public float ShakeCameraTime = 0f;
    public float ShakeCameraAmount = 0f;

    public float eForceHight = 0;
    public float eForceAmount = 0;
    
    public void HandleCollider(GameObject col)
    {

        if (ShakeCameraTime > 0)
        {
            ShakeCamera shakeCamera = Camera.main.GetComponent<ShakeCamera>();
            if (shakeCamera != null)
            {
                shakeCamera.Play(ShakeCameraTime, ShakeCameraAmount);
            }
        }

        if (col == null) return;

        Controller targetController = col.GetComponent<Controller>();

        if (targetController == null) return;
        
        if (!noCollider)
            SkillCollider.HandleSkillHit(col, controller, skillIndex, TargetType.EnemyOne);
              

        if (eForceAmount > 0)
        {
            Vector3 dir = targetController.transform.position - controller.transform.position;
            dir.y = 0;
            dir = dir.normalized;
            
            Vector3 addForce = (dir + Vector3.up * eForceHight).normalized;
            targetController.AddForce(addForce * eForceAmount);
        }


        if (audioEffect != null)
        {
            AudioSource.PlayClipAtPoint(audioEffect, Vector3.zero);
        }
    }

    public void OnCollisionHandler(CollisionInfo e)
    {

        if (e.Hit.collider != null)
            HandleCollider(e.Hit.collider.gameObject);


        foreach (var activeElemet in activeGo)
        {
            Invoke("SetGoActive", activeElemet.Value);
        }
        foreach (var inactiveElemet in inactiveGo)
        {
            Invoke("SetGoInactive", inactiveElemet.Value);
        }
        var handler = CollisionEnter;
        if (handler != null)
            handler(this, e);
        if (DeactivateAfterCollision && !deactivatedIsWait)
        {
            deactivatedIsWait = true;
            Invoke("Deactivate", DeactivateTimeDelay);
        }
    }
    public void OnEffectDeactivatedHandler()
    {
        var handler = EffectDeactivated;
        if (handler != null)
            handler(this, EventArgs.Empty);
    }

    private void Deactivate()
    {
        OnEffectDeactivatedHandler();
        gameObject.SetActive(false);
    }

    private void SetGoActive()
    {
        activeGo.ElementAt(currentActiveGo).Key.SetActive(false);
        ++currentActiveGo;
        if (currentActiveGo >= activeGo.Count)
            currentActiveGo = 0;
    }

    private void SetGoInactive()
    {
        inactiveGo.ElementAt(currentInactiveGo).Key.SetActive(true);
        ++currentInactiveGo;
        if (currentInactiveGo >= inactiveGo.Count)
            currentInactiveGo = 0;
    }

    public void OnEnable()
    {
        foreach (var activeElemet in activeGo)
        {
            activeElemet.Key.SetActive(true);
        }
        foreach (var inactiveElemets in inactiveGo)
        {
            inactiveElemets.Key.SetActive(false);
        }
        deactivatedIsWait = false;
    }

    public void OnDisable()
    {
        CancelInvoke("SetGoActive");
        CancelInvoke("SetGoInactive");
        CancelInvoke("Deactivate");
        currentActiveGo = 0;
        currentInactiveGo = 0;
    }

    public void RegistreActiveElement(GameObject go, float time)
    {
        activeGo.Add(go, time);
        activeGo = activeGo.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }

    public void RegistreInactiveElement(GameObject go, float time)
    {
        inactiveGo.Add(go, time);
        inactiveGo = inactiveGo.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }
}

public class CollisionInfo : EventArgs
{
    public RaycastHit Hit;
}