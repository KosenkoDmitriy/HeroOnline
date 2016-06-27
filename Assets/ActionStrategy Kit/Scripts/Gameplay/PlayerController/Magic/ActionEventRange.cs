/// <summary>
/// This script use for control state of character(Range)
/// to attack,skill,call sound effect,call particle by using with Setthing Action(Script)
/// </summary>

using UnityEngine;
using System.Collections;

public class ActionEventRange : MonoBehaviour
{

    public Controller controller;

    public void CastSkillFX(GameObject FX)
    {
        //Spawn particle fx when calling this method
        Instantiate(FX, this.transform.position, this.transform.rotation);
    }

    public void Attacking()
    {
        SkillManager.Instance.DoSkill(controller, 0);
    }

    public void Skill_1()
    {
        SkillManager.Instance.DoSkill(controller, 1);
    }

    public void Skill_2()
    {
        SkillManager.Instance.DoSkill(controller, 2);
    }
      

    public void CastSkill_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
        //AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    public void Spelling_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
       // AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    private int AttackValue(float val)
    {
        //random value of healing power when calling this method
        float attackValue = Random.Range(val * 0.8f, val * 1.2f);
        return (int)attackValue;
    }

}
