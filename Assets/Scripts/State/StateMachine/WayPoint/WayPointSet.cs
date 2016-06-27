using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WayPointSet : MonoBehaviour {

    private static WayPointSet _instance;
    public static WayPointSet Instance { get { return _instance; } }

    public GameObject wayPointPrefab;

    private WayPointController[] _wayPointSet;
     

    void Start()
    {
        _instance = this;

        _wayPointSet = transform.GetComponentsInChildren<WayPointController>();

        /* Vector3 _offeset = new Vector3(-6, 0, -6);
         int count = 0;
         for (int i = 0; i < 10; i++)
         {
             for (int j = 0; j < 5; j++)
             {
                 GameObject go = GameObject.Instantiate(wayPointPrefab) as GameObject;
                 go.transform.parent = transform;
                 go.transform.localPosition = new Vector3((i * 1.3f + _offeset.x), 0, (j * 1.4f + _offeset.z));
                 go.name = "waypoint_" + count;
                 count++;
             }
         }*/
    }

    public WayPointController FindWayPointRandom(Controller controller)
    {
        WayPointController waypoint = null;
        int index = Random.Range(0, _wayPointSet.Length);
        waypoint = _wayPointSet[index];
        return waypoint;
    }

    public WayPointController FindWayPointNoEnemy(Controller controller)
    {
        WayPointController waypoint = null;


        HeroSet enemies = GameplayManager.Instance.getEnemySet(controller);      


        float max = float.MinValue;

        foreach (WayPointController wp in _wayPointSet)
        {
            float distace = 0;

            foreach (Hero hero in enemies)
            {
                Controller c = hero.controller;

                if (c.gameObject != null)
                {
                    if (c.typeCharacter != Controller.TypeChatacter.Healer)
                    {
                        float distanceToEnemy = 0;
                        distanceToEnemy = Vector3.Distance(wp.transform.position, c.transform.position);

                        if (c.typeCharacter == Controller.TypeChatacter.Range)
                        {
                            distanceToEnemy *= 2;
                        }

                        distace += distanceToEnemy;
                    }
                }
            }

            if (distace > max)
            {
                max = distace;
                waypoint = wp;
            }
        }

        return waypoint;
    }
   
}
