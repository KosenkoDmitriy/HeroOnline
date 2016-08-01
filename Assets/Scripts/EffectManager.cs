using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using System.Collections.Generic;
using System.Linq;

public class EffectManager {

    private Queue<UserRole.MagicState>                      _magicAttribs;
    private Controller                                      _controller;
    private EffectController                                _effectController;
    private Vector2                                         _offset = new Vector2(45,- 45);
    private GameObject                                      _uiEffectPrefab;
    private Dictionary<EffectType,UIEffectController>       _uiEffectSet;

    public EffectManager(Controller controller)
    {
        _controller = controller;
        _magicAttribs = new Queue<UserRole.MagicState>();
        _controller.StartCoroutine(CheckShowEffect());

        _uiEffectPrefab = Resources.Load<GameObject>("Prefabs/UI/Battle/UIEffectPrefab");
        _uiEffectSet = new Dictionary<EffectType,UIEffectController>();
    }

    private IEnumerator CheckShowEffect()
    {
        while (true)
        {
            if ((_effectController == null || _effectController.curState != EffectController.EffectState.Idle) && _magicAttribs.Count > 0)
            {
                UserRole.MagicState magicAttrib = _magicAttribs.Dequeue();

                string path = GetPathEffect(magicAttrib.EffectType);
                if (path != "")
                {
                    GameObject go = GameObject.Instantiate(Resources.Load(path) as GameObject) as GameObject;

                    _effectController = go.GetComponent<EffectController>();
                    if (_controller.uiSlot != null)
                        _effectController.EndPos = _controller.uiSlot.worldPos;
                    go.transform.parent = _controller.transform;
                    go.transform.localPosition = _controller.offsetEffect;
                    go.transform.localScale = new Vector3(1, 1, 1);

                    go.transform.rotation = _controller.transform.rotation;
                }


            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void AddEffect(UserRole.MagicState magicAttrib)
    {

        _magicAttribs.Enqueue(magicAttrib);

        if (_controller.uiSlot == null) return;
        //Debug.Log("AddEffect " + magicAttrib.EffectType);

        if (_uiEffectSet.ContainsKey(magicAttrib.EffectType))
        {
            //Debug.Log("_uiEffectSet.ContainsKey " + magicAttrib.EffectType);
            _uiEffectSet[magicAttrib.EffectType].AddDuration(magicAttrib.RemainTime);
        }
        else
        {
            //Debug.Log("effectRoot " + _controller.uiSlot.effectRoot);
            //Debug.Log("_uiEffectPrefab " + _uiEffectPrefab);
            GameObject go = NGUITools.AddChild(_controller.uiSlot.effectRoot, _uiEffectPrefab);
            UIEffectController uiEffect = go.GetComponent<UIEffectController>();
            uiEffect.CreateEffect(this, magicAttrib);
            go.transform.localPosition = new Vector3((_uiEffectSet.Count % 3) * _offset.x,
                                                        (_uiEffectSet.Count / 3) * _offset.y,
                                                        0);

            _uiEffectSet.Add(magicAttrib.EffectType, uiEffect);
        }
    }

    public void RemoveEffect(EffectType type)
    {
        UIEffectController uiEffect = _uiEffectSet[type];
        if (uiEffect != null)
        {
            GameObject.Destroy(uiEffect.gameObject);
            _uiEffectSet.Remove(type);
        }
        //Debug.Log("RemoveEffect");
        _controller.RemoveEffect(type);
        RefreshEffectSet();
    }

    private void RefreshEffectSet()
    {
        for (int i = 0; i < _uiEffectSet.Values.Count; i++)
        {
            _uiEffectSet.Values.ToList<UIEffectController>()[i].transform.localPosition = new Vector3((i % 3) * _offset.x,
                                                        (i / 3) * _offset.y,
                                                        0);
        }
    }

    private string GetPathEffect(EffectType type)
    {
        string path = "Effect/Prefabs/";
        string name = "";
       /* switch (type)
        {
            case EffectType.Barrier:
            case EffectType.BattleMage:         name = "MagicAttackUp"; break;
            case EffectType.Bind: 
            case EffectType.Bleed: 
            case EffectType.Blind:              name = "Blind"; break;
            case EffectType.Break:
            case EffectType.Burn:
            case EffectType.Cleansing:
            case EffectType.Crack:
            case EffectType.Cripple:
            case EffectType.Disable:            name = "Silence"; break;
            case EffectType.Disorder:
            case EffectType.Freeze:             name = "Freeze"; break;
            case EffectType.Frenzy:
            case EffectType.Haste:              name = "PhysicAttackUp"; break;
            case EffectType.Invincible:
            case EffectType.Knockback:
            case EffectType.Lame:
            case EffectType.ManaBurn:
            case EffectType.Poison:             name = "Poison"; break;
            case EffectType.Protection:         name = "PDefenceUp"; break;
            case EffectType.Root:               name = "Root"; break;
            case EffectType.Shield:             name = "MDefenceUp"; break;
            case EffectType.Silent:             name = "Silence"; break;
            case EffectType.Slow:               name = "SlowMovementSpeed"; break;
            case EffectType.SoulBurn:
            case EffectType.SpiritBurn:
            case EffectType.Stun:               name = "Stun"; break;
            case EffectType.Transform:          name = "Effect01"; break;
            default:
                return string.Empty;
        }*/
        return "";// path + name;
    }
}
