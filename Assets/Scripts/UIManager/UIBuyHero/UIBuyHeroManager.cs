using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Collections.Generic;
using System.Linq;

public class UIBuyHeroManager : MonoBehaviour {

    [System.Serializable]
    public struct UIObject
    {
        public UILabel lblSilver;
        public UILabel lblGold;
        public UISprite priceCore;


        public UILabel lblUserGold;
        public UILabel lblUserSilver;

        public GameObject runeRoot;
        public GameObject rune1;
        public GameObject rune2;
        public GameObject rune3;
        public GameObject runeBorder;
    }
    [System.Serializable]
    public struct RotateRune
    {
        public GameObject gameObject;
        public float delay;
        public float speed;
        public float maxSpeed;
        [HideInInspector]
        public float curSpeed;
        
    }
    [System.Serializable]
    public struct UITutorial
    {
        public GameObject hand;
        public GameObject effectClick;
        public Transform startPoint;
        public Transform endPoint;
        public GameObject arrowExit;
    }

    public enum PriceType
    {
        None,
        Silver,
        Gold
    }

    public enum State
    {
        Start,
        RotateMiniRune,
        RotateRootRune,
        HideRune,
        HidingRune,
        ShowEffect,
        SpawnHero,
        MoveHero,
        ShowInfo,
        End
    }

    public UIObject uiObjects;
    public RotateRune[] rotateRunes;
    public RotateRune rotateRuneRoot;
    public Transform spawnPoint;
    public Transform endPoint;
    public UIHeroNewDetail heroDetail;
    public UITutorial uiTutorial;
    public GameObject effect;

    private string _silverPriceName = "silver_coin";
    private string _goldPriceName = "gold_coin";
    private List<GameObj> _shopItems;
    private PriceType _priceType;
    private bool _locked;
    private BuyHeroController _controller;
    private State _state;
    private float _timerRotateMiniRune;
    private float _timerRotateRune;
    private string heroPath = "Prefabs/Heroes/Gunner";
    private GameObject hero;
    private UserRole roleBuy;

    void Start()
    {
        //check status of tutorial , if in step summoned hero then continues
        Tutorial();

        //hand auto move to target 
        StartCoroutine(MoveHand());

        _locked = false;
        _priceType = PriceType.None;
        _state = State.Start;
        _timerRotateMiniRune = Time.time;
        _timerRotateRune = Time.time;
        _shopItems = GameManager.ChargeShop.Where(p => ((ShopItem)p).ItemKind == ItemKind.Hero).ToList();
        _controller = new BuyHeroController(this);
        uiObjects.lblSilver.text = string.Format("{0}", ((ShopItem)_shopItems[0]).PriceSilver);
        uiObjects.lblGold.text = string.Format("{0}", ((ShopItem)_shopItems[1]).PriceGold);
        OnRefreshGold();
    }

    //update Current state 
    void Update()
    {
        switch (_state)
        {
            //animation Rune 
            case State.RotateRootRune:
                {
                    RotateRootRune();
                    RotateMiniRune();
                }
                break;
            case State.RotateMiniRune:
                RotateMiniRune();
                break;
            case State.HideRune:
                StartCoroutine(HideRune());
                break;

            case State.ShowEffect:
                SpawnEffect();
                break;


            case State.SpawnHero:
                StartCoroutine(SpawnHero());
                _state = State.MoveHero;
               break;

            //show base attributes of Hero
            case State.ShowInfo:
               ShowInforHero();
                break;
         
        }
    }

    #region EventMethods

    //Buy Hero by Gold
    //drag gold to target for summon Hero
    public void OnDragGold()
    {
        if (_locked) return;
        if (_priceType != PriceType.Gold)
        {
            UICursorManager.Set(_goldPriceName);
            _priceType = PriceType.Gold;

            iTween.Stop(uiTutorial.hand);
            uiTutorial.hand.SetActive(false);
        }
    }

