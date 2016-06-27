using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public enum Strategy
{
    None,
    Nearest,
    MaxHP,
    MinHP,
    MaxPDefence,
    MaxMDefence,
    MinPDefence,
    MinMDefence,
    MaxPDamage,
    MaxMDamage,
    MinPDamage,
    MinMDamage,
    MaxMP,
    MinMP
}

public class UIStrategyManager : MonoBehaviour {
    
    public UILabel L_Strategy_Header;
    public UILabel LNearest;
    public UILabel LMaxHP;
    public UILabel LMinHP;
    public UILabel LMaxPDefence;
    public UILabel LMaxMDefence;
    public UILabel LMinPDefence;
    public UILabel LMinMDefence;
    public UILabel LMaxPDamage;
    public UILabel LMaxMDamage;
    public UILabel LMinPDamage;
    public UILabel LMinMDamage;
    public UILabel LMaxMP;
    public UILabel LMinMP;

    public UIToggle toggleNearest;
    public UIToggle toggleMaxHP;
    public UIToggle toggleMinHP;
    public UIToggle toggleMaxPDefence;
    public UIToggle toggleMaxMDefence;
    public UIToggle toggleMinPDefence;
    public UIToggle toggleMinMDefence;
    public UIToggle toggleMaxPDamage;
    public UIToggle toggleMaxMDamage;
    public UIToggle toggleMinPDamage;
    public UIToggle toggleMinMDamage;
    public UIToggle toggleMaxMP;
    public UIToggle toggleMinMP;

    public Strategy strategy { get; set; }

    public UserRole curRole;
    public UIHeroNewManager heroManager;

	void Start () {
        strategy = Strategy.Nearest;        
        Localization();        
	}

    void OnEnable()
    {
        Debug.Log("OnEnable");
        Invoke("PrefresCheckList", 0.2f);
    }

    public void SetRole(UserRole role,UIHeroNewManager manager)
    {
        curRole = role;
        heroManager = manager;
        if (gameObject.activeInHierarchy)
            PrefresCheckList();
    }

    private void PrefresCheckList()
    {
        toggleNearest.value = false;
        toggleMaxHP.value = false;
        toggleMinHP.value = false;
        toggleMaxPDefence.value = false;
        toggleMaxMDefence.value = false;
        toggleMinPDefence.value = false;
        toggleMinMDefence.value = false;
        toggleMaxPDamage.value = false;
        toggleMaxMDamage.value = false;
        toggleMinPDamage.value = false;
        toggleMinMDamage.value = false;
        toggleMaxMP.value = false;
        toggleMinMP.value = false;

        if (curRole == null) return;
        //Debug.Log("curRole.Base.AIMode " + curRole.Base.AIMode);
        switch ((Strategy)curRole.Base.AIMode)
        {
            case Strategy.None:
            case Strategy.Nearest:
                toggleNearest.value = true;
                break;
            case Strategy.MaxHP:
                toggleMaxHP.value = true;
                break;
            case Strategy.MinHP:
                toggleMinHP.value = true;
                break;
            case Strategy.MaxPDefence:
                toggleMaxPDefence.value = true;
                break;
            case Strategy.MaxMDefence:
                toggleMaxMDefence.value = true;
                break;
            case Strategy.MinPDefence:
                toggleMinPDefence.value = true;
                break;
            case Strategy.MinMDefence:
                toggleMinMDefence.value = true;
                break;
            case Strategy.MaxPDamage:
                toggleMaxPDamage.value = true;
                break;
            case Strategy.MaxMDamage:
                toggleMaxMDamage.value = true;
                break;
            case Strategy.MinPDamage:
                toggleMinPDamage.value = true;
                break;
            case Strategy.MinMDamage:
                toggleMinMDamage.value = true;
                break;
            case Strategy.MaxMP:
                toggleMaxMP.value = true;
                break;
            case Strategy.MinMP:
                toggleMinMP.value = true;
                break;
        }
    }

    public void OnStrategyChanged()
    {
        curRole.Base.AIMode = (int) strategy;
        heroManager.OnSaveStrategy(curRole);
    }

    #region toggle
    public void OnChkNearest_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.Nearest;
            OnStrategyChanged();
        }
    }    
    public void OnChkMaxHP_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MaxHP;
            OnStrategyChanged();
        }
    }
    public void OnChkMinHP_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MinHP;
            OnStrategyChanged();
        }
    }
    public void OnChkMaxPDef_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MaxPDefence;
            OnStrategyChanged();
        }
    }
    public void OnChkMaxMDef_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MaxMDefence;
            OnStrategyChanged();
        }
    }
    public void OnChkMinPDef_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MinPDefence;
            OnStrategyChanged();
        }
    }
    public void OnChkMinMDef_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MinMDefence;
            OnStrategyChanged();
        }
    }
    public void OnChkMaxPDamage_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MaxPDamage;
            OnStrategyChanged();
        }
    }
    public void OnChkMaxMDamage_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MaxMDamage;
            OnStrategyChanged();
        }
    }
    public void OnChkMinPDamage_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MinPDamage;
            OnStrategyChanged();
        }
    }
    public void OnChkMinMDamage_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MinMDamage;
            OnStrategyChanged();
        }
    }
    public void OnChkMaxMP_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MaxMP;
            OnStrategyChanged();
        }
    }
    public void OnChkMinMP_toggle()
    {
        if (UIToggle.current.value)
        {
            strategy = Strategy.MinMP;
            OnStrategyChanged();
        }
    }
    #endregion

    private void Localization()
    {
        L_Strategy_Header.text = GameManager.localization.GetText("L_Strategy_Header");
        LNearest.text = GameManager.localization.GetText("Hero_Strategy_Nearest");
        LMaxHP.text = GameManager.localization.GetText("Hero_Strategy_MaxHP");
        LMinHP.text = GameManager.localization.GetText("Hero_Strategy_MinHP");
        LMaxPDefence.text = GameManager.localization.GetText("Hero_Strategy_MaxPDefence");
        LMaxMDefence.text = GameManager.localization.GetText("Hero_Strategy_MaxMDefence");
        LMinPDefence.text = GameManager.localization.GetText("Hero_Strategy_MinPDefence");
        LMinMDefence.text = GameManager.localization.GetText("Hero_Strategy_MinMDefence");
        LMaxPDamage.text = GameManager.localization.GetText("Hero_Strategy_MaxPDamage");
        LMaxMDamage.text = GameManager.localization.GetText("Hero_Strategy_MaxMDamage");
        LMinPDamage.text = GameManager.localization.GetText("Hero_Strategy_MinPDamage");
        LMinMDamage.text = GameManager.localization.GetText("Hero_Strategy_MinMDamage");
        LMaxMP.text = GameManager.localization.GetText("Hero_Strategy_MaxMP");
        LMinMP.text = GameManager.localization.GetText("Hero_Strategy_MinMP");

    }
}
