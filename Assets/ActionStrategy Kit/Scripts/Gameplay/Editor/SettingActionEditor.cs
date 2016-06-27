using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(SettingAction))]
public class SettingActionEditor :  Editor {

	SerializedProperty activeIndex;
	SerializedProperty methodName;
	SerializedProperty time;
	SerializedProperty animations;
	SerializedProperty preview;
	
	SerializedProperty sendParameterType;
	/*SerializedProperty sendInt;
	SerializedProperty sendFloat;
	SerializedProperty sendBool;
	SerializedProperty sendObject;*/
	
	private int[] sizeInt;
	private string[] stringSize;
	
	private int size;
	private int currentSize;
	private GameObject[] currentObjSkinMesh;
	private float maxSlider;
	private int currenctActiveIndex;
	private SettingAction mySettingAction;
	
	public void OnEnable(){
		mySettingAction = (SettingAction)target;
		activeIndex = serializedObject.FindProperty("activeIndex");	
		methodName = serializedObject.FindProperty("methodName");	
		time = serializedObject.FindProperty("time");	
		animations = serializedObject.FindProperty("animations");	
		//sendInt = serializedObject.FindProperty("sendInt");	
		//sendFloat = serializedObject.FindProperty("sendFloat");	
		//sendBool = serializedObject.FindProperty("sendBool");	
		//sendObject = serializedObject.FindProperty("sendObject");	
		sendParameterType = serializedObject.FindProperty("sendParameterType");
		preview = serializedObject.FindProperty("preview");
		
		if(mySettingAction.eventSet.Count < 1){
			mySettingAction.eventSet.Add(new SettingAction.EventSet());
			activeIndex.intValue = 0;
		}
		
		sizeInt = new int[mySettingAction.eventSet.Count];
		stringSize = new string[mySettingAction.eventSet.Count];
		for(int i = 0; i < sizeInt.Length; i++){
			sizeInt[i] = i;
			stringSize[i] = (i).ToString();
		}
	}
	
	public override void OnInspectorGUI(){
		serializedObject.Update();
		try{
			mySettingAction = (SettingAction)target;
			if(GUILayout.Button("Create Action")){
				mySettingAction.eventSet.Add(new SettingAction.EventSet());
				SetSizeIntPopUp();
			}	
			
			preview.boolValue = GUILayout.Toggle(preview.boolValue,"Preview Animation");
			
			activeIndex.intValue = EditorGUILayout.IntPopup("Active Index", activeIndex.intValue,stringSize,sizeInt);
			if(currenctActiveIndex != activeIndex.intValue){
				ResetDisplayValue();
				GetDisplayValue();
			}else{
				SetDisplayValue();
			}
			
			if(GUILayout.Button("Apply")){
				Debug.Log("Apply Setting");	
				ResetDisplayValue();
				GetDisplayValue();
			}
			
			if(GUILayout.Button("Delete Action")){
				if(mySettingAction.eventSet.Count > 1){
					mySettingAction.eventSet.RemoveRange(activeIndex.intValue,1);
					SetSizeIntPopUp();
					activeIndex.intValue -= 1;
					if(activeIndex.intValue <= 0){
						activeIndex.intValue = 0;	
					}
				}
				serializedObject.Update();
				serializedObject.ApplyModifiedProperties();
			}
			
			currenctActiveIndex = activeIndex.intValue;
		}catch{
			Debug.Log("Catch");
		}
		serializedObject.ApplyModifiedProperties();
		
	}
	
	private void ResetDisplayValue(){
		methodName.stringValue = "";
		sendParameterType.enumValueIndex = 0;
		animations.objectReferenceValue = null;
		maxSlider = 1;	
		time.floatValue = 0;
	}
	
	private void GetDisplayValue(){
		methodName.stringValue = mySettingAction.eventSet[activeIndex.intValue].methodName;
		sendParameterType.enumValueIndex = (int)(SettingAction.SendParameterType)mySettingAction.eventSet[activeIndex.intValue].sendParameterType;
		animations.objectReferenceValue = mySettingAction.eventSet[activeIndex.intValue].animations;
		if(mySettingAction.eventSet[activeIndex.intValue].animations != null){
			maxSlider = mySettingAction.eventSet[activeIndex.intValue].animations.length;
		}
		time.floatValue = mySettingAction.eventSet[activeIndex.intValue].time;
	}
	
	private void SetDisplayValue(){
		methodName.stringValue = EditorGUILayout.TextField("Method Name", methodName.stringValue);
		
		sendParameterType.enumValueIndex = (int)(SettingAction.SendParameterType) EditorGUILayout.EnumPopup("Send Parameter Type",(SettingAction.SendParameterType)sendParameterType.enumValueIndex);
		SendType();
		animations.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Animation Action",animations.objectReferenceValue,typeof(AnimationClip),true);
		mySettingAction.eventSet[activeIndex.intValue].methodName = methodName.stringValue;
		mySettingAction.eventSet[activeIndex.intValue].sendParameterType = (SettingAction.SendParameterType)sendParameterType.enumValueIndex;
		mySettingAction.eventSet[activeIndex.intValue].time = time.floatValue;
		mySettingAction.eventSet[activeIndex.intValue].animations = (AnimationClip)animations.objectReferenceValue;
		if(mySettingAction.eventSet[activeIndex.intValue].animations != null){
			maxSlider = mySettingAction.eventSet[activeIndex.intValue].animations.length;
		}else{
			maxSlider = 1;	
		}
		EditorGUILayout.Slider(time,0,maxSlider,"Time");
		
	}
	
	private void SendType(){
		switch((SettingAction.SendParameterType)sendParameterType.enumValueIndex){
			case SettingAction.SendParameterType.Int :{
				//sendInt.intValue = EditorGUILayout.IntField("Send Int", sendInt.intValue);
				mySettingAction.eventSet[activeIndex.intValue].sendInt = EditorGUILayout.IntField("Send Int", mySettingAction.eventSet[activeIndex.intValue].sendInt);
			}
			break;
			
			case SettingAction.SendParameterType.Float :{
				//sendFloat.floatValue = EditorGUILayout.FloatField("Send Float", sendFloat.floatValue);
				mySettingAction.eventSet[activeIndex.intValue].sendFloat = EditorGUILayout.FloatField("Send Int", mySettingAction.eventSet[activeIndex.intValue].sendFloat);
			}
			break;
			
			case SettingAction.SendParameterType.Object :{
				//sendObject.objectReferenceValue = EditorGUILayout.ObjectField("Send Object",sendObject.objectReferenceValue,typeof(Object),true);
				mySettingAction.eventSet[activeIndex.intValue].sendObject = EditorGUILayout.ObjectField("Send Object",mySettingAction.eventSet[activeIndex.intValue].sendObject,typeof(Object),true);
			}
			break;
		}
	}
	
	private void SetSizeIntPopUp(){
		sizeInt = new int[mySettingAction.eventSet.Count];
		stringSize = new string[mySettingAction.eventSet.Count];
		for(int i = 0; i < sizeInt.Length; i++){
			sizeInt[i] = i;
			stringSize[i] = (i).ToString();
		}
	}
}
