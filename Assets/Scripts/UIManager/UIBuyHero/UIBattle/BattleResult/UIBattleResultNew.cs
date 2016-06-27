using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;
using DEngine.Common.Config;

public class UIBattleResultNew : MonoBehaviour {

    [System.Serializable]
    public struct BattleAward
    {
        public string name;
        public float level;
        public float Exp;
        public float gold;
        public float silver;
        public int silverSign;
        public bool finnish;
        public bool isLevelUp;
        public float honor;
        public float HeroEXP;
        public bool EndDungeon;
        public float silverEx;
                
        public void Update(float duration, BattleAward max)
        {
            int count = 0;

            Exp += max.Exp / duration;
            silver += max.silver / duration;
            gold += max.gold / duration;
            honor = max.honor;
            HeroEXP += max.HeroEXP / duration;

            if (Exp > max.Exp || max.Exp==0)
            {
                Exp = max.Exp;
                count++;
            }
            if (gold > max.gold || max.gold == 0)
            {
                gold = max.gold;
                count++;
            }
            if (silver > max.silver || max.silver == 0)
            {
                silver = max.silver;
                count++;
            }
            if (HeroEXP > max.HeroEXP || max.HeroEXP == 0)
            {
                HeroEXP = max.HeroEXP;
                count++;
            }
            if (count >= 4)
                finnish = true;
        }

    }  

    [System.Serializable]
    public struct UITab1
    {
        public UIPlayTween tweenOpenHeroWindow;
        public UILabel lblGold;
        public UILabel lblSilver;
        public UILabel lblEXPUser;
        public UIGrid roleRoot;
        public GameObject heroSlot;
        public UISprite chest;
        public AnimationCurve openChestCurve;
        public UISprite openChestEffect;
        public GameObject tab1Root;
        public UILabel lblHeader;
    }

    [System.Serializable]
    public struct UITab2
    {
        public GameObject tab2Root;
        public GameObject itemWindown;
        public UIGrid itemRoot;
        public GameObject itemSlot;
       // public UILabel lblButtonNext;
    }

    [System.Serializable]
    public struct UITab3
    {
        public GameObject tab3Root;
        public GameObject heroEnergy;
        public UIGrid itemRoot;
        public UILabel lblButtonReplay;
        public UILabel lblButtonNext;
        public UILabel lblButtonWorldMap;
        public UILabel lblButtonShop;
        public UILabel lblButtonHero;
        public UILabel lblAddfriend;
    }

    public enum State
    {
        Tab1,
        Tab2,
        Tab3,
    }

    public GameObject winText;
    public GameObject loseText;
    public GameAward award;
    public BattleAward battleAward;
    public UITab1 uiTab1;
    public UITab2 uiTab2;
    public UITab3 uiTab3;
    public float delay = 1.5f;
    public bool win = true;

    private BattleAward _battleAwardTemp;
    private List<UIBattleResultNewHeroSlot> _heroSlot;
    private BattleResultController _controller;
    private State _state;

