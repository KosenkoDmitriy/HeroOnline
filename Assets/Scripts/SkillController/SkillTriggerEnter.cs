using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Collections.Generic;

public class SkillTriggerEnter : MonoBehaviour {

    public SkillRootManager skillRootManager;
    
    private GameObject[] enemys;
    private Transform _transform;

    private List<string> collderNames;

    void Start()
    {
        string tagName = Helper.GetTagEnemy(skillRootManager.controller);

        enemys = GameObject.FindGameObjectsWithTag(tagName);

        _transform = transform;
        collderNames = new List<string>();
    }

    void Update()
    {
        foreach (GameObject go in enemys)
        {
            if (go != null && _transform != null)
            {
                if (Vector3.Distance(go.transform.position, _transform.position) < skillRootManager.distance)
                {
                    if (!collderNames.Contains(go.name))
                    {
                        collderNames.Add(go.name);
                        skillRootManager.OnSkillColiider(go);
                    }
                }
            }
        }



    }

}
