using UnityEngine;
using System.Collections;

public class UIBattleResult : MonoBehaviour {

    [System.Serializable]
    public struct uiRoleResult
    {
        public UILabel lblName;
        public UILabel lblLevel;
        public UILabel lblExp;
        public UILabel lblEnergy;
    }

    [System.Serializable]
    public struct HeroResult
    {
        public string name;
        public int level;
        public float exp;
        public float damage;
        public bool isLevelUp;

        public void Clone(HeroResult hero)
        {
            isLevelUp = hero.isLevelUp;
            name = hero.name;
            level = hero.level;
            exp = hero.exp;
            damage = hero.damage;
            isLevelUp = hero.isLevelUp;
        }
    }

    [System.Serializable]
    public struct UserResult
    {
        public string name;
        public float level;
        public float Exp;
        public float gold;
        public float silver;
        public HeroResult[] heroResult;
        public bool finnish;
        public bool isLevelUp;
        public float honor;
        public float HeroEXP;
       
        public void Clone(UserResult user)
        {           
            finnish = false;
            name = user.name;
            level = user.level;
            Exp = user.Exp;
            gold = user.gold;
            silver = user.silver;
            isLevelUp = user.isLevelUp;
            honor = user.honor;
            HeroEXP = user.HeroEXP;
        }
        public void Clear()
        {
            Exp = 0;
            gold = 0;
            silver = 0;
            honor = 0;
            HeroEXP = 0;
        }

        public void Update( float duration, UserResult max)
        {
            int count = 0;

            Exp += max.Exp / duration;
            silver += max.silver / duration;
            gold += max.gold / duration;
            honor = max.honor;
            HeroEXP += max.HeroEXP / duration;

            if (Exp > max.Exp)
            {
                Exp = max.Exp;
                count++;
            }
            if (gold > max.gold)
            {
                gold = max.gold;
                count++;
            }
            if (silver > max.silver)
            {
                silver = max.silver;
                count++;
            }
            if (HeroEXP > max.HeroEXP)
            {
                HeroEXP = max.HeroEXP;
                count++;
            }
            if (count >= 4)
                finnish = true;
        }

    }

    public UILabel Llevel;
    public UILabel LExp;
    public UILabel LHeroes;

    public UIButton btnShop;
    public UIButton btnHero;
    public UIButton btnLobby;
    public UIButton btnContinue;
    public UIButton btnCancel;

    public UITexture avatar;
    public UILabel lblNickName;
    public UILabel lblLevel;
    public UILabel lblExp;
    public UILabel lblGold;
    public UILabel lblSilver;
    public UILabel lblResult;
    public UILabel lblResultWin;
    public UILabel lblHonor;

    public UIBattleBonus bonus;
    public uiRoleResult[] roleResult;
    public UserResult userResult;
    public GameplayManager.BattleStatus result;
    public bool _reward = true;

    public GameObject ItemRoot;
    public GameObject ItemPrefab;

    private UserResult userResultTemp;
    private bool _endDungeon;
    private bool _loseDungeon;

    public void Start()
    {
        //OnStart();
    }

    public void OnStart(bool endDungeon = false)
    {
        _endDungeon = endDungeon;
        if (GameManager.Status == GameStatus.Dungeon)
        {
            btnHero.gameObject.SetActive(false);
            btnLobby.gameObject.SetActive(false);
            btnShop.gameObject.SetActive(false);
            btnContinue.gameObject.SetActive(true);
            btnCancel.gameObject.SetActive(true);
        }
        else
        {
            btnHero.gameObject.SetActive(true);
            btnLobby.gameObject.SetActive(true);
            btnShop.gameObject.SetActive(true);
            btnContinue.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(false);
        }

        if (GameManager.GameUser != null)
        {
            avatar.mainTexture = Helper.LoadTextureForAvatar(GameManager.GameUser.Base.Avatar);
            lblNickName.text = GameManager.GameUser.Base.NickName;
        }
        int index = 0;
        foreach (var item in GameManager.itemReward)
        {
            GameObject go = NGUITools.AddChild(ItemRoot, ItemPrefab);
            go.GetComponent<UIBattleItem>().SetItem(item.Key, item.Value, index++);
        }
        ItemRoot.GetComponent<UIGrid>().Reposition();

        GameManager.itemReward.Clear();

        switch (result)
        {
            case GameplayManager.BattleStatus.Draw:
                {
                    lblResult.gameObject.SetActive(true);
                    lblResult.text = GameManager.localization.GetText("Dialog_Battle_Draw");
                }
                break;

            case GameplayManager.BattleStatus.Win:
                {
                    lblResultWin.gameObject.SetActive(true);
                    lblResultWin.text = GameManager.localization.GetText("Dialog_Battle_Win");
                    btnCancel.gameObject.SetActive(false);
                    /* if (GameManager.battleType == DEngine.Common.GameLogic.BattleMode.RandomPvE)
                     {
                         bonus.gameObject.SetActive(true);
                         gameObject.SetActive(false);
                         _reward = false;
                     }     */
                }
                break;

            case GameplayManager.BattleStatus.Lose:
                {
                    lblResult.gameObject.SetActive(true);
                    lblResult.text = GameManager.localization.GetText("Dialog_Battle_Lose");
                    btnCancel.gameObject.SetActive(true);
                    if (GameManager.Status == GameStatus.Dungeon)
                    {
                        _loseDungeon = true;
                        btnContinue.gameObject.SetActive(false);
                    }
                }
                break;
        }

        string level = "";
        string localLevel = GameManager.localization.GetText("Global_Level");
        if (userResult.isLevelUp)
            level = string.Format(localLevel + "[00FF00]{0:0}[-]", userResult.level);
        else
            level = string.Format(localLevel + "{0:0}", userResult.level);

        lblLevel.text = level;

        userResultTemp = new UserResult();
        userResultTemp.Clone(userResult);
        userResultTemp.Clear();

        if (lblHonor != null)
        {
            if (userResult.honor == 0)
            {
                lblHonor.gameObject.SetActive(false);
            }
            else
            {
                lblHonor.gameObject.SetActive(true);
            }
        }

        GameplayManager.battleStatus = GameplayManager.BattleStatus.End;
    }