    void Start()
    {     
        Localization();
    }
    //260   -52
    public void ShowWindow()
    {
        _state = State.Tab1;
        StartCoroutine(Show());

        if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed)
        {
            GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.Equip_Begin);
        }

        if (GameManager.battleType == BattleMode.RandomPvA || GameManager.battleType == BattleMode.RandomPvP)
        {
            if (GameManager.GameUser.UserFriends.FirstOrDefault(p => p.Opponent.Id == GameManager.EnemyUser.Id && (p.Mode == (int)UserRelation.FriendOne || p.Mode == (int)UserRelation.FriendTwo)) == null)
                uiTab3.lblAddfriend.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            uiTab3.lblAddfriend.transform.parent.gameObject.SetActive(false);
        }
    }

    #region private methods
    private IEnumerator Show()
    {      

        if (GameManager.battleType != BattleMode.RandomPvE || GameManager.Status == GameStatus.Dungeon)
        {
            uiTab3.lblButtonNext.transform.parent.gameObject.SetActive(false);
            uiTab3.lblButtonReplay.transform.parent.gameObject.SetActive(false);
        }

        if (win)
        {
            SoundManager.Instance.PlayOneShot("victory");
            winText.SetActive(true);
        }
        else
        {
            SoundManager.Instance.PlayOneShot("defeat");
            loseText.SetActive(true);
            delay /= 2;
        }

        yield return new WaitForSeconds(delay);

        if (GameManager.Status == GameStatus.Dungeon && !battleAward.EndDungeon && win)
        {
            GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Dungeon);
            yield return null;
        }
        else
        {
            StartCoroutine(OpenRoleWindow());
        }
    }
    
    private IEnumerator OpenRoleWindow()
    {
        Hashtable hash = new Hashtable();
        hash["y"] = 200;
        hash["time"] = 0.3f;
        hash["islocal"] = true;
        hash["easetype"] = iTween.EaseType.linear;

        iTween.MoveTo(winText, hash);
        iTween.MoveTo(loseText, hash);

        yield return new WaitForSeconds(0.3f);

        uiTab1.tweenOpenHeroWindow.Play(true);

        yield return null;
    }

    public void OpenRoleWinDowFinished()
    {
        _heroSlot = new List<UIBattleResultNewHeroSlot>();

        List<UserRole> userRoles = GameManager.GameUser.UserRoles.Where(p => p.Base.Status == RoleStatus.Active).ToList();

        for (int i = 0; i < userRoles.Count; i++)
        {
            GameObject go = NGUITools.AddChild(uiTab1.roleRoot.gameObject, uiTab1.heroSlot);
            go.SetActive(true);
            UIBattleResultNewHeroSlot slot = go.GetComponent<UIBattleResultNewHeroSlot>();
            slot.SetRole(userRoles[i]);
            _heroSlot.Add(slot);
        }
        uiTab1.roleRoot.Reposition();

        StartCoroutine(UpdateValue());

        if (battleAward.isLevelUp)
        {
            GameObject levelUpObject = GameObject.Instantiate(Resources.Load("Prefabs/UI/UILevelUp") as GameObject) as GameObject;
            levelUpObject.GetComponent<UILevelUp>().Init(GameManager.GameUser.Base.Level);
        }
    }

    private IEnumerator UpdateValue()
    {
        float oldEXp = 0;
        _battleAwardTemp = new BattleAward();


        foreach (UIBattleResultNewHeroSlot slot in _heroSlot)
        {
            if (battleAward.HeroEXP > 0)
                slot.ShowThump();
            slot.lblExp.text = battleAward.HeroEXP.ToString("0");
        }
        

        string userExp = GameManager.localization.GetText("BattleResult_TabHero_UserExp");
        while (!_battleAwardTemp.finnish)
        {
            _battleAwardTemp.Update(70, battleAward);

            string silverEx = "";            
            if (battleAward.silverEx > 0)
                silverEx = string.Format("[00FF00](+{0})[-]", battleAward.silverEx);

            string signSilver = "";
            if (battleAward.silverSign < 0)
                signSilver = "-";

            uiTab1.lblSilver.text = signSilver + _battleAwardTemp.silver.ToString("0") + silverEx;
                    
            uiTab1.lblGold.text = _battleAwardTemp.gold.ToString("0");
            uiTab1.lblEXPUser.text = string.Format(userExp, _battleAwardTemp.Exp.ToString("0"));

            foreach (UIBattleResultNewHeroSlot slot in _heroSlot)
            {
                slot.AddExp((int)(_battleAwardTemp.HeroEXP - oldEXp));
            }

            oldEXp = _battleAwardTemp.HeroEXP;
            yield return null;
        }

        if (_battleAwardTemp.HeroEXP > 0)
        {
            foreach (UIBattleResultNewHeroSlot slot in _heroSlot)
            {
                slot.HideThump();
            }
        }

    }

    private IEnumerator ShowChest()
    {
        Hashtable hash = new Hashtable();
        hash["y"] = 0;
        hash["time"] = 0.5f;
        hash["islocal"] = true;
        hash["easetype"] = iTween.EaseType.easeInCubic;
        uiTab1.chest.gameObject.SetActive(true);
        iTween.MoveTo(uiTab1.chest.gameObject, hash);
        Helper.FadeIn(uiTab1.chest, 0.8f, uiTab1.openChestCurve, 0, null);
        yield return null;
    }

    private IEnumerator OnOpenChest()
    {
        uiTab1.openChestEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        uiTab1.chest.GetComponent<UIButton>().normalSprite = "ruong_nau_open";
        yield return new WaitForSeconds(0.3f);
        uiTab1.openChestEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        uiTab1.chest.gameObject.SetActive(false);
        StartCoroutine(ShowItem());
    }

    private IEnumerator ShowItem()
    {
        uiTab2.itemWindown.SetActive(true);

        foreach (var item in GameManager.itemReward)
        {
            GameObject go = NGUITools.AddChild(uiTab2.itemRoot.gameObject, uiTab2.itemSlot);
            go.SetActive(true);
            Texture2D icon = Helper.LoadTextureForEquipItem(item.Key);
            if (icon == null)
                icon = Helper.LoadTextureForSupportItem(item.Key);
            go.transform.FindChild("Texture").GetComponent<UITexture>().mainTexture = icon;
            go.transform.FindChild("Label").GetComponent<UILabel>().text = "x" + item.Value;
        }
        uiTab2.itemRoot.Reposition();

        Hashtable hash = new Hashtable();
        hash["x"] = -1200;
        hash["time"] = 0.75f;
        hash["islocal"] = true;
        hash["easetype"] = iTween.EaseType.easeInCubic;

        iTween.MoveTo(uiTab1.tab1Root, hash);
        hash["x"] = 0;
        iTween.MoveTo(uiTab2.tab2Root, hash);


        GameManager.itemReward.Clear();

        yield return null;
    }

    private IEnumerator ShowEnd()
    {

        List<UserRole> userRoles = GameManager.GameUser.UserRoles.Where(p => p.Base.Status == RoleStatus.Active).ToList();

        for (int i = 0; i < userRoles.Count; i++)
        {
            GameObject go = NGUITools.AddChild(uiTab3.itemRoot.gameObject, uiTab3.heroEnergy);
            UIBattleResultEnergy energySlot = go.GetComponent<UIBattleResultEnergy>();
            energySlot.SetRole(userRoles[i]);
            go.SetActive(true);
        }
        uiTab3.itemRoot.Reposition();

        Hashtable hash = new Hashtable();
        hash["x"] = -1200;
        hash["time"] = 0.75f;
        hash["islocal"] = true;
        hash["easetype"] = iTween.EaseType.easeInCubic;

        iTween.MoveTo(uiTab2.tab2Root, hash);
        hash["x"] = 0;
        iTween.MoveTo(uiTab3.tab3Root, hash);


        _controller = new BattleResultController(this);

        yield return null;
    }

    private void Localization()
    {
        uiTab1.lblHeader.text = GameManager.localization.GetText("BattleResult_TabHero_Header");
       // uiTab2.lblButtonNext.text = GameManager.localization.GetText("BattleResult_TabItem_Next");
        uiTab3.lblButtonHero.text = GameManager.localization.GetText("BattleResult_TabEnd_Hero");
        uiTab3.lblButtonNext.text = GameManager.localization.GetText("BattleResult_TabEnd_Next");
        uiTab3.lblButtonReplay.text = GameManager.localization.GetText("BattleResult_TabEnd_Replay");
        uiTab3.lblButtonWorldMap.text = GameManager.localization.GetText("BattleResult_TabEnd_WorldMap");
        uiTab3.lblButtonShop.text = GameManager.localization.GetText("BattleResult_TabEnd_Shop");

        if (GameManager.EnemyUser != null && (GameManager.battleType == BattleMode.RandomPvA || GameManager.battleType == BattleMode.RandomPvP))
        {
            uiTab3.lblAddfriend.text = string.Format(GameManager.localization.GetText("BattleResult_AddFriend"), GameManager.EnemyUser.Base.NickName);
        }
    }
    #endregion

    #region public methods
    public void OpenChest()
    {
        StartCoroutine(OnOpenChest());
    }
    public void OnTab1_Click()
    {
        if (_state == State.Tab2) return;
        if (GameManager.itemReward.Count > 0)
            StartCoroutine(ShowChest());
        else
            StartCoroutine(ShowItem());

        _state = State.Tab2;
    }
    public void OnButtonTab3_Click()
    {

        if (_state == State.Tab3) return;

        StartCoroutine(ShowEnd());

        _state = State.Tab3;
    }
    public void OnButtonWorldMap_Click()
    {
        if (GameManager.GameUser.Base.TutorStep < (int)TutorialManager.TutorialStep.Equip_Begin)
        {
            GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.Equip_Begin);
        }

        switch (GameManager.Status)
        {          
            case GameStatus.PVP:
                GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Arena);
                break;          
            case GameStatus.Pillage:
                GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Pillage);
                break;
            default:
                GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.WorldMap);
                break;
        }

    }
    public void OnButtonShop_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;

        GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.ChargeShop);
    }
    public void OnButtonHero_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;
        GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Hero);
    }
    public void OnButtonReplay_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;

        if (!GameManager.CheckHeroToBattle()) return;

        _controller.SendRequestBattle(UIPVEMapManager.curMap, UIPVEMapManager.curStandard);
    }
    public void OnButtonNext_Click()
    {
        if (GameManager.tutorial.step < TutorialManager.TutorialStep.Finished) return;
        GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.PVEMap);
    }
    public void OnButtonAddfriend_Click()
    {
        _controller.SendAddFriend(GameManager.EnemyUser.Base.NickName);

    }
    public void OnResponseAddFriendSuccess()
    {
        uiTab3.lblAddfriend.transform.parent.gameObject.SetActive(false);
        MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("BattleResult_AddFriendSuccess"), GameManager.EnemyUser.Base.NickName), UINoticeManager.NoticeType.Message);   
    }
    #endregion
}
