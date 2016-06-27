/// <summary>
/// This script use for control enemy wave ,
/// use to manage monster list , monster type
/// </summary>
 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {
	
	[System.Serializable]
	public class PatternSet{
		public List<GameObject> enemyPref = new List<GameObject>();	
		public List<Transform> pointSpawn = new List<Transform>();
	}
	
	public int wave; //wave of monster in this stage
	
	public List<PatternSet> patternSet = new List<PatternSet>();
	
	public float deleyNextSpawn; //delay wait for nextspawn if end before wave
	
	private List<GameObject> enemyCheck = new List<GameObject>(); //Check null enemy
	
	[HideInInspector]
	public bool endGame;
	
	void Start(){
		endGame = false;
		StartCoroutine(SpawningEnemy());	
	}
	
	IEnumerator WaitEnemyNull(){
		
		//Wait all enemy dead 
		int i = 0;
		bool complete = false;
		bool check = true;
		while(complete == false){
			i = 0;
			check = true;
			while(i < enemyCheck.Count){
				if(check){
					if(enemyCheck[i] != null){
						check = false;	
					}
				}
				i++;
			}
			if(check){
				complete = true;	
			}
			yield return 0;
		}	
		
		yield return new WaitForSeconds(deleyNextSpawn);
		enemyCheck.Clear();
		wave++;
		
		//if final wave is clear
		if(wave > patternSet.Count-1){
			wave = patternSet.Count-1; 	
			
			
			//variable endgame is true if final wave clear
            ASKGameManager.Instance.isEnd = true;
		}else //isn't final wave , spawn new wave
		{
			//Spawn new enemy wave
			StartCoroutine(SpawningEnemy());
		}
		
	}
	
	IEnumerator SpawningEnemy(){
		
		//Spawn enemy
		int i = 0;
		while(i < patternSet[wave].enemyPref.Count){
			GameObject go = (GameObject)Instantiate(patternSet[wave].enemyPref[i], patternSet[wave].pointSpawn[i].position, patternSet[wave].pointSpawn[i].rotation);
			enemyCheck.Add(go);
			i++;
		}
		yield return 0;
		StartCoroutine(WaitEnemyNull());
	}
	

	
}