    //Buy Hero by Silver
    //drag silver to target for summon Hero
    public void OnDragSilver()
    {
        if (_locked) return;
        if (_priceType != PriceType.Silver)
        {
            UICursorManager.Set(_silverPriceName);
            _priceType = PriceType.Silver;

            iTween.Stop(uiTutorial.hand);
            uiTutorial.hand.SetActive(false);
        }
    }
    public void OnDropRun()
    {
        if (_locked) return;
        if (_priceType != PriceType.None)
        {
            UICursorManager.Clear();
            BuyHero();
        }
    }
    public void OnDragRelease()
    {
        _priceType = PriceType.None;
        UICursorManager.Clear();
    }
    public void OnButtonBack()
    {
        if (GameManager.GameUser.UserRoles.Count < 3)
        {
           // MessageBox.ShowDialog(GameManager.localization.GetText("ErrorCode_SummonTutorial_Not3Hero"), UINoticeManager.NoticeType.Message);
            GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("ErrorCode_SummonTutorial_Not3Hero"));
            return;
        }
        GameScenes.ChangeScense(GameScenes.MyScene.BuyHero, GameScenes.MyScene.WorldMap);
    }
    public void OnFinishedHideRune()
    {
        Debug.Log("OnFinishedHideRune");
        _state = State.ShowEffect;
    }
    public void OnCloseHeroDetail()
    {
        Destroy(hero);
        uiObjects.runeRoot.SetActive(true);
        uiObjects.runeRoot.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        uiObjects.runeRoot.GetComponent<TweenScale>().enabled = false;
        _locked = false;
        _state = State.Start;
        uiObjects.priceCore.gameObject.SetActive(false);
        MoveHand();

        OnFinishedBuyHeroTut();
    }
    #endregion

    #region Public methods
    public void OnRefreshGold()
    {
        uiObjects.lblUserGold.text = GameManager.GameUser.Base.Gold.ToString();
        uiObjects.lblUserSilver.text = GameManager.GameUser.Base.Silver.ToString();
    }
    public void OnResponseBuyHero()
    {
        _controller.SendRequestRefreshGold();

        roleBuy = GameManager.GameUser.UserRoles[GameManager.GameUser.UserRoles.Count - 1];
        heroPath = "Prefabs/" + roleBuy.GameRole.AssetPath;
        if (_priceType == PriceType.Silver)
        {
            uiObjects.priceCore.spriteName = _silverPriceName;
        }
        else if (_priceType == PriceType.Gold)
        {
            uiObjects.priceCore.spriteName = _goldPriceName;
        }
        uiObjects.priceCore.gameObject.SetActive(true);
        _priceType = PriceType.None;
        AnimationDropRune();
    }
    #endregion

    #region Private Methods
    private void BuyHero()
    {
        if (CheckBuy())
        {
            _locked = true;
            if (_priceType == PriceType.Silver)
                _controller.SendRequestBuyItem(_shopItems[0].Id, 0);
            else if (_priceType == PriceType.Gold)
                _controller.SendRequestBuyItem(_shopItems[1].Id, 0);

            //OnResponseBuyHero();
        }
    }
    private bool CheckBuy()
    {
        if (_locked) return false;

        if (GameManager.GameUser.UserRoles.Count >= GameManager.maxSlotHero)
        {
            MessageBox.ShowDialog(string.Format(GameManager.localization.GetText("Shop_MaxHeroSlot"), GameManager.maxSlotHero), UINoticeManager.NoticeType.Message);
            return false;
        }

        if (_priceType == PriceType.Silver)
        {
            if (GameManager.GameUser.Base.Silver < ((ShopItem)_shopItems[0]).PriceSilver)
            {
                Helper.HandleCashInsufficient();
                return false;
            }
        }
        else if (_priceType == PriceType.Gold)
        {
            if (GameManager.GameUser.Base.Gold < ((ShopItem)_shopItems[1]).PriceGold)
            {
                Helper.HandleCashInsufficient();
                return false;
            }
        }

        return true;
    }
    #endregion

    #region animation
    private void SpawnEffect()
    {
        GameObject.Instantiate(effect, Vector3.forward * 2, Quaternion.identity);
        _state = State.SpawnHero;
    }
    private IEnumerator MoveHand()
    {
        uiTutorial.hand.SetActive(true);
        uiTutorial.hand.transform.position = uiTutorial.startPoint.position;       

        Hashtable hash = new Hashtable();
        hash["name"] = "movehand";
        hash["position"] = uiTutorial.endPoint;
        hash["time"] = 4;
        hash["easetype"] = iTween.EaseType.easeInOutExpo;
        iTween.MoveTo(uiTutorial.hand, hash);

        yield return new WaitForSeconds(1);
        uiTutorial.effectClick.SetActive(false);
        yield return new WaitForSeconds(4);
        uiTutorial.hand.SetActive(false);
    }
    private IEnumerator SpawnHero()
    {
        yield return new WaitForSeconds(3.5f);
        hero = GameObject.Instantiate(Resources.Load(heroPath) as GameObject, spawnPoint.transform.position, Quaternion.identity) as GameObject;
        Vector3 lookPos = endPoint.position;
        lookPos.y = hero.transform.position.y;
        hero.transform.LookAt(lookPos);
        hero.GetComponent<Controller>().enabled = false;

        StartCoroutine(HeroToEndPoint());
    }

    // hero move to point
    private IEnumerator HeroToEndPoint()
    {
        Animation anim = hero.GetComponent<Animation>();
        if (anim["hoisinh"])
        {
            anim.CrossFade("hoisinh");
            yield return new WaitForSeconds(anim["hoisinh"].length);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
        anim.CrossFade("dung");
        yield return new WaitForSeconds(0.5f);
        float distance = Vector3.Distance(hero.transform.position, endPoint.position);
        //Debug.Log(distance);
        while (distance > 1.0f)
        {
            anim.CrossFade("di");
            hero.transform.Translate(Vector3.forward * Time.deltaTime * 4);
            distance = Vector3.Distance(hero.transform.position, endPoint.position);
            yield return null;
        }

        anim.CrossFade("dung");
        yield return new WaitForSeconds(0.5f);
        _state = State.ShowInfo;
        
    }

    //show base info of Hero
    private void ShowInforHero()
    {
        heroDetail.SetHero(roleBuy);
        UIPlayTween playTween = new UIPlayTween();
        playTween.tweenTarget = heroDetail.gameObject;
        playTween.playDirection = AnimationOrTween.Direction.Forward;
        playTween.ifDisabledOnPlay = AnimationOrTween.EnableCondition.EnableThenPlay;
        playTween.disableWhenFinished = AnimationOrTween.DisableCondition.DoNotDisable;
        playTween.Play(true);
        _state = State.End;
    }
    private IEnumerator HideRune()
    {
        yield return new WaitForSeconds(3);
        _state = State.HidingRune;
        GetComponent<UIPlayTween>().Play(true);
    }   
    private void RotateRootRune()
    {
        if (Time.time - _timerRotateRune >= rotateRuneRoot.delay)
        {
            rotateRuneRoot.curSpeed += rotateRuneRoot.speed * Time.deltaTime;
            rotateRuneRoot.curSpeed = Mathf.Min(rotateRuneRoot.curSpeed, rotateRuneRoot.maxSpeed);
            rotateRuneRoot.gameObject.transform.Rotate(-Vector3.forward * rotateRuneRoot.curSpeed);
        }
    }
    private void RotateMiniRune()
    {
        int count = 0;
        for (int i = 0; i < rotateRunes.Length;i++ )
        {
            if (Time.time - _timerRotateMiniRune >= rotateRunes[i].delay)
            {
                count++;
                rotateRunes[i].curSpeed += rotateRunes[i].speed * Time.deltaTime;
                rotateRunes[i].curSpeed = Mathf.Min(rotateRunes[i].curSpeed, rotateRunes[i].maxSpeed);
                rotateRunes[i].gameObject.transform.Rotate(-Vector3.forward * rotateRunes[i].curSpeed);
            }
        }
        if (count >= 3)
        {            
            _state = State.RotateRootRune;
        }
    }
    private void AnimationDropRune()
    {
        Vector3 scaleTo = new Vector3(1.1f, 1.1f, 1);
        Vector3 scaleEnd = new Vector3(1, 1, 1);

        Hashtable hash = new Hashtable();
        hash["scale"] = scaleTo;
        hash["time"] = 0.1f;
        hash["delay"] = 0.0f;
        hash["looptype"] = iTween.LoopType.none;
        iTween.ScaleTo(uiObjects.priceCore.gameObject, hash);

        hash = new Hashtable();
        hash["scale"] = scaleEnd;
        hash["time"] = 0.1f;
        hash["delay"] = 0.1f;
        hash["looptype"] = iTween.LoopType.none;
        //  hash["oncomplete"] = "AnimationRune";
        iTween.ScaleTo(uiObjects.priceCore.gameObject, hash);
        StartCoroutine(AnimationRune(0));

    }
    private IEnumerator AnimationRune(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hashtable hash = new Hashtable();

        _state = State.RotateMiniRune;
        _timerRotateMiniRune = Time.time;
        _timerRotateRune = Time.time;
        StartCoroutine(HideRune());
    }
    #endregion

    #region Tutorial
    private void Tutorial()
    {
        if (GameManager.tutorial.step == TutorialManager.TutorialStep.SummonHeroes_BuyHero)
        {
            GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Shame1, GameManager.localization.GetText("Tut_BuyHero"));
        }
    }
    private void OnFinishedBuyHeroTut()
    {
        StartCoroutine(MoveHand());

        if (GameManager.tutorial.step == TutorialManager.TutorialStep.SummonHeroes_BuyHero)
        {
            if (GameManager.GameUser.UserRoles.Count >= 3)
            {
                uiTutorial.arrowExit.gameObject.SetActive(true);

                GameManager.tutorial.CreateNPC(UINPCTutorialManager.Status.Normal, GameManager.localization.GetText("Tut_BuyHeroFinished"));
                GameManager.tutorial.ChangeStep(TutorialManager.TutorialStep.BuyHero_Exit);
            }
          
        }
    }

    #endregion
}
