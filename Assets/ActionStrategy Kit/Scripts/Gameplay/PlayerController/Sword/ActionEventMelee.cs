/// <summary>
/// This script use for control state of character(Melee)
/// to attack,skill,call sound effect,call particle by using with Setthing Action(Script)
/// </summary>

using UnityEngine;
using System.Collections;

public class ActionEventMelee : MonoBehaviour
{

    public Controller controller;

    public void CastSkillFX(GameObject FX)
    {
        //Spawn particle fx when calling this method
        Instantiate(FX, this.transform.position, this.transform.rotation);
    }
    #region base
    public void Skill_1()
    {
        SkillManager.Instance.DoSkill(controller, 1);      
    }

    public void Skill_2()
    {
        SkillManager.Instance.DoSkill(controller, 2);
    }

    public void Attacking()
    {
        SkillManager.Instance.DoSkill(controller, 0);
    }

    public void CastSkill_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
        AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    public void Attacking_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
        //AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    public void Skill_1_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
        //AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    public void Skill_2_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
       // AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    public void WeaponSwing(AudioClip audio)
    {
        //calling sfx when calling this method
        //AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    private int AttackValue(float val)
    {
        //random value of healing power when calling this method
        float attackValue = Random.Range(val * 0.8f, val * 1.2f);
        return (int)attackValue;
    }
    #endregion


}
