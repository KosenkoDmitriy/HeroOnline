/// <summary>
/// This script use for search & detect player
/// use with isTrigger Collider
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaChecker : MonoBehaviour {
	
	public Enemy enemy;
	
	private Vector3 defaultScale;
	private bool down;
	
	void OnTriggerEnter(Collider col){
		
		//Search and detect target
		if(col.tag == GameManager.PlayerTagName){
			if(enemy.target == null){
				if(col.GetComponent<Controller>().hp > 0){
					if(enemy.actionStat != Enemy.ActionStat.Dead){
						enemy.target = col.GetComponent<Collider>().gameObject;	
						enemy.actionStat = Enemy.ActionStat.Attack;
					}
				}
			}
		}
	}
	
	void Start(){
		defaultScale = this.transform.localScale;
	}
	
	void Update(){
		
		//collider will change scale if can't detect hero 
		if(enemy.target == null){
			if(this.transform.localScale.x > defaultScale.x-0.1f){
				down = true;
			}else if(this.transform.localScale.x <= 0.1f){
				down = false;
			}
			
			if(down){
				this.transform.localScale = Vector3.Lerp(this.transform.localScale,Vector3.zero,5*Time.deltaTime);
			}else{
				this.transform.localScale = Vector3.Lerp(this.transform.localScale,defaultScale,5*Time.deltaTime);
			}
		}
	}
	
}
