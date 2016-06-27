/// <summary>
/// This script use for control condition game over and condition win
/// </summary>

using UnityEngine;
using System.Collections;

public class ASKGameManager : MonoBehaviour {

    public static ASKGameManager Instance;
	public GameObject uiStageClear; //GUI Stage Clear
	public GameObject uiGameOver; //GUI Gameover
	public bool isEnd; //ending game variable if isEnd = true it mean StageClear
	public bool isOver; //gameover varible if isOver = true it mean Game Over
	public Controller knight,mage,priest;
	
	// Use this for initialization
	void Start () {
		
		Instance = this;
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//Condition Gameover
		if(knight.hp <= 0 && mage.hp <= 0 && priest.hp <=0 && !isOver)
		{
			isOver = true;
		}
		
		if(isEnd && uiStageClear.activeSelf == false) //if win go to publisher website
		{
			uiStageClear.SetActive(true);
			StartCoroutine(NextPage());
		}
		
		if(isOver && uiGameOver.activeSelf == false) //if lose go to title scene
		{
			uiGameOver.SetActive(true);
			StartCoroutine(ToTitle());
		}
	
	}
	
	IEnumerator NextPage(){
		//Open publisher website
		yield return new WaitForSeconds(3);
	}
	
	IEnumerator ToTitle(){
		//Load title scene
		yield return new WaitForSeconds(3);
	}
}
