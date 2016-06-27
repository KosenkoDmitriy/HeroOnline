using UnityEngine;
using System.Collections;

public class UIMusicSetting : MonoBehaviour {

    public enum SlideType
    {
        Ambient,
        Hero,
        Skill,
        Total,
    }

    public UILabel lblAmbient;
    public UILabel lblHero;
    public UILabel lblSkill;
    public UILabel lblTotal;
    public UILabel lblOk;
    public UILabel lblCancel;

    public float ambientValue;
    public float heroValue;
    public float skillValue;
    public float totalValue;

    public UIMusicSilde ambientSlide;
    public UIMusicSilde heroSlide;
    public UIMusicSilde skillSlide;
    public UIMusicSilde totalSlide;

    public static UIMusicSetting Instance { get; private set; }

    public delegate void HandleChangeVolumn();
    public event HandleChangeVolumn OnChangedVolumn;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        //Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {

        ambientSlide.Init();
        heroSlide.Init();
        skillSlide.Init();
        totalSlide.Init();

        Localization();
        LoadValue();

        if (OnChangedVolumn != null)
            OnChangedVolumn.Invoke();
    }

    public void Localization()
    {
        lblAmbient.text = GameManager.localization.GetText("Music_Ambient");
        lblHero.text = GameManager.localization.GetText("Music_Hero");
        lblSkill.text = GameManager.localization.GetText("Music_Skill");
        lblTotal.text = GameManager.localization.GetText("Music_All");
        lblOk.text = GameManager.localization.GetText("Global_btn_OK");
        lblCancel.text = GameManager.localization.GetText("Global_btn_Cancel");
    }

    public void OnAccept_Click()
    {
        SaveValue();
    }

    public void OnCancel_Click()
    {
        LoadValue();
        if (OnChangedVolumn != null)
            OnChangedVolumn.Invoke();
    }

    public void OnChangeValue(SlideType type, float value)
    {
        if (type == SlideType.Ambient)
            ambientValue = value;
        if (type == SlideType.Hero)
            heroValue = value;
        if (type == SlideType.Skill)
            skillValue = value;
        if (type == SlideType.Total)
            totalValue = value;

        if (OnChangedVolumn != null)
            OnChangedVolumn.Invoke();
    }

    private void SaveValue()
    {
        PlayerPrefs.SetFloat("ambientValue", ambientValue);
        PlayerPrefs.SetFloat("heroValue", heroValue);
        PlayerPrefs.SetFloat("skillValue", skillValue);
        PlayerPrefs.SetFloat("totalValue", totalValue);
    }

    private void LoadValue()
    {
        if (PlayerPrefs.HasKey("ambientValue"))
        {
            ambientValue = PlayerPrefs.GetFloat("ambientValue");
        }
        else
        {
            ambientValue = 0.4f;
        }

        if (PlayerPrefs.HasKey("heroValue"))
        {
            heroValue = PlayerPrefs.GetFloat("heroValue");
        }
        else
        {
            heroValue = 0.6f;
        }

        if (PlayerPrefs.HasKey("skillValue"))
        {
            skillValue = PlayerPrefs.GetFloat("skillValue");
        }
        else
        {
            skillValue = 1.0f;
        }

        if (PlayerPrefs.HasKey("totalValue"))
        {
            totalValue = PlayerPrefs.GetFloat("totalValue");
        }
        else
        {
            totalValue = 1.0f;
            SaveValue();
        }

        ambientSlide.SetVolumn(ambientValue);
        heroSlide.SetVolumn(heroValue);
        skillSlide.SetVolumn(skillValue);
        totalSlide.SetVolumn(totalValue);


        if (OnChangedVolumn != null)
            OnChangedVolumn.Invoke();
    }

    public void OnShare_Click()
    {
        SocialManager.Instance.ShareScreenShot();
    }
}
