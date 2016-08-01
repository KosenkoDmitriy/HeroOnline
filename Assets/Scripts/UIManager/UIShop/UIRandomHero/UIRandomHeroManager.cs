using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DEngine.Common.GameLogic;

public class UIRandomHeroManager : MonoBehaviour
{
    [System.Serializable]
    public struct UITutorial
    {
        public GameObject ArrowSkill;
        public GameObject ArrowExitInfo;
        public GameObject objArrowBuy;
        public GameObject objArrowView;
        public GameObject objArrowExitBuyHero;
    }

    public enum State
    {
        Idle,
        Playing,
        ReduceSpeed,
        End
    }

    public UIHeroNewDetail uiInformationHero;
    public UIRandomHeroSkill skillInfo;

    public UIShopManager shop;
    public GameObject randomRoot;
    public GameObject uislotRefab;

    public UILabel lblPriceGreen;
    public UILabel lblPriceBlue;
    public UILabel lblPriceYellow;
    public UITexture iconHero;
    public UITexture background;

    public UILabel lblStr;
    public UILabel lbAgi;
    public UILabel lblInt;
    public UITexture iconSkill1;
    public UITexture iconSkill2;
    public UIButton inforHero;
    public UIHeroStarManager starManager;
    public GameObject particleRoot;
    public GameObject particlePrefab;
    public UISprite element;
    public UISprite roleClass;

    public int length = 9;
    public float beginX = 280.0f;
    public float deltaX = 140.0f;
    public float maxSpeed = 2;
    public float minSpeed = 0.3f;
    public float windownWith = 624;
    public float maxCircle = 3;
    public int maxHero = 10;
    public State state;

    public UILabel lblArrowBuy;
    public UILabel lblArrowView;

    public UITutorial uiTutorial;

    private List<UIRandomHeroSlot> _slots;
    private float _maxLength;
    private float _speed = 0;
    private Vector2 _range;
    private int _circle;
    private List<GameObj> _shopItems;
    private UserRole _roleBuy;
    private bool _locked;

    void Start()
    {

        _shopItems = GameManager.ChargeShop.Where(p => ((ShopItem)p).ItemKind == ItemKind.Hero).ToList();


        lblPriceGreen.text = string.Format("{0}", ((ShopItem)_shopItems[0]).PriceSilver);
        lblPriceBlue.text = string.Format("{0}", ((ShopItem)_shopItems[1]).PriceGold);
        //lblPriceYellow.text = string.Format("{0}", ((ShopItem)_shopItems[2]).PriceGold);

        state = State.Idle;
        _slots = new List<UIRandomHeroSlot>();
        _maxLength = deltaX * length;
        _speed = 0;
        _range = new Vector2((beginX + deltaX) - _maxLength, beginX + deltaX);
        _circle = 0;
        _locked = false;
        CreateRandomSlot();
        Move();

        lblArrowView.text = GameManager.localization.GetText("Tut_ShopHero_ViewInfo");
        lblArrowBuy.text = GameManager.localization.GetText("Tut_ShopHero_BuyHero");

        uiTutorial.objArrowBuy.SetActive(true);

    }

    void refresh()
    {
        state = State.Idle;

        iconHero.mainTexture = Resources.Load<Texture2D>("HeroIcons/None");
        background.mainTexture = Helper.LoadTextureElement(1);
        lblStr.text = "";
        lbAgi.text = "";
        lblInt.text = "";
        iconSkill1.mainTexture = null;
        iconSkill2.mainTexture = null;
        _roleBuy = null;
        inforHero.gameObject.SetActive(false);
        starManager.SetStart(0);
        foreach (UIRandomHeroSlot slot in _slots)
        {
            Destroy(slot.gameObject);
        }

        _slots.Clear();
        Resources.UnloadUnusedAssets();
        CreateRandomSlot();
        Move();
    }

    void Update()
    {
        if (state == State.Idle || state == State.End) return;

        if (state == State.Playing)
        {
            _speed += Time.smoothDeltaTime * 2;
            _speed = Mathf.Min(_speed, maxSpeed);
        }
        else if (state == State.ReduceSpeed)
        {
            _speed -= Time.smoothDeltaTime;
            _speed = Mathf.Max(_speed, minSpeed);
        }

        Move();
    }

    #region public
    public void OnButtonGreen_Click()
    {

        if (_locked) return;

        if (GameManager.GameUser.UserRoles.Count >= GameManager.maxSlotHero)
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Shop_MaxHeroSlot"), GameManager.maxSlotHero), UINoticeManager.NoticeType.Message);
            return;
        }

        uiTutorial.objArrowBuy.SetActive(false);
        uiTutorial.objArrowView.SetActive(false);

