/// <summary>
/// This script use to setting GUI factor
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUI_Setting : MonoBehaviour {

	public void SetFactor(List<SizeGUI> sizeGuiList_S){
		for(int i = 0; i < sizeGuiList_S.Count; i++){
			sizeGuiList_S[i].factorX = (float)Screen.width/1920;//Screen.currentResolution.width;
			sizeGuiList_S[i].factorY = (float)Screen.height/1080;//Screen.currentResolution.height;
			sizeGuiList_S[i].sizeXwith_Factor = sizeGuiList_S[i].sizeX * sizeGuiList_S[i].factorX;
			sizeGuiList_S[i].sizeYwith_Factor = sizeGuiList_S[i].sizeY * sizeGuiList_S[i].factorY;
		}
	}
	
	public void SetFactor(List<SizeGUI> sizeGuiList_S, float maxWidth, float maxHeight){
		for(int i = 0; i < sizeGuiList_S.Count; i++){
			sizeGuiList_S[i].factorX = (float)Screen.width/maxWidth;//Screen.currentResolution.width;
			sizeGuiList_S[i].factorY = (float)Screen.height/maxHeight;//Screen.currentResolution.height;
			sizeGuiList_S[i].sizeXwith_Factor = sizeGuiList_S[i].sizeX * sizeGuiList_S[i].factorX;
			sizeGuiList_S[i].sizeYwith_Factor = sizeGuiList_S[i].sizeY * sizeGuiList_S[i].factorY;
		}
	}
					
	public Rect RectGUI(List<SizeGUI> sizeGuiList_S, int index){
		Rect rect = new Rect(Screen.width*sizeGuiList_S[index].offSetX-((sizeGuiList_S[index].sizeX*sizeGuiList_S[index].factorX)/2),
												Screen.height*sizeGuiList_S[index].offSetY-((sizeGuiList_S[index].sizeY*sizeGuiList_S[index].factorY)/2), 
												sizeGuiList_S[index].sizeX*sizeGuiList_S[index].factorX,
												sizeGuiList_S[index].sizeY*sizeGuiList_S[index].factorY);
		return rect;
	}
	
	public Rect RectGUI(List<SizeGUI> sizeGuiList_S, int index, float factorHeight){
		Rect rect = new Rect(Screen.width*sizeGuiList_S[index].offSetX-((sizeGuiList_S[index].sizeX*sizeGuiList_S[index].factorX)/2),
							 Screen.height*sizeGuiList_S[index].offSetY+factorHeight-((sizeGuiList_S[index].sizeY*sizeGuiList_S[index].factorY)/2), 
							 sizeGuiList_S[index].sizeX*sizeGuiList_S[index].factorX,
							 sizeGuiList_S[index].sizeY*sizeGuiList_S[index].factorY);
		return rect;
	}
	
	public Rect RectGUI(List<SizeGUI> sizeGuiList_S, int index, float screenHeight, float screenWight){
		Rect rect = new Rect(screenWight*sizeGuiList_S[index].offSetX-((sizeGuiList_S[index].sizeX*sizeGuiList_S[index].factorX)/2),
							 screenHeight*sizeGuiList_S[index].offSetY-((sizeGuiList_S[index].sizeY*sizeGuiList_S[index].factorY)/2), 
							 sizeGuiList_S[index].sizeX*sizeGuiList_S[index].factorX,
							 sizeGuiList_S[index].sizeY*sizeGuiList_S[index].factorY);
		return rect;
	}
}
