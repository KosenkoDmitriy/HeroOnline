using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;
public class UIHUDHealthBarController : MonoBehaviour {

    [System.Serializable]
    public struct UIEffect
    {
        public GameObject prefab;
        public UIGrid root;
    }

    public UIFollowTarget uiFollowTarget;

    public UISlider healthBar;
    public UISlider manaBar;
    public UISprite hpsprite;

    public UISprite element;
    public UISprite roleClass;
    public UIEffect uiEffect;

    private Controller _controller;
    private Dictionary<EffectType, GameObject> effectObjectList;

    public void Init(Transform target, string tag, Controller controller)
    {
        _controller = controller;
        effectObjectList = new Dictionary<EffectType, GameObject>();
        uiFollowTarget.target = target;
        uiFollowTarget.gameCamera = Camera.main;
        uiFollowTarget.uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
        // gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        if (tag == GameManager.EnemyTagName)
        {
            if (_controller.isMob)
            {
                hpsprite.spriteName = "HealthBar_Mons_HP_forground";
            }
            else
            {
                hpsprite.spriteName = "HealthBar_Enemy_HP_Forgound";
            }
        }



        element.spriteName = Helper.GetSpriteNameOfElement(controller.role.Base.ElemId);

        if (!controller.isMob)
        {

            roleClass.spriteName = Helper.GetSpriteNameOfRoleClass(controller.role.Base.Class);
        }
        else
        {
            switch (controller.role.Base.Type)
            {
                case DEngine.Common.GameLogic.RoleType.Mob:
                    roleClass.spriteName = "Monster";
                    break;
                case DEngine.Common.GameLogic.RoleType.Elite:
                    roleClass.spriteName = "Elite";
                    break;
                case DEngine.Common.GameLogic.RoleType.Boss:
                    roleClass.spriteName = "Boss";
                    break;
                default:
                    roleClass.gameObject.SetActive(false);
                    break;
            }
        }

      //  spriteType.MakePixelPerfect();
    }

    public void SetHP(float curHP, float maxHP)
    {

        float value = Mathf.Min(curHP / maxHP, 1);
        healthBar.value = value;

       // StopCoroutine("AnimationBar");
     //   StartCoroutine(AnimationBar(curHP, maxHP, healthBar));
    }
    public void SetMP(float curMP, float maxMP)
    {
        float value = Mathf.Min(curMP / maxMP, 1);
        manaBar.value = value;
    }

    private IEnumerator AnimationBar(float value, float max , UISlider bar)
    {
        float curBarValue = bar.value;
        float destValue = value / max;
        float lerpValue = 0;
        float timer = Time.time;

        while (lerpValue != destValue)
        {
            lerpValue = Mathf.Lerp(curBarValue, destValue, (Time.time - timer) * 3);
            bar.value = lerpValue;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void AddEffect(UserRole.MagicState magicAttrib)
    {
        int idEffect = (int)magicAttrib.EffectType;
        GameObject go = NGUITools.AddChild(uiEffect.root.gameObject, uiEffect.prefab);

        go.GetComponent<UIEffectEvent>().effectID = idEffect;

        UISpriteAnimation anim = go.GetComponent<UISpriteAnimation>();
        string effectPrefix = string.Format("effect{0:D2}_", idEffect);

        go.GetComponent<UISprite>().spriteName = effectPrefix + "1";
        anim.namePrefix = effectPrefix;
        go.SetActive(true);

        if (effectObjectList.ContainsKey(magicAttrib.EffectType))
        {
            RemoveEffect(magicAttrib.EffectType);
        }

        effectObjectList.Add(magicAttrib.EffectType, go);        

        uiEffect.root.Reposition();
    }

    public void ClearAllEffect()
    {
        foreach( var item in effectObjectList)
        {
            NGUITools.Destroy(item.Value);
            uiEffect.root.Reposition();
        }
        effectObjectList.Clear();
    }

    public void RemoveEffect(EffectType effectType)
    {
        if (effectObjectList[effectType] != null)
            NGUITools.Destroy(effectObjectList[effectType]);
        effectObjectList.Remove(effectType);
        uiEffect.root.Reposition();

    }
}
