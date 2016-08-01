using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
public class UIHireHeroManager : MonoBehaviour {

    [System.Serializable]
    public struct HireList
    {
       public UIGrid root;
       public GameObject prefab;
    }
    [System.Serializable]
    public struct UIHireHero
    {
        public UILabel lblHeader;
        public UILabel lblAcceptToHire;
        public UILabel lblButtonHire;
        public UILabel txtGold;
        public UILabel txtSilver;
        public UIToggle togAcceptToHire;
    }

    public HireList hireList;
    public UIHireHeroSlot selectedSlot;
    public UIHireHero uiHireHero;
    public GameObject root;

    public delegate void HandleStart();
    public event HandleStart OnStartClick;

    private List<UIHireHeroSlot> listHireSlot=new List<UIHireHeroSlot>();

    void Start()
    {
        uiHireHero.lblHeader.text = GameManager.localization.GetText("HireHero_Header");
        uiHireHero.lblButtonHire.text = GameManager.localization.GetText("HireHero_ButtonStart");
    }

    public void Show (UserRoleHire[] userRolesHire) 
    {
        root.SetActive(true);
        if (userRolesHire != null)
        {
            foreach (UserRoleHire role in userRolesHire)
            {
                GameObject go = NGUITools.AddChild(hireList.root.gameObject, hireList.prefab);
                UIHireHeroSlot slot = go.GetComponent<UIHireHeroSlot>();
                slot.InitUser(role, this);
                listHireSlot.Add(slot);
            }
            hireList.root.Reposition();
        }
	}

    public void Hide()
    {
        root.SetActive(false);
        foreach (UIHireHeroSlot slot in listHireSlot)
        {
            Destroy(slot.gameObject);
        }
        listHireSlot.Clear();
    }

    public void OnSelectedSlot(UIHireHeroSlot _slot)
    {
        if (selectedSlot != null)
            selectedSlot.OnDeSlected();

        selectedSlot = _slot;
        if (selectedSlot != null)
        {
            uiHireHero.txtGold.text = selectedSlot.userRoleHire.HireGold.ToString();
            uiHireHero.txtSilver.text = selectedSlot.userRoleHire.HireSilver.ToString();
            uiHireHero.togAcceptToHire.value = true;
        }
    }
	
    public void OnStart_Click()
    {
        if (uiHireHero.togAcceptToHire.value)
        {
            if (selectedSlot != null)
            {
                int gold = selectedSlot.userRoleHire.HireGold;
                int silver = selectedSlot.userRoleHire.HireSilver;

                if (GameManager.GameUser.Base.Gold < gold || GameManager.GameUser.Base.Silver < silver)
                {
                    uiHireHero.togAcceptToHire.value = false;
                    Helper.HandleCashInsufficient();
                    return;
                }

            }
        }

        if (OnStartClick != null)
            OnStartClick.Invoke();
    }

    public void OnButtonX_Click()
    {
        Hide();
    }
}
