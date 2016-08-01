using UnityEngine;
using System.Collections;
using DEngine.Common.Config;

public class UIPVPSearchManager : MonoBehaviour {

    public UISprite root;
    public UILabel lblText;
    public AnimationCurve fadeAnimCurve;
    public static UIPVPSearchManager Instance;

    IEnumerator Start()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        Destroy(gameObject, 3);

        lblText.text = GameManager.localization.GetText("PVPSearchManager_Text");

        Hashtable hash = new Hashtable();
        hash["y"] = 0;
        hash["time"] = 0.5f;
        hash["islocal"] = true;
        hash["easetype"] = iTween.EaseType.easeInCubic;

        iTween.MoveTo(root.gameObject, hash);
        Helper.FadeIn(root, 0.5f, fadeAnimCurve, 0, null);

        yield return new WaitForSeconds(2);

        hash["y"] = 140;
        hash["time"] = 0.5f;
        hash["islocal"] = true;
        hash["easetype"] = iTween.EaseType.easeInCubic;

        iTween.MoveTo(root.gameObject, hash);
        Helper.FadeOut(root, 0.5f, fadeAnimCurve, null);


        yield return null;
    }

    public void OnClick()
    {
        int levelArena = GameConfig.ARENALEVEL;
        if (GameManager.GameUser.Base.Level < levelArena)
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Arena_NotLevel"), levelArena), UINoticeManager.NoticeType.Message);
            return;
        }

        GameScenes.ChangeScense(GameScenes.currentSence, GameScenes.MyScene.Arena);
    }
}
