/// <summary>
/// This Script use with ActionEventRange(Script) to spawn bullet
/// </summary>

using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class Spell_Bullet : MonoBehaviour {
	
	public GameObject Fx; //bullet particle
	public AudioClip audioEffect; // sound effect
	
	[HideInInspector]
	public Transform target;
	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float damage;

    public int SkillID { get; set; }
    
    public Controller controllerAttack { get; set; }

 
	void OnTriggerEnter(Collider col){

        if (GetComponent<EffectSettings>() != null)
            return;

        string tagName = Helper.GetTagEnemy(controllerAttack);

        if (col.tag == tagName && col.name == target.name)
        {
            //Debug.Log("OnTriggerEnter " + col.name + " " + controllerAttack.target.name);

            if (controllerAttack.isConnectedServer())
            {

                GameplayManager.Instance.SendSkillHit(controllerAttack.role.Id, controllerAttack.index,
                    col.GetComponent<Controller>(), controllerAttack.curSkill);
            }
            else
            {
                Controller targetController = col.GetComponent<Controller>();
                float damageVal = damage - targetController.def;
                if (damageVal <= 0)
                {
                    damageVal = 0;
                }
                targetController.TakingDamage();
                targetController.hp -= damageVal;
                targetController.damageGet = damageVal;
                targetController.InitTextDamage(Color.red);
                if ((targetController.hp - damageVal) <= 0)
                {
                    targetController.hp = 0;
                    targetController.actionStat = Controller.ActionStat.Dead;
                }
                             
            }

            //Spawn sfx when hit to enemy
            if (audioEffect != null)
            {
               // AudioSource.PlayClipAtPoint(audioEffect, Vector3.zero);
            }

            if (Fx != null && col != null)
            {
                Instantiate(Fx, col.transform.position+Vector3.up, col.transform.rotation);
            }

            Destroy(this.gameObject);

        }
	}
	

	void Start(){
        if (!target) return; 
       
        StartCoroutine(UpdateMove());
	}
	
	IEnumerator UpdateMove(){
		
		//bullet move to target
		while(true){
			if(target != null){
                transform.Translate(Vector3.forward * speed * Time.deltaTime);// Time.smoothDeltaTime);
				transform.LookAt(target.position + Vector3.up);
			}
			yield return 0;
		}
	}
}
