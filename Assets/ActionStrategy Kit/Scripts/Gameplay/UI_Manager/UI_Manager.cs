/// <summary>
/// This script use to manage GUI
/// GUI skill 1,2 on top-right screen
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_Manager : MonoBehaviour {
	
	public List<SizeGUI> sizeGUIList = new List<SizeGUI>();
	public GUITexture guiSkill_1, guiSkill_2;
	public GUITexture guiCoolDownSkill_1, guiCoolDownSkill_2;
	
	private GUI_Setting guiSetting = new GUI_Setting();
	private Rect rectCoolDownSkill_1, rectCoolDownSkill_2;
	
	void Update(){
		if(TouchController.instance.controllerGetSkill != null){
			guiSetting.SetFactor(sizeGUIList);
			
			if(TouchController.instance.controllerGetSkill.icon_Skill_1 != null){
				guiCoolDownSkill_1.enabled = true;
				guiSkill_1.enabled = true;
				guiCoolDownSkill_2.enabled = true;
				guiSkill_2.enabled = true;
				guiSkill_1.texture = TouchController.instance.controllerGetSkill.icon_Skill_1;
				guiSkill_1.pixelInset = guiSetting.RectGUI(sizeGUIList,0);
				
				if(guiSkill_1.HitTest(Input.mousePosition)){
					if(Input.GetMouseButtonDown(0)){
						if(TouchController.instance.controllerGetSkill.value_CoolDown_Skill_1 <= 0
							&& TouchController.instance.controllerGetSkill.actionStat != Controller.ActionStat.Skill
							&& TouchController.instance.controllerGetSkill.actionStat != Controller.ActionStat.Dead){
							TouchController.instance.controllerGetSkill.StartCoroutine("CalCoolDownSkill_1");
							TouchController.instance.controllerGetSkill.SkillHandle = TouchController.instance.controllerGetSkill.Skill_1_Cast;
							TouchController.instance.controllerGetSkill.actionStat = Controller.ActionStat.Skill;
						}
					}
				}
			}else{
				guiCoolDownSkill_1.enabled = false;
				guiSkill_1.enabled = false;
				guiCoolDownSkill_2.enabled = false;
				guiSkill_2.enabled = false;
			}
			
			if(TouchController.instance.controllerGetSkill.icon_Skill_2 != null){
				guiCoolDownSkill_1.enabled = true;
				guiSkill_1.enabled = true;
				guiCoolDownSkill_2.enabled = true;
				guiSkill_2.enabled = true;
				guiSkill_2.texture = TouchController.instance.controllerGetSkill.icon_Skill_2;
				guiSkill_2.pixelInset = guiSetting.RectGUI(sizeGUIList,1);
				
				if(guiSkill_2.HitTest(Input.mousePosition)){
					if(Input.GetMouseButtonDown(0)){
						if(TouchController.instance.controllerGetSkill.value_CoolDown_Skill_2 <= 0
							&& TouchController.instance.controllerGetSkill.actionStat != Controller.ActionStat.Skill
							&& TouchController.instance.controllerGetSkill.actionStat != Controller.ActionStat.Dead){
							TouchController.instance.controllerGetSkill.StartCoroutine("CalCoolDownSkill_2");
							TouchController.instance.controllerGetSkill.SkillHandle = TouchController.instance.controllerGetSkill.Skill_2_Cast;
							TouchController.instance.controllerGetSkill.actionStat = Controller.ActionStat.Skill;
						}
					}
				}
			}else{
				guiCoolDownSkill_1.enabled = false;
				guiSkill_1.enabled = false;	
				guiCoolDownSkill_2.enabled = false;
				guiSkill_2.enabled = false;
			}
			
			rectCoolDownSkill_1.x = guiSetting.RectGUI(sizeGUIList,2).x;
			rectCoolDownSkill_1.y = guiSetting.RectGUI(sizeGUIList,2).y;
			rectCoolDownSkill_1.width = guiSetting.RectGUI(sizeGUIList,2).width;
			rectCoolDownSkill_1.height = guiSetting.RectGUI(sizeGUIList,2).height * TouchController.instance.controllerGetSkill.value_CoolDown_Skill_1;
			guiCoolDownSkill_1.pixelInset = rectCoolDownSkill_1;
			
			rectCoolDownSkill_2.x = guiSetting.RectGUI(sizeGUIList,3).x;
			rectCoolDownSkill_2.y = guiSetting.RectGUI(sizeGUIList,3).y;
			rectCoolDownSkill_2.width = guiSetting.RectGUI(sizeGUIList,3).width;
			rectCoolDownSkill_2.height = guiSetting.RectGUI(sizeGUIList,3).height * TouchController.instance.controllerGetSkill.value_CoolDown_Skill_2;
			guiCoolDownSkill_2.pixelInset = rectCoolDownSkill_2;
		}else{
			guiCoolDownSkill_1.enabled = false;
			guiSkill_1.enabled = false;	
			guiCoolDownSkill_2.enabled = false;
			guiSkill_2.enabled = false;
		}
	}
}
