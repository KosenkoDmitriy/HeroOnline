using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DEngine.Common.GameLogic;
using System.Linq;
using DEngine.Unity.Photon;

public class UIInviterManager : MonoBehaviour {

    public GameObject InviterRoot;
    public GameObject InviterPrefab;
    public UIButton btnIniteInfo;

    public static UIInviterManager Instance { get { return _instance; } }
    private static UIInviterManager _instance;


    private static Dictionary<int, GameUser> _inviterSet = new Dictionary<int, GameUser>();
    private Dictionary<int, UIInviterSlotManager> _inviterSlots;


	void Start () {
        _instance = this;
        _inviterSlots = new Dictionary<int, UIInviterSlotManager>();

        ReDraw();
	}

    private void ReDraw()
    {
        for (int i = 0; i < _inviterSet.Count; i++)
        {
            AddGameObject(_inviterSet.Values.ToList<GameUser>()[i], i);
        }
    }


    public void AddInviter(GameUser inviter)
    {
        if (_inviterSet.ContainsKey(inviter.Id)) return;

        _inviterSet.Add(inviter.Id, inviter);

        if (InviterRoot != null)
        {
            AddGameObject(inviter, _inviterSlots.Count);
            Invoke("StopScale", 2);
        }
       
    }

    public void AddGameObject(GameUser inviter,int index)
    {
        GameObject go = NGUITools.AddChild(InviterRoot, InviterPrefab);
        go.transform.localPosition = new Vector3(0, -52 * index, 0);
        UIInviterSlotManager slot = go.GetComponent<UIInviterSlotManager>();
        slot.SetInviter(inviter, this);
        _inviterSlots.Add(inviter.Id, slot);

        btnIniteInfo.GetComponent<TweenScale>().enabled = true;
    }

    private void StopScale()
    {
        btnIniteInfo.GetComponent<TweenScale>().enabled = false;
        btnIniteInfo.transform.localScale = new Vector3(1, 1, 1);
    }

    public void RemoveInviter(GameUser inviter)
    {
        _inviterSet.Remove(inviter.Id);

        if (_inviterSlots.ContainsKey(inviter.Id))
        {
            GameObject.Destroy(_inviterSlots[inviter.Id].gameObject);
            _inviterSlots.Remove(inviter.Id);

            RefreshSlots();
        }
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < _inviterSlots.Count; i++)
        {
            _inviterSlots.Values.ToList<UIInviterSlotManager>()[i].transform.localPosition = new Vector3(0, -52 * i, 0);
        }
    }

}
