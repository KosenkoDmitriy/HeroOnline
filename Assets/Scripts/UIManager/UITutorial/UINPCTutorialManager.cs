using UnityEngine;
using System.Collections;

public class UINPCTutorialManager : MonoBehaviour {

    public delegate void FinishedHandler();
    public static event FinishedHandler OnFinished;

    public enum Status
    {
        Normal,
        Angry,
        Sad,
        Smile,
        Shame1,
        Shame2
    }
        
    public Texture2D[] iconStatus; 

    public Status status;
    public UITexture icon;
    public UILabel lblText;
    public UIButton btnNext;

    private string _text;
    
    public void Show(Status stat, string text)
    {
        icon.mainTexture = iconStatus[(int)stat];
        _text = text;
        lblText.gameObject.SetActive(false);
    }
        

    public void OnFinishedMC()
    {        
        lblText.text = _text;
        Invoke("Writing", 0.2f);
    }

    public void OnFinishedWriting()
    {
        btnNext.gameObject.SetActive(true);
    }

    private void Writing()
    {
        lblText.gameObject.SetActive(true);
    }

    public void OnTab()
    {
        if (lblText.text.Length >= _text.Length)
        {
            Destroy(gameObject);
            if (OnFinished != null)
            {
                OnFinished.Invoke();
                OnFinished -= OnFinished;
            }
        }
    }

    
}
