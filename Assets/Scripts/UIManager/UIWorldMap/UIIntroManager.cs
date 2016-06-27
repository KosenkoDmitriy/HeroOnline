using UnityEngine;
using System.Collections;

public class UIIntroManager : MonoBehaviour
{
    public UIWorldMapManager worldMap;
    public GameObject intro1;
    public GameObject intro2;
    public GameObject intro3;
    public GameObject intro4;

    public UILabel label1;
    public UILabel label2;
    public UILabel label3;
    public UILabel label4;

    public string[] s;
    public bool FinishedIntro1;
    public bool FinishedIntro2;
    public bool FinishedIntro3;
    public bool FinishedIntro4;

    void Start()
    {
        GameManager.localization = new MyLocalization();
        Localization();
        FinishedIntro1 = false;
        FinishedIntro2 = false;
        FinishedIntro3 = false;
        FinishedIntro4 = false;
    }

    public void OnIntro1Finished()
    {
        StartCoroutine(ShowLable(label1));
    }
    public void OnIntro2Finished()
    {
        StartCoroutine(ShowLable(label2));
    }
    public void OnIntro3Finished()
    {
        StartCoroutine(ShowLable(label3));
    }
    public void OnIntro4Finished()
    {
        StartCoroutine(ShowLable(label4));
    }
    
    public void Typing1_Finished()
    {
        FinishedIntro1 = true;
    }
    public void Typing2_Finished()
    {
        FinishedIntro2 = true;       
    }
    public void Typing3_Finished()
    {
        FinishedIntro3 = true;      
    }
    public void Typing4_Finished()
    {
        FinishedIntro4 = true;
    }

    public void OnIntro1_Click()
    {
       // if (FinishedIntro1)
            StartCoroutine(Fade(intro1, label1, intro2));
       // else
        //{
            ShowAllText(label1, s[0]);
            FinishedIntro1 = true;
       // }

    }
    public void OnIntro2_Click()
    {
        //if (FinishedIntro2)
            StartCoroutine(Fade(intro2, label2, intro3));
       // else
       // {
           // ShowAllText(label2, s[1]);
            FinishedIntro2 = true;
       // }
    }
    public void OnIntro3_Click()
    {
      //  if (FinishedIntro3)
            StartCoroutine(Fade(intro3, label3, intro4));
      //  else
       // {
          //  ShowAllText(label3, s[2]);
            FinishedIntro3 = true;
        //}
    }
    public void OnIntro4_Click()
    {
       
            FinishedIntro4 = true;

            gameObject.SetActive(false);

            //init worldmap
            worldMap.Init();
            GameManager.viewedIntro = true;
      
    }

    private void ShowAllText(UILabel lbl, string s)
    {
        lbl.GetComponent<TypewriterEffect>().enabled = false;
        lbl.text = s;
    }

    private IEnumerator Fade(GameObject goFrom, UILabel lblForm, GameObject goTo)
    {
        goTo.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        goFrom.SetActive(false);
        lblForm.gameObject.SetActive(false);
    }

    private IEnumerator ShowLable(UILabel lbl)
    {
        lbl.alpha = 0;
        lbl.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        lbl.alpha = 1;
    }

    private void Localization()
    {
        s = new string[4];

        s[0] = GameManager.localization.GetText("Intro_1");
        s[1] = GameManager.localization.GetText("Intro_2");
        s[2] = GameManager.localization.GetText("Intro_3");
        s[3] = GameManager.localization.GetText("Intro_4");

        label1.text = s[0];
        label2.text = s[1];
        label3.text = s[2];
        label4.text = s[3];
    }
}