    void Update()
    {
        //if (!_reward) return;

        if (!userResultTemp.finnish)
        {
            //Debug.Log("update");
            userResultTemp.Update(70, userResult);

            //EXPUser
            lblExp.text = string.Format(GameManager.localization.GetText("Global_EXP_User") + ": [00FF00]+{0:0}[-]", userResultTemp.Exp);
           
            //Gold
            lblGold.text = string.Format("Gold: [00FF00]+{0:0}[-]", userResultTemp.gold);
           
            //Silver
            lblSilver.text = string.Format("Silver: [00FF00]+{0:0}[-]", userResultTemp.silver);

           
            if (userResultTemp.honor > 0)
                lblHonor.text = string.Format(GameManager.localization.GetText("Arena_EnemyPoint"), "[00FF00]+" + userResultTemp.honor + "[-]");
            else
                lblHonor.text = string.Format(GameManager.localization.GetText("Arena_EnemyPoint"), "[FF0000]" + userResultTemp.honor + "[-]");

            if (userResultTemp.HeroEXP > 0)
            {
                lblHonor.text = string.Format(GameManager.localization.GetText("Global_EXP_Hero") + ": [00FF00]+{0:0}[-]", userResultTemp.HeroEXP); 
                lblHonor.gameObject.SetActive(true);
            }

           /* int index = 0;
            foreach (HeroResult hero in GameplayManager.playerRoles)
            {
                string level = "";
                if (hero.isLevelUp)
                    level = string.Format("Lvl.[00FF00]{0:0}[-]", hero.level);
                else
                    level = string.Format("Lvl.{0:0}", hero.level);

                roleResult[index].lblLevel.text = level;
                roleResult[index].lblEnergy.text = string.Format("Damage: [FF0000]{0:0}[-]", hero.damage); //"Damage taken: " + hero.damage.ToString("0");
                roleResult[index].lblExp.text = string.Format(GameManager.localization.GetText("Global_btn_Exp") + ": [00FF00]+{0:0}[-]", hero.exp);
            
                roleResult[index].lblName.text = hero.name;
                index++;
            }*/
        }
    }

    #region btn
    public void OnButtonLobby_Click()
    {
        switch (GameManager.Status)
        {
            case GameStatus.Mission:
                GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.PVEMap);
                break;
            case GameStatus.Dungeon:
                GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.WorldMap);
                break;
            case GameStatus.PVP:
                GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Arena);
                break;
            case GameStatus.PVA:
                GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.WorldMap);
                break;
            case GameStatus.Pillage:
                GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Pillage);
                break;
        }
    }
    
    public void OnButtonShop_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.ChargeShop);
    }

    public void OnButtonHero_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Hero);
    }
    public void OnButtonContinue_Click()
    {
        if (_endDungeon)
            GameScenes.ChangeScense(GameScenes.MyScene.Dungeon, GameScenes.MyScene.WorldMap);
        else
            GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.Dungeon);
    }
    public void OnButtonCancel_Click()
    {
        GameScenes.ChangeScense(GameScenes.MyScene.Battle, GameScenes.MyScene.WorldMap);
    }
    #endregion
     
    public void OnReciveReward()
    {
        gameObject.SetActive(true);
    }

    private void Localization()
    {
        LHeroes.text = GameManager.localization.GetText("Shop_Herroes");
    }
}
