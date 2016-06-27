using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor {
	
	SerializedProperty idle, walk, attack, dead;
	SerializedProperty objMeshAndSkinMesh;
	SerializedProperty colorTakeDamage;
	SerializedProperty textureHpFull, textureHpEmpty;
	SerializedProperty maxHp;
	SerializedProperty def;
	SerializedProperty speedMove;
	SerializedProperty attackDamage;
	SerializedProperty attackSpeed;
	SerializedProperty distanceAttack;

    private int size = 0;
	private int currentSize;
	private GameObject[] currentObjSkinMesh;
	
	public void OnEnable(){
		idle = serializedObject.FindProperty("idle");
		walk = serializedObject.FindProperty("walk");
		attack = serializedObject.FindProperty("attack");
		dead = serializedObject.FindProperty("dead");
		objMeshAndSkinMesh = serializedObject.FindProperty("objMeshAndSkinMesh");
		colorTakeDamage = serializedObject.FindProperty("colorTakeDamage");
		textureHpFull = serializedObject.FindProperty("textureHpFull");
		textureHpEmpty = serializedObject.FindProperty("textureHpEmpty");
		maxHp = serializedObject.FindProperty("maxhp");
		def = serializedObject.FindProperty("def");
		speedMove = serializedObject.FindProperty("speedMove");
		attackDamage = serializedObject.FindProperty("attackDamage");
		attackSpeed = serializedObject.FindProperty("attackSpeed");
		distanceAttack = serializedObject.FindProperty("distanceAttack");
	}
	
	public override void OnInspectorGUI(){
		serializedObject.Update();
		Enemy myTarget = (Enemy)target;

		idle.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Idle",idle.objectReferenceValue,typeof(AnimationClip),true);
		walk.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Walk",walk.objectReferenceValue,typeof(AnimationClip),true);
		attack.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Attack",attack.objectReferenceValue,typeof(AnimationClip),true);
		dead.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Dead",dead.objectReferenceValue,typeof(AnimationClip),true);
		textureHpFull.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField("Texture Hp Full",textureHpFull.objectReferenceValue,typeof(Texture2D),true);
		textureHpEmpty.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField("Texture Hp Empty",textureHpEmpty.objectReferenceValue,typeof(Texture2D),true);
		
		objMeshAndSkinMesh.arraySize = EditorGUILayout.IntField("Size Object Mesh", objMeshAndSkinMesh.arraySize);
		for(int i = 0; i < objMeshAndSkinMesh.arraySize; i++){
			objMeshAndSkinMesh.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField("Object Mesh And SkinMesh", objMeshAndSkinMesh.GetArrayElementAtIndex(i).objectReferenceValue, typeof(GameObject),true);
		}
		
		colorTakeDamage.colorValue = EditorGUILayout.ColorField("Color take damage", colorTakeDamage.colorValue);
		
		EditorGUILayout.Slider(maxHp,1,1000,"Max HP");
		ProgressBar(maxHp.floatValue/1000,"Max Hp");
		
		EditorGUILayout.Slider(def,1,1000,"Defend");
		ProgressBar(def.floatValue/200,"Defend");
		
		EditorGUILayout.Slider(speedMove,1,20,"Speed Move");
		ProgressBar(speedMove.floatValue/20,"Speed Move");
		
		EditorGUILayout.Slider(attackDamage,1,200,"Attack Damage");
		ProgressBar(attackDamage.floatValue/200,"Attack Damage");
		
		EditorGUILayout.Slider(attackSpeed,1,200,"Attack Speed");
		ProgressBar(attackSpeed.floatValue/200,"Attack Speed");
		
		EditorGUILayout.Slider(distanceAttack,1,100,"Distance Attack");
		ProgressBar(distanceAttack.floatValue/100,"Distance Attack");
		
		currentObjSkinMesh = myTarget.objMeshAndSkinMesh;
		currentSize = size;
		serializedObject.ApplyModifiedProperties();
	}
	
	public void ProgressBar (float val,string label) {
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		EditorGUI.ProgressBar (rect, val, label);
		EditorGUILayout.Space ();
	}
	
}
