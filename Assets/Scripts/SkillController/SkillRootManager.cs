using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class SkillRootManager : MonoBehaviour {

    public float distance = 1;
    public RoleSkill gameSkill { get; set; }
    public Controller controller { get; set; }
    public AudioClip audioEffect { get; set; }
    public int skillIndex { get; set; }

    public void OnSkillColiider(GameObject other)
    {
        if (other != null)
        {
            Controller con = other.GetComponent<Controller>();
            if (con != null && con.tag != controller.tag)
            {
                SkillCollider.HandleSkillHit(other.gameObject, controller, skillIndex, DEngine.Common.GameLogic.TargetType.EnemyOne);
            }
        }
    }
}
