using UnityEngine;
using System.Collections;

public class WayPointController : MonoBehaviour {

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.01f);
    }
}
