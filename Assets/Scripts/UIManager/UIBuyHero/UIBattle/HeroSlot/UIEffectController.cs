using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public class UIEffectController : MonoBehaviour {

    public UITexture icon;
    public UISprite timer;

    private float _duration;
    private float _timeLeft;

    private EffectManager _manager;
    private UserRole.MagicState _magicAttrib;

    public void CreateEffect(EffectManager manager, UserRole.MagicState magicAttrib)
    {
        //Debug.Log("CreateEffect " + magicAttrib.EffectType + " " + magicAttrib.RemainTime);
       /* icon.mainTexture = Resources.Load<Texture2D>(GetIconPath(magicAttrib.EffectType));
        _duration = magicAttrib.RemainTime;
        _timeLeft = _duration;
        _manager = manager;
        _magicAttrib = magicAttrib;
        StartCoroutine(UpdateTimer());*/
    }

    private IEnumerator UpdateTimer()
    {
        while (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;

            timer.fillAmount = _timeLeft / _duration;

            yield return null;
        }

        _manager.RemoveEffect(_magicAttrib.EffectType);
    }

    public void AddDuration(float duration)
    {
        //Debug.Log("AddDuration");
        if (_duration < duration)
        {
            _timeLeft += duration - _duration;
            _duration = duration;
        }
    }


    private string GetIconPath(EffectType type)
    {
        return "";
       /* string path = "Effect/Icons/";
        string name = "";
        switch (type)
        {
            case EffectType.Barrier:
            case EffectType.BattleMage:         name = "effect3_icon"; break;
            case EffectType.Bind:
            case EffectType.Bleed:
            case EffectType.Blind:              name = "effect14_icon"; break;
            case EffectType.Break:
            case EffectType.Burn:
            case EffectType.Cleansing:
            case EffectType.Crack:
            case EffectType.Cripple:
            case EffectType.Disable:            name = "effect10_icon"; break;
            case EffectType.Disorder:
            case EffectType.Freeze:             name = "effect6_icon"; break;
            case EffectType.Frenzy:
            case EffectType.Haste:              name = "effect1_icon"; break;
            case EffectType.Invincible:
            case EffectType.Knockback:
            case EffectType.Lame:
            case EffectType.ManaBurn:
            case EffectType.Poison:             name = "effect11_icon"; break;
            case EffectType.Protection:         name = "effect15_icon"; break;
            case EffectType.Root:               name = "effect5_icon"; break;
            case EffectType.Shield:             name = "effect16_icon"; break;
            case EffectType.Silent:             name = "effect10_icon"; break;
            case EffectType.Slow:               name = "effect8_icon"; break;
            case EffectType.SoulBurn:
            case EffectType.SpiritBurn:
            case EffectType.Stun:               name = "effect9_icon"; break;
            case EffectType.Transform:          name = ""; break;
            default:
                return string.Empty;
        }
        return path + name;*/
    }
}
