/// <summary>
/// This script use for healer skill(heal all)
/// </summary>
using UnityEngine;
using System.Collections;

public class GetAllPlayer : MonoBehaviour {
	
	public string tags;
	public GameObject[] players;
	
	void Start() {
		//Find all player to gameobject
		players = GameObject.FindGameObjectsWithTag(tags);	
	}
}
