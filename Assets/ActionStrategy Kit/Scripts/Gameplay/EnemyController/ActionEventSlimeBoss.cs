/// <summary>
/// This script use for control state of monster(SlimeBoss)
/// to attack,dead,call sound effect,call particle by using with Setthing Action(Script)
/// </summary>

using UnityEngine;
using System.Collections;

public class ActionEventSlimeBoss : MonoBehaviour {

	public Enemy enemy;
	
	public void Attacking(GameObject FX){
		
		//Decrease Hp
		if(enemy.target != null){
			Controller playerTarget = enemy.target.GetComponent<Controller>();
			Instantiate(FX, playerTarget.transform.position, playerTarget.transform.rotation);
			float damage = enemy.attackDamage - playerTarget.def;
			if(damage <= 0){ // if damage less than 0 it equal 0
				damage = 0;	
			}
			
			if((playerTarget.hp - damage) <= 0){ //Decrease Hp hero
				playerTarget.actionStat = Controller.ActionStat.Dead;
				enemy.target = null;
				playerTarget.TakingDamage();
				playerTarget.damageGet = AttackValue(damage);
				playerTarget.hp -= playerTarget.damageGet;
			}
			
		//Spawn floating text damage	
			playerTarget.TakingDamage();
			playerTarget.damageGet = AttackValue(damage);
			playerTarget.hp -= playerTarget.damageGet;
			playerTarget.InitTextDamage(Color.red);
			
			if(playerTarget.hp <= 0){
				playerTarget.hp = 0;
			}
		}
	}
	
	public void Dead(GameObject FX){
		//Spawn dead effect
		Instantiate(FX, this.transform.position, this.transform.rotation);
	}
	
	public void Attacking_Sound(AudioClip audio){
		//Spawn attack sound
		AudioSource.PlayClipAtPoint(audio, Vector3.zero);	
	}
	
	private int AttackValue(float val){
		//Random Attack damage value
		float attackValue = Random.Range(val*0.8f, val*1.2f);
		return (int)attackValue;
	}
}
