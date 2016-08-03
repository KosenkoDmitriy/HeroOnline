/// <summary>
/// This script use for control state of enemy.
/// </summary>
/// 
using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class Enemy : MonoBehaviour {
	
	public enum ActionStat{ //enemy state
		Idle, Move, Attack, Dead
	}
	
	public AnimationClip idle, walk, attack, dead; //enemy animation
	public GameObject[] objMeshAndSkinMesh; //Gameobject Mesh use for change color material when taking attack
	public Color colorTakeDamage; //color when character take damage
	public Texture2D textureHpFull, textureHpEmpty; //hp bar
	
	public float maxhp; //Max hp
	public float def; //Defend
	public float attackDamage; //Attack Damage
	public float attackSpeed; //Attack Speed
	public float distanceAttack; //Distance Attack
	public float speedMove; //Speed Move
	
	//Variable private field 
    private Vector3 positionWay = Vector3.zero;
	private Vector3 pointHp;
    private Rect rectHp;
	private float checkDistance;
	private float countAttack;
	
	//Delegate update function
	public delegate void FunctionHandle();
	public FunctionHandle AttackHandle;
	
	[HideInInspector]
	public float hp;
	
	[HideInInspector]
	public float damageGet;
	
	public GameObject target;
	[HideInInspector]
	public ActionStat actionStat;
	
	void Start(){
		hp = maxhp;
		actionStat = ActionStat.Move;
		AttackHandle = Attack_1;	
	}
	
	void OnGUI(){
		
		//GUI monster hp
		Vector3 pointTransform = Vector3.zero;
		pointTransform.x = transform.position.x;
		pointTransform.y = transform.position.y+1.5f;
		pointTransform.z = transform.position.z;
		pointHp = TouchController.instance.cameraTarget.WorldToScreenPoint(pointTransform);
		rectHp.width = 100;
		rectHp.height = 10;
		rectHp.x = pointHp.x-(rectHp.width/2);
		rectHp.y = Screen.height-pointHp.y-(rectHp.height/2);
		GUI.DrawTexture(rectHp,textureHpEmpty);
		GUI.BeginGroup(rectHp,"");
		GUI.DrawTexture(new Rect(0,0,100*(hp/maxhp),rectHp.height),textureHpFull);
		GUI.EndGroup();
	}
	
	void Update () {
		UpdateState();
	}
	
	void UpdateState(){
		
		//Monster state animation
		switch(actionStat){
			case ActionStat.Idle :{
				if(actionStat != ActionStat.Dead){
					GetComponent<Animation>().CrossFade(idle.name);
					if(target){
						actionStat = ActionStat.Attack;
					}
				}
			}
			break;
			
			case ActionStat.Move :{
				if(actionStat != ActionStat.Dead){
					checkDistance = (transform.position - LookAtTo(positionWay)).magnitude;
					if(checkDistance >= distanceAttack){
						GetComponent<Animation>().CrossFade(walk.name);
						transform.Translate(Vector3.forward*speedMove*Time.deltaTime);
					}else{
						actionStat = ActionStat.Idle;
					}
				}
			}
			break;
			
			case ActionStat.Attack :{
				if(actionStat != ActionStat.Dead){
					if(target != null){
						checkDistance = (transform.position - LookAtTo(target.transform.position)).magnitude;
						if(checkDistance >= distanceAttack){
							GetComponent<Animation>().CrossFade(walk.name);
							transform.Translate(Vector3.forward*speedMove*Time.deltaTime);
						}else{
							AttackHandle();
						}
					}else{
						actionStat = ActionStat.Idle;
					}
				}
			}
			break;
			
			case ActionStat.Dead :{
				GetComponent<Animation>().CrossFade(dead.name);
				Destroy(gameObject, 2);
			}
			break;
		}
	}
	
	private Vector3 LookAtTo(Vector3 pos){
		//Lookat Target
		Vector3 look = Vector3.zero;
		look.x = pos.x;
		look.y = transform.position.y;
		look.z = pos.z;
		transform.LookAt(look);
		return look;
	}
	
	private void Attack_1(){
		//wait delay & prepare attack
		GetComponent<Animation>().CrossFade(idle.name);
        countAttack += attackSpeed * Time.deltaTime;// Time.smoothDeltaTime;
		if(countAttack >= 100){
			countAttack = 0;
			AttackHandle = Attack_2;	
		}
	}
	
	private void Attack_2(){
		//attack
		GetComponent<Animation>().Play(attack.name);
		if(GetComponent<Animation>()[attack.name].normalizedTime > 0.9f){
			AttackHandle = Attack_1;	
		}
	}
	
	public void TakingDamage(){
		//if take damage material monster will change to white mat
		int index = 0;
		while(index < objMeshAndSkinMesh.Length){
			objMeshAndSkinMesh[index].GetComponent<Renderer>().material.color = Color.white;
			index++;
		}
		StartCoroutine(TakeDamage(0.1f));
	}
	
	public void InitTextDamage(Color colorText){
		//Init text damage
		/*GameObject loadPref = (GameObject)Resources.Load("TextDamage");
		GameObject go = (GameObject)Instantiate(loadPref, transform.position, Quaternion.identity);
		go.GetComponentInChildren<TextDamage>().SetDamage(damageGet, colorText);*/
        
        TextDamageController.Instance.create(transform.position, damageGet, Color.red);
	}
	
	private IEnumerator TakeDamage(float time){
		
		//if take damage material monster will change to setting color
		
		int index = 0;
		Color[] colorDef = new Color[objMeshAndSkinMesh.Length];
		while(index < objMeshAndSkinMesh.Length){
			colorDef[index] = objMeshAndSkinMesh[index].GetComponent<Renderer>().material.color;
			objMeshAndSkinMesh[index].GetComponent<Renderer>().material.color = colorTakeDamage;
			index++;
		}
		yield return new WaitForSeconds(time);
		index = 0;
		while(index < objMeshAndSkinMesh.Length){
			objMeshAndSkinMesh[index].GetComponent<Renderer>().material.color = colorDef[index];
			index++;
		}
		yield return 0;
		StopAllCoroutines();
	}

    public void OnSelected()
    {
        
        foreach (GameObject go in objMeshAndSkinMesh)
        {
            foreach (Material mat in go.GetComponent<Renderer>().materials)
            {
                mat.shader = Resources.Load<Shader>("Shaders/Toony-BasicOutline");
                                
                mat.SetColor("_OutlineColor", new Color(0.8f, 0f, 0f, 0.7f));

                mat.SetFloat("_OuntLine", 0.0f);
            }
        }
    }

    public void OnDeSelected()
    {
        foreach (GameObject go in objMeshAndSkinMesh)
        {
            foreach (Material mat in go.GetComponent<Renderer>().materials)
            {
                mat.shader = Resources.Load<Shader>("Shaders/Toony-Basic");
            }
        }
    }
}
