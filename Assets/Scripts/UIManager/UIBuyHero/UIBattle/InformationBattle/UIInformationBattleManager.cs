using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DEngine.Common.GameLogic;

public class UIInformationBattleManager : MonoBehaviour {

    [System.Serializable]
    public struct uiGroupItemSlot
    {
        public UITexture Border;
        public UITexture Icon;
        public void Hide()
        {
            Border.transform.parent.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public struct uiGroupUser
    {
        public UITexture icon;
        public UILabel lblName;
        public UILabel lblLevel;
        public UILabel lblInformation;

        public void Hide()
        {
            icon.transform.parent.parent.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public struct uiGroupHero
    {
        public UITexture icon;
        public UISprite quality;
        public UILabel lblName;
        public UILabel lblLevel;

        public UISprite element;
        public UISprite roleClass;

        public uiGroupItemSlot[] itemSlot;

        public void Hide()
        {
            icon.transform.parent.parent.parent.gameObject.SetActive(false);
        }
    }

    public uiGroupUser uiUserLeft;
    public uiGroupUser uiUserRight;
    public uiGroupHero[] uiLeftHero;
    public uiGroupHero[] uiRightHero;
    public GameObject[] uiGroupTween;
    public GameObject star;

    private List<Controller> enemy;
    private List<Controller> player;
    private bool _isPlayerInLeft = true;
    private bool _isConnectToServer = false;
    private UIPlayTween _playTween;
    void Start()
    {
        _playTween = GetComponent<UIPlayTween>();
        
        player = (from item in GameplayManager.Instance.heroPlayerSet select item.controller).ToList(); 
        enemy = (from item in GameplayManager.Instance.heroEnemySet select item.controller).ToList();

        if (player.Count <= 0)
        {
            Invoke("Hide", 5f);
            return;
        }
        if (player[0].transform.position.x > 0)
            _isPlayerInLeft = false;
        else
            _isPlayerInLeft = true;

        _isConnectToServer = player[0].isConnectedServer();
        
        InitGUI();

        Invoke("Hide", 5f);

    }

    private void Hide()
    {
        foreach (GameObject go in uiGroupTween)
        {
            _playTween.tweenTarget = go;
            _playTween.Play(true);
        }
    }

    private void InitGUI()
    {
        uiGroupUser uiGroupPlayer;
        uiGroupUser uiGroupEnemy;
        uiGroupHero[] uiGroupHeroPlayer;
        uiGroupHero[] uiGroupHeroEnemy;

        if (_isPlayerInLeft)
        {
            uiGroupPlayer = uiUserLeft;
            uiGroupHeroPlayer = uiLeftHero;
            uiGroupEnemy = uiUserRight;
            uiGroupHeroEnemy = uiRightHero;
        }
        else
        {
            uiGroupPlayer = uiUserRight;
            uiGroupHeroPlayer = uiRightHero;
            uiGroupEnemy = uiUserLeft;
            uiGroupHeroEnemy = uiLeftHero;
        }

        if (_isConnectToServer)
        {

            uiGroupPlayer.icon.mainTexture = Helper.LoadTextureForAvatar(GameManager.GameUser.Base.Avatar);
            uiGroupPlayer.lblLevel.text = "Level: " + GameManager.GameUser.Base.Level;
            uiGroupPlayer.lblInformation.text = "Rank: " + GameManager.GameUser.Base.LevelRank;
            uiGroupPlayer.lblName.text = string.Format("{0}", GameManager.GameUser.Base.NickName);


            uiGroupEnemy.icon.mainTexture = Helper.LoadTextureForAvatar(GameManager.EnemyUser.Base.Avatar);

            uiGroupEnemy.lblLevel.text = "Level: " + GameManager.EnemyUser.Base.Level;
            uiGroupEnemy.lblInformation.text = "Rank: " + GameManager.EnemyUser.Base.LevelRank;
            uiGroupEnemy.lblName.text = string.Format("{0}", GameManager.EnemyUser.Base.NickName);
        }
        else
        {
            uiGroupPlayer.Hide();
            uiGroupEnemy.Hide();
        }

        InitRolesGUI(player, uiGroupHeroPlayer);
        InitRolesGUI(enemy, uiGroupHeroEnemy);

    }

    private void InitRolesGUI(List<Controller> controllerSet, uiGroupHero[] uiGroup)
    {
        int count = 0;
        if (controllerSet != null)
        {
            count = Mathf.Min(3, controllerSet.Count);
            for (int i = 0; i < count; i++)
            {
                Controller controller = controllerSet[i];
                uiGroupHero gui = uiGroup[i];

                gui.icon.mainTexture = Helper.LoadTextureForMiniHero(controller.roleID);
                int grade = 4;
                if (controller.isConnectedServer())
                    grade = controller.role.Base.Grade;

                string gradeName = Helper.GetSpriteNameElement(controller.role); 
                gui.quality.spriteName = gradeName;

                gui.lblLevel.text = "Level: " + controller.level;
                gui.lblName.text = controller.roleName;

                gui.element.spriteName = Helper.GetSpriteNameOfElement(controller.role.Base.ElemId);
                gui.roleClass.spriteName = Helper.GetSpriteNameOfRoleClass(controller.role.Base.Class);

                GameObject go = NGUITools.AddChild(gui.quality.gameObject, star);
                go.SetActive(true);
                go.GetComponent<UIHeroStarManager>().SetStart(grade);
                go.transform.localPosition = new Vector3(-46.2f, 41.5f, 0);
                go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

                if (_isConnectToServer)
                {


                    List<UserItem> itemEquip = controller.role.RoleItems.Where(p=>p.GameItem.SubKind == (int)ItemSubKind.Equipment).ToList();// GameManager.GameUser.UserItems.Where(p => p.RoleUId == controller.role.Id).OrderBy(p => p.GameItem.Kind).ToArray();

                    int itemCount = itemEquip.Count;

                    for (int j = 0; j < itemEquip.Count; j++)
                    {
                        gui.itemSlot[j].Icon.mainTexture = Helper.LoadTextureForEquipItem(itemEquip[j].ItemId);
                        Color c = Helper.ItemColor[itemEquip[j].Grade];
                        gui.itemSlot[j].Border.color = c;
                    }

                    for (int j = itemCount; j < 3; j++)
                    {
                        gui.itemSlot[j].Hide();
                    }
                }

            }
        }

        for (int i = count; i < 3; i++)
        {
            uiGroupHero gui = uiGroup[i];
            gui.Hide();
        }
    }
}

