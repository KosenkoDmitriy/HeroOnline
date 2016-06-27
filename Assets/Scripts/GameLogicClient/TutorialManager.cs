using UnityEngine;
using System.Collections;
using DEngine.Common;
using ExitGames.Client.Photon;
using DEngine.Common.GameLogic;
using DEngine.Unity.Photon;

public class TutorialManager 
{

    public enum TutorialStep
    {
        Begin,

        SummonHeroes_NPC,
        SummonHeroes_BuyHero,

        BuyHero_Exit,

        Party_NPC,
        Party_NPCFinished,

        Control_NPCFinshed,

        Equip_Begin,
        Equip_Arrow,

        Equip_Finished,

        Finished
    }

    public TutorialStep step;
    
    public void Init()
    {
        //load current tutorial saved from database, and continues 
        step = (TutorialStep)GameManager.GameUser.Base.TutorStep;      // TutorialStep.Begin; // 
    }
    

    private void Save()
    {
        int TutorStep = (int)step;
        GameManager.GameUser.Base.TutorStep = TutorStep;
        GameObject go = GameObject.Find("PhotonManager");
        if (go != null)
        {
            go.GetComponent<PhotonManager>().Controller.SendSaveTutorialStep();           
        }
     
    }

    public void CreateNPC(UINPCTutorialManager.Status status, string s)
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/UI/UITutorial/UINPCTutorialRoot") as GameObject) as GameObject;
        UINPCTutorialManager npcTutorial = go.GetComponent<UINPCTutorialManager>();
        npcTutorial.Show(status, s);
    }

    public void ChangeStep(TutorialStep nextStep)
    {
        step = nextStep;
        Save();
    }
}
