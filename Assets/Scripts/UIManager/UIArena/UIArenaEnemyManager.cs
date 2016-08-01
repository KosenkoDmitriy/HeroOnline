using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIArenaEnemyManager : MonoBehaviour {

    [System.Serializable]
    public class Role
    {
        public UILabel lblname;
        public UITexture grade;
        public UITexture icon;
        public UIHeroStarManager starManager;

        public UISprite element;
        public UISprite RoleClass;

        public void SetRole(UserRole role)
        {
            if (role != null)
            {
                lblname.text = role.Name;
                grade.mainTexture = Helper.LoadTextureElement((int)role.Base.ElemId);
                icon.mainTexture = Helper.LoadTextureForHero(role.Base.RoleId);
                starManager.SetStart(role.Base.Grade);

                element.spriteName = Helper.GetSpriteNameOfElement(role.Base.ElemId);
                RoleClass.spriteName = Helper.GetSpriteNameOfRoleClass(role.Base.Class);
            }
            else
            {
                lblname.text = "";
                grade.mainTexture = null;
                icon.mainTexture = null;
                starManager.SetStart(0);
            }
        }
    }

    public enum State
    {
        None,
        FindEnemy,
        CooldownBattle,
        Battle
    }

    public UILabel lblHeader;
    public UILabel lblUsername;
    public UILabel lblLevel;
    public UILabel lblPoint;
    public UILabel lblWin;
    public UILabel lblLost;
    public UILabel lblRater;
    public Role[] roles;
    public UILabel lbl_btnLetStart;
    public UILabel lbl_btnLetRefresh;

    public GameObject enemyRoot;
    public UIArenaManager _marenaManager;

    private float timerChallenge = -1;

    private State _state;
    private float _timerJoinBattle = 5;
    private AsyncOperation _async = null;

    void Start()
    {
       // _state = State.None;
        Localization();
    }

    void Update()
    {
        switch (_state)
        {
            case State.FindEnemy:
                {
                    if (timerChallenge != -1)
                    {
                        float dt = Time.time - timerChallenge;
                        float m = dt / 60.0f;
                        string time = string.Format("{0:00}:{1:00}", (int)m, (int)dt % 60);
                        lblHeader.text = string.Format(GameManager.localization.GetText("LookingForBattle"), time);
                    }

                }
                break;
            case State.CooldownBattle:
                {
                    _timerJoinBattle -= Time.deltaTime;
                    lblHeader.text = string.Format(GameManager.localization.GetText("Arena_Fight_Header"), 0, _timerJoinBattle);
                    //Debug.Log("process: " + _async.progress);
                    if (_timerJoinBattle <= 0)
                    {
                        ChangeScene();
                    }
                }
                break;

        }
       
    }

    public void Init()
    {
        timerChallenge = Time.time;
        enemyRoot.SetActive(false);
        _state = State.FindEnemy;
       // StartCoroutine(LoadingSence());
    }

    public void SetInfoEnemy()
    {

        float win = 0, lose = 0;
        float ratio = 0;

        //GameUser gameUser = GameManager.EnemyUser;

        win = GameManager.GameUser.Base.TotalWon;
        lose = GameManager.GameUser.Base.TotalLost;

        if ((win + lose) > 0)
            ratio = (win / (win + lose)) * 100;
        else
            ratio = 0;

        enemyRoot.SetActive(true);
        lblUsername.text = GameManager.EnemyUser.Base.NickName;
        lblLevel.text = GameManager.localization.GetText("Global_Level") + GameManager.EnemyUser.Base.Level;
        lblPoint.text = string.Format(GameManager.localization.GetText("Arena_EnemyPoint"), GameManager.EnemyUser.Base.Level);
        lblWin.text = string.Format(GameManager.localization.GetText("Arena_Win"), string.Format("[00FF00]{0}[-]", win));
        lblLost.text = string.Format(GameManager.localization.GetText("Arena_Lose"), string.Format("[FF0000]{0}[-]", lose));
        lblRater.text = string.Format(GameManager.localization.GetText("Arena_Ratio"), ratio.ToString("0.0") + " %");


        for (int i = 0; i < 3; i++)
        {
            if (i < GameplayManager.enemyRoles.Count)
            {
                roles[i].SetRole(GameplayManager.enemyRoles[i]);
            }
            else
            {
                roles[i].SetRole(null);
            }
        }

        _state = State.CooldownBattle;
        _timerJoinBattle = 5;

       StartCoroutine(LoadingSence());
    }

    private void Localization()
    {
        lbl_btnLetStart.text = GameManager.localization.GetText("Arena_btnLetStart");
    }

    private void ChangeScene()
    {
        if (_async != null)
        {
            Debug.Log("_async.allowSceneActivation");
            GameScenes.currentSence = GameScenes.MyScene.Battle;
            _state = State.Battle;
            _async.allowSceneActivation = true;
        }       
    }
    
    private IEnumerator LoadingSence()
    {        
        _async = Application.LoadLevelAsync(GameScenes.MyScene.Battle.ToString());
        _async.allowSceneActivation = false;
        yield return _async;
        Debug.Log("Loading complete");
    }

    #region Button 
    public void OnButtonClose_Click()
    {
        if (_state == State.Battle || _state == State.CooldownBattle) return;
        timerChallenge = -1;
        gameObject.SetActive(false);
        _marenaManager.OnExitArene();
    }

    public void OnButtonLetStart_Click()
    {
    }

    public void OnButtonRefresh_Click()
    {
    }
    #endregion
}
