using UnityEngine;
using System.Collections;

public class UIRankingManager : MonoBehaviour {

    

	void Start () {
	
	}


    void Update()
    {
        //MyInput.CheckInput();
    }

    #region buttonClick
    public void OnTabArena_Click()
    {
    }
    public void OnTabSurvive_Click()
    {
    }
    public void OnButtonArena_Click()
    {
    }
    public void OnButtonSurvive_Click()
    {
    }
    public void OnButtonBack_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Ranking, GameScenes.previousSence);
    }
    #endregion
}
