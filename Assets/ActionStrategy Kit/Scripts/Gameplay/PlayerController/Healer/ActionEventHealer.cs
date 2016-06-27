/// <summary>
/// This script use for control state of character(healer)
/// to heal,skill,call sound effect,call particle by using with Setthing Action(Script)
/// </summary>

using UnityEngine;
using System.Collections;

public class ActionEventHealer : MonoBehaviour
{

    public Controller controller;
    public GetAllPlayer getAllPlayer;

    public void CastSkillFX(GameObject FX)
    {
        //Spawn particle fx when calling this method
        Instantiate(FX, this.transform.position, this.transform.rotation);
    }

    //call in Update() of SettingAction.cs
    public void Buff()
    {
        SkillManager.Instance.DoSkill(controller, 0);
    }

    //call in Update() of SettingAction.cs
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
        AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    public void Healing_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
        AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    public void HealingGroup_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
        AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    public void BuffAttack_Sound(AudioClip audio)
    {
        //calling sfx when calling this method
        AudioSource.PlayClipAtPoint(audio, Vector3.zero);
    }

    private int HealingValue(float val)
    {
        //random value of healing power when calling this method
        float healingValue = Random.Range(val * 0.8f, val * 1.2f);
        return (int)healingValue;
    }
  
}

