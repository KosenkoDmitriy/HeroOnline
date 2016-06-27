using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor {
	
	SerializedProperty typeCharacter;
    SerializedProperty idle, walk, action, action1, action2, cast_1, cast_2, skill_1, skill_2, dead, hit;
	SerializedProperty objMeshAndSkinMesh;
	SerializedProperty colorTakeDamage;
	SerializedProperty icon_Skill_1, icon_Skill_2;
	SerializedProperty textureHpFull, textureHpEmpty;
	SerializedProperty maxHp;
	SerializedProperty def;
	SerializedProperty speedMove;
	SerializedProperty actionValue;
	SerializedProperty skill_1_Value;
	SerializedProperty skill_2_Value;
	SerializedProperty actionSpeed;
	SerializedProperty distanceAction;
	SerializedProperty deley_Cast_Skill_1;
	SerializedProperty deley_Cast_Skill_2;
	SerializedProperty coolDown_Skill_1;
	SerializedProperty coolDown_Skill_2;
	SerializedProperty pointSpell;
    SerializedProperty roleName;
    SerializedProperty roleID;

    SerializedProperty strength;
    SerializedProperty agility;
    SerializedProperty intelligent;
    SerializedProperty offsetEffect;
    SerializedProperty roleType;

	private GameObject[] currentObjSkinMesh;
    SerializedProperty healthbarSize;

	public void OnEnable(){
		typeCharacter = serializedObject.FindProperty("typeCharacter");
		idle = serializedObject.FindProperty("idle");
		walk = serializedObject.FindProperty("walk");
        action = serializedObject.FindProperty("action");
        action1 = serializedObject.FindProperty("action1");
        action2 = serializedObject.FindProperty("action2");
		cast_1 = serializedObject.FindProperty("cast_1");
		cast_2 = serializedObject.FindProperty("cast_2");
		skill_1 = serializedObject.FindProperty("skill_1");
		skill_2 = serializedObject.FindProperty("skill_2");
        dead = serializedObject.FindProperty("dead");
        hit = serializedObject.FindProperty("hit");
		objMeshAndSkinMesh = serializedObject.FindProperty("objMeshAndSkinMesh");
		colorTakeDamage = serializedObject.FindProperty("colorTakeDamage");
		icon_Skill_1 = serializedObject.FindProperty("icon_Skill_1");
		icon_Skill_2 = serializedObject.FindProperty("icon_Skill_2");
		textureHpFull = serializedObject.FindProperty("textureHpFull");
		textureHpEmpty = serializedObject.FindProperty("textureHpEmpty");
		maxHp = serializedObject.FindProperty("maxhp");
		def = serializedObject.FindProperty("def");
		speedMove = serializedObject.FindProperty("speedMove");
		actionValue = serializedObject.FindProperty("actionValue");
		skill_1_Value = serializedObject.FindProperty("skill_1_Value");
		skill_2_Value = serializedObject.FindProperty("skill_2_Value");
		actionSpeed = serializedObject.FindProperty("actionSpeed");
		distanceAction = serializedObject.FindProperty("distanceAction");
		deley_Cast_Skill_1 = serializedObject.FindProperty("deley_Cast_Skill_1");
		deley_Cast_Skill_2 = serializedObject.FindProperty("deley_Cast_Skill_2");
		coolDown_Skill_1 = serializedObject.FindProperty("coolDown_Skill_1");
		coolDown_Skill_2 = serializedObject.FindProperty("coolDown_Skill_2");
        pointSpell = serializedObject.FindProperty("pointSpell");
        roleName = serializedObject.FindProperty("roleName");
        roleID = serializedObject.FindProperty("roleID");

        strength = serializedObject.FindProperty("strength");
        agility = serializedObject.FindProperty("agility");
        intelligent = serializedObject.FindProperty("intelligent");
        offsetEffect = serializedObject.FindProperty("offsetEffect");
        roleType =  serializedObject.FindProperty("roleType");
        healthbarSize = serializedObject.FindProperty("healthbarSize");
	}
	
	public override void OnInspectorGUI(){
		serializedObject.Update();
		Controller myTarget = (Controller)target;
		typeCharacter.enumValueIndex = (int)(Controller.TypeChatacter) EditorGUILayout.EnumPopup("Type Character",(Controller.TypeChatacter)typeCharacter.enumValueIndex);
		if(typeCharacter.enumValueIndex == (int)Controller.TypeChatacter.Range){
			pointSpell.objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Point Spell", pointSpell.objectReferenceValue, typeof(Transform), true);
		}

		idle.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Idle",idle.objectReferenceValue,typeof(AnimationClip),true);
		walk.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Walk",walk.objectReferenceValue,typeof(AnimationClip),true);
        action.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Action", action.objectReferenceValue, typeof(AnimationClip), true);
        action1.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Action 1", action1.objectReferenceValue, typeof(AnimationClip), true);
        action2.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Action 2", action2.objectReferenceValue, typeof(AnimationClip), true);
		cast_1.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Cast_1",cast_1.objectReferenceValue,typeof(AnimationClip),true);
		cast_2.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Cast_2",cast_2.objectReferenceValue,typeof(AnimationClip),true);
		skill_1.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Skill_1",skill_1.objectReferenceValue,typeof(AnimationClip),true);
		skill_2.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Skill_2",skill_2.objectReferenceValue,typeof(AnimationClip),true);
		dead.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Dead",dead.objectReferenceValue,typeof(AnimationClip),true);
        hit.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Hit", hit.objectReferenceValue, typeof(AnimationClip), true);
		icon_Skill_1.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField("Icon Skill 1",icon_Skill_1.objectReferenceValue,typeof(Texture2D),true);
		icon_Skill_2.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField("Icon Skill 2",icon_Skill_2.objectReferenceValue,typeof(Texture2D),true);
		textureHpFull.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField("Texture Hp Full",textureHpFull.objectReferenceValue,typeof(Texture2D),true);
		textureHpEmpty.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField("Texture Hp Empty",textureHpEmpty.objectReferenceValue,typeof(Texture2D),true);
		
		objMeshAndSkinMesh.arraySize = EditorGUILayout.IntField("Size Object Mesh", objMeshAndSkinMesh.arraySize);
		for(int i = 0; i < objMeshAndSkinMesh.arraySize; i++){
			objMeshAndSkinMesh.GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField("Object Mesh And SkinMesh", objMeshAndSkinMesh.GetArrayElementAtIndex(i).objectReferenceValue, typeof(GameObject),true);
		}
		
		colorTakeDamage.colorValue = EditorGUILayout.ColorField("Color take damage", colorTakeDamage.colorValue);
		
		EditorGUILayout.Slider(maxHp,1,1000,"max HP");
		ProgressBar(maxHp.floatValue/1000,"Max Hp");
		
		EditorGUILayout.Slider(def,1,1000,"Defend");
		ProgressBar(def.floatValue/1000,"Defend");
		
		EditorGUILayout.Slider(speedMove,1,20,"Speed Move");
		ProgressBar(speedMove.floatValue/20,"Speed Move");
		
		EditorGUILayout.Slider(actionValue,1,1000,"Action Value");
		ProgressBar(actionValue.floatValue/1000,"Action Value");
		
		EditorGUILayout.Slider(skill_1_Value,1,1000,"Skill 1 Value");
		ProgressBar(skill_1_Value.floatValue/1000,"Skill 1 Value");
		
		EditorGUILayout.Slider(skill_2_Value,1,1000,"Skill 2 Value");
		ProgressBar(skill_2_Value.floatValue/1000,"Skill 2 Value");
		
		EditorGUILayout.Slider(actionSpeed,0,200,"Action Speed");
		ProgressBar(actionSpeed.floatValue/200,"Action Speed");
		
		EditorGUILayout.Slider(distanceAction,1,100,"Distance Action");
		ProgressBar(distanceAction.floatValue/100,"Distance Action");
		
		deley_Cast_Skill_1.floatValue = EditorGUILayout.FloatField("Deley Cast Skill 1 (sec)", deley_Cast_Skill_1.floatValue);
		deley_Cast_Skill_2.floatValue = EditorGUILayout.FloatField("Deley Cast Skill 2 (sec)", deley_Cast_Skill_2.floatValue);
		coolDown_Skill_1.floatValue = EditorGUILayout.FloatField("Cool Down Skill 1 (sec)", coolDown_Skill_1.floatValue);
		coolDown_Skill_2.floatValue = EditorGUILayout.FloatField("Cool Down Skill 2 (sec)", coolDown_Skill_2.floatValue);

        roleName.stringValue = EditorGUILayout.TextField("Role Name", roleName.stringValue);
        roleID.intValue = EditorGUILayout.IntField("Role ID", roleID.intValue);

        strength.intValue = EditorGUILayout.IntField("strength", strength.intValue);
        agility.intValue = EditorGUILayout.IntField("agility", agility.intValue);
        intelligent.intValue = EditorGUILayout.IntField("intelligent", intelligent.intValue);
        offsetEffect.vector3Value = EditorGUILayout.Vector3Field("offsetEffect", offsetEffect.vector3Value);

        roleType.enumValueIndex = (int)(RoleType)EditorGUILayout.EnumPopup("roleType", (RoleType)roleType.enumValueIndex);
        healthbarSize.vector2Value = EditorGUILayout.Vector2Field("healthbarSize", healthbarSize.vector2Value);

		currentObjSkinMesh = myTarget.objMeshAndSkinMesh;
		
		serializedObject.ApplyModifiedProperties();
	}
	
	public void ProgressBar (float val,string label) {
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		EditorGUI.ProgressBar (rect, val, label);
		EditorGUILayout.Space ();
	}
	
}
