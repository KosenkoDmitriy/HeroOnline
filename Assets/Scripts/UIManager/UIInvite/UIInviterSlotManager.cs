using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;
using DEngine.Unity.Photon;

public class UIInviterSlotManager : MonoBehaviour
{
    public UILabel lblName;
    private GameUser _inviter;
    private UIInviterManager _uiInviterManager;

    private PhotonController _photonController;

    public void Start()
    {
        _photonController = GameObject.FindObjectOfType<PhotonManager>().Controller;
    }

    public void SetInviter(GameUser inviter, UIInviterManager uiInviterManager)
    {
        _inviter = inviter;
        _uiInviterManager = uiInviterManager;
        lblName.text = _inviter.Base.NickName;
    }

    public void OnButtonAccept_Click()
    {
        _photonController.SendRequestAcceptBattle(_inviter);
        _uiInviterManager.RemoveInviter(_inviter);

    }


    public void OnButtonCancel_Click()
    {
        _uiInviterManager.RemoveInviter(_inviter);

        _photonController.SendRequestCancelBattle(_inviter);
    }



	
}
    