        if (GameManager.GameUser.Base.Silver < ((ShopItem)_shopItems[0]).PriceSilver)
        {
            Helper.HandleCashInsufficient();
            return;
        }
        _locked = true;
        shop.OnBuyHero(_shopItems[0].Id);
    }
    public void OnButtonBlue_Click()
    {

        if (_locked) return;

        if (GameManager.GameUser.UserRoles.Count >= GameManager.maxSlotHero)
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Shop_MaxHeroSlot"), GameManager.maxSlotHero), UINoticeManager.NoticeType.Message);
            return;
        }

        uiTutorial.objArrowBuy.SetActive(false);
        uiTutorial.objArrowView.SetActive(false);

        if (GameManager.GameUser.Base.Silver < ((ShopItem)_shopItems[0]).PriceGold)
        {
            Helper.HandleCashInsufficient();
            return;
        }
        _locked = true;
        shop.OnBuyHero(_shopItems[1].Id);
    }
    public void OnButtonYellow_Click()
    {
        
    }
    public void OnResponseFromServer(UserRole roleBuy)
    {
        if (state == State.ReduceSpeed || state == State.Playing) return;
        if (state == State.End)
        {
            refresh();
        }
        _roleBuy = roleBuy;
        _slots[length - 1].SetSlot(_roleBuy.Base.RoleId, (int)_roleBuy.Base.ElemId, length - 1);
        state = State.Playing;
    }
    public void OnInformation_Click()
    {
        if (_roleBuy != null)
        {
            uiInformationHero.SetHero(_roleBuy);
            skillInfo.SetRole(_roleBuy);

            UIPlayTween tweenPlay = new UIPlayTween();
            tweenPlay.tweenTarget = uiInformationHero.gameObject;
            tweenPlay.playDirection = AnimationOrTween.Direction.Forward;
            tweenPlay.ifDisabledOnPlay = AnimationOrTween.EnableCondition.EnableThenPlay;
            tweenPlay.disableWhenFinished = AnimationOrTween.DisableCondition.DoNotDisable;
            tweenPlay.Play(true);
        }
    }

    public void OnButtonSkill1_Click()
    {
    }

    public void OnButtonSkill2_Click()
    {
    }

    public void OnButtonExitSkillInfo_Click()
    {
    }

    public void OnButtonExitHeroInfo_Click()
    {
    }
    #endregion

    #region private
    private void OnFinishedPlaying()
    {
        //particle.SetActive(true);
        GameObject go = GameObject.Instantiate(particlePrefab) as GameObject;
        go.transform.parent = particleRoot.transform;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.localPosition = Vector3.zero;

        Destroy(go, 5);
        Invoke("ShowInfo", 1.8f);
    }
    private void ShowInfo()
    {
        UserRole gameRole = _roleBuy;

        iconHero.mainTexture = _slots[length - 1].iconHero.mainTexture;
        background.mainTexture = _slots[length - 1].background.mainTexture;
        lblStr.text = GameManager.localization.GetText("Global_Str") + ": " + gameRole.GameRole.Strength;
        lbAgi.text = GameManager.localization.GetText("Global_Agi") + ": " + gameRole.GameRole.Agility;
        lblInt.text = GameManager.localization.GetText("Global_Int") + ": " + gameRole.GameRole.Intelligent;
        iconSkill1.mainTexture = Helper.LoadTextureForSkill(gameRole.RoleSkills[1].SkillId);
        iconSkill2.mainTexture = Helper.LoadTextureForSkill(gameRole.RoleSkills[2].SkillId);
        starManager.SetStart(_roleBuy.Base.Grade);
        element.spriteName = Helper.GetSpriteNameOfElement(_roleBuy.Base.ElemId);
        roleClass.spriteName = Helper.GetSpriteNameOfRoleClass(_roleBuy.Base.Class);

        inforHero.gameObject.SetActive(true);
        _locked = false;

        uiTutorial.objArrowView.SetActive(true);

    }
    private void CreateRandomSlot()
    {
        float poxX = beginX;
        int startHero = Random.Range(0, maxHero);

        for (int i = 0; i < length; i++)
        {
            int idHero = startHero++ % maxHero + 1;
            int quality = Random.Range(1, 6);

            GameObject go = NGUITools.AddChild(randomRoot, uislotRefab);
            go.transform.localPosition = new Vector3(poxX, 0, 0);
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            UIRandomHeroSlot slot = go.GetComponent<UIRandomHeroSlot>();
            slot.SetSlot(idHero, quality, i);
            _slots.Add(slot);
            poxX -= deltaX;
        }
    }
    private void Move()
    {
        foreach (UIRandomHeroSlot slot in _slots)
        {
            slot.transform.Translate(Vector3.right * Time.smoothDeltaTime * _speed, Space.Self);

            if (slot.transform.localPosition.x >= _range.y)
            {
                slot.transform.localPosition = new Vector3(_range.x, 0, 0);
                if (slot.Index == length - 1)
                {
                    _circle++;
                    if (_circle >= maxCircle)
                    {
                        state = State.ReduceSpeed;
                    }
                }
            }

            if (Mathf.Abs(slot.transform.localPosition.x) < deltaX / 2)
            {
                slot.OnSelect();
            }
            else
            {
                slot.OnDeSelect();
            }

            float percent = Mathf.Abs(slot.transform.localPosition.x) / (windownWith / 2);
            float l = Mathf.Lerp(0, 0.3f, percent);
            float scale = 0.9f - l;
            scale = Mathf.Max(scale, 0.6f);
            slot.transform.localScale = new Vector3(scale, scale, 1);


            if (Mathf.Abs(slot.transform.localPosition.x) < 10)
            {
                if (slot.Index == length - 1 && _speed == minSpeed)
                {
                    state = State.End;
                    OnFinishedPlaying();
                }
            }

        }
    }
    #endregion

    
}
