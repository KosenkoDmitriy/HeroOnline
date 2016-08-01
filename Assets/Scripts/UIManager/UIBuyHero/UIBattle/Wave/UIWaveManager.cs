using UnityEngine;
using System.Collections;

public class UIWaveManager : MonoBehaviour {

    public TweenScale root;
    public UILabel lblNextWave;
    public UIPlayTween playTweenOpen;
    public float delayTime = 2;

    public float speedOpen = 0.5f;
    public float speedClose = 0.2f;

    private static UIWaveManager _instance;
    public static UIWaveManager Instance { get { return _instance; } }

    int _waveIndex=-1;

    public void Start()
    {
        _waveIndex = -1;
        _instance = this;
    }

    public void Show()
    {
        _waveIndex++;
        if (_waveIndex == 0)
        {
            lblNextWave.text = GameManager.localization.GetText("PVE_StartWave");         
        }
        else
        {
            lblNextWave.text = GameManager.localization.GetText("PVE_NextWave");
        }
        root.duration = speedOpen;
        playTweenOpen.Play(true);
        StartCoroutine(Close());
    }

    IEnumerator Close()
    {
        yield return new WaitForSeconds(delayTime);

        root.duration = speedClose;
        UIPlayTween close = new UIPlayTween();
        close.tweenTarget = root.gameObject;
        close.playDirection = AnimationOrTween.Direction.Reverse;
        close.ifDisabledOnPlay = AnimationOrTween.EnableCondition.EnableThenPlay;
        close.disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;
        close.Play(true);

        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed && _waveIndex == 0)
        {
            UIBattleManager.Instance.OnBattleStart();
        }
        else
        {
            GameplayManager.Instance.ResumeGame();

            yield return new WaitForSeconds(1);
            StartCoroutine(GameplayManager.Instance.CreateMobsNextWave());

        }
    }

}
