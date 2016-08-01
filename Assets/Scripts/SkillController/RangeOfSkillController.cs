using UnityEngine;
using System.Collections;

public class RangeOfSkillController : MonoBehaviour {

    public float range = 1;
    
    public void setPosition(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, 0.2f, pos.z);
    }
    public void setLocalPos(Vector3 pos)
    {
        transform.localPosition = new Vector3(pos.x, 0.2f, pos.z);
    }
    public void setRange(float range)
    {
        range *= 2;
        transform.localScale = new Vector3(range, range, 1);
    }  
}
