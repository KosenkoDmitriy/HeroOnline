using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class SoundManager : MonoBehaviour 
{
    public static SoundManager Instance { get { return _instance; } }
    private static SoundManager _instance;

    private AudioSource audioSource;

    private string pathSound = "Sound/";
    public AudioSource ambientSound;

    void Awake()
    {

        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }

        

        audioSource = GetComponent<AudioSource>();      
    }

    void Start()
    {
        UIMusicSetting.Instance.OnChangedVolumn += OnChangedVolumn;
        UIMusicSetting.Instance.Init();
    }

    void Update()
    {
        MyInput.CheckInput();
    }

    public void PlaySkillCast(Controller controller , int index)
    {
        if (controller.isMob) return;

        string audioName = string.Format("{0:D2}_{1:D2}", controller.role.GameRole.Id, index);
        AudioClip clip = Resources.Load("Sound/Hero/Cast/" + audioName) as AudioClip;

        controller.audio.volume = UIMusicSetting.Instance.heroValue * UIMusicSetting.Instance.totalValue;
        controller.audio.PlayOneShot(clip);
    }
    
    public void PlayDeath(Controller controller)
    {
        if (controller.isMob) return;

        string audioName = string.Format("{0:D2}", controller.role.GameRole.Id);
        AudioClip clip = Resources.Load("Sound/Hero/Death/" + audioName) as AudioClip;

        controller.audio.volume = UIMusicSetting.Instance.heroValue * UIMusicSetting.Instance.totalValue;
        controller.audio.PlayOneShot(clip);
    }

    public void PlayAction(Controller controller)
    {
        if (controller.isMob)
        {
            string className = "Ranger";
            int actionID = Random.Range(1, 3);

            if (controller.role.GameRole.Class == (int)RoleClass.Healer)
            {
                actionID = 1;
                className = "Healer";
            }
            else if (controller.role.GameRole.Class == (int)RoleClass.Warrior
                    || controller.role.GameRole.Class == (int)RoleClass.Tanker)
            {
                className = "Tanker";
                actionID = Random.Range(1, 7);
            }

            string audioName = string.Format("Mob_{0}_{1:D2}", className, actionID);
            AudioClip clip = Resources.Load("Sound/Skill/Action/" + audioName) as AudioClip;
            controller.audio.volume = UIMusicSetting.Instance.skillValue * 0.6f * UIMusicSetting.Instance.totalValue;
            controller.audio.PlayOneShot(clip);

        }
        else
        {
            int actionID = Random.Range(1, 3);
            string audioName = string.Format("{0:D2}_{1:D2}", controller.role.GameRole.Id, actionID);
            AudioClip clip = Resources.Load("Sound/Skill/Action/" + audioName) as AudioClip;
            controller.audio.volume = UIMusicSetting.Instance.skillValue * 0.6f * UIMusicSetting.Instance.totalValue; 
            controller.audio.PlayOneShot(clip);

        }
    }

    public void PlaySkillBegin(RoleSkill skill, Controller controller)
    {
        AudioClip clip = null;
        string audioName = "";

        if (controller.role.GameRole.Class == (int)RoleClass.Healer)
        {
            audioName = string.Format("{0:D2}_{1:D2}", skill.GameSkill.Id, controller.role.GameRole.Id);
            clip = Resources.Load("Sound/Skill/Skill/" + audioName) as AudioClip;
        }

        if (clip == null)
        {
            audioName = string.Format("{0:D2}", skill.GameSkill.Id);
            clip = Resources.Load("Sound/Skill/Skill/" + audioName) as AudioClip;
        }

        if (clip != null)
        {
            controller.audio.volume = UIMusicSetting.Instance.skillValue * UIMusicSetting.Instance.totalValue;
            controller.audio.PlayOneShot(clip);
        }
    }

    public void PlaySkillEnd(RoleSkill skill, Controller controller)
    {
        string audioName = string.Format("End_{0:D2}", skill.GameSkill.Id);
        AudioClip clip = Resources.Load("Sound/Skill/Skill/" + audioName) as AudioClip;

        if (clip != null)
        {
            controller.audio.volume = UIMusicSetting.Instance.skillValue * UIMusicSetting.Instance.totalValue;
            controller.audio.PlayOneShot(clip);
        }

    }

    public void PlayOneShot(string name)
    {
        string paht = pathSound + name;

        AudioClip clip = Resources.Load(paht) as AudioClip;
        if (clip != null)
        {
            audioSource.volume = UIMusicSetting.Instance.totalValue;
            audioSource.PlayOneShot(clip);
        }

    }

    public void PlayAmbient(string name)
    {
        string paht = pathSound + name;

        AudioClip clip = Resources.Load(paht) as AudioClip;
        if (clip != null)
        {
            ambientSound.loop = true;
            ambientSound.volume = UIMusicSetting.Instance.ambientValue * UIMusicSetting.Instance.totalValue;
            ambientSound.clip = clip;
            ambientSound.Play();
        }
    }

    public void OnChangedVolumn()
    {
        ambientSound.volume = UIMusicSetting.Instance.ambientValue * UIMusicSetting.Instance.totalValue;
    }

    public void StopAmbient()
    {
        ambientSound.Stop();
    }
}
