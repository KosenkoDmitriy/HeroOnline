using UnityEngine;
using System.Collections;

public class UIAvatarController : MonoBehaviour {

    public UITexture icon;
    private int _avatarID;
    private MonoBehaviour _manager;

    public void setAvatar(int id, MonoBehaviour manager)
    {
        _avatarID = id;
        _manager = manager;
        icon.mainTexture = Helper.LoadTextureForAvatar(id);
    }

    public void OnChoseAvatar()
    {
        if (GameManager.GameUser.Base.Gold <= 0)
        {
            MessageBox.ShowDialog(GameManager.localization.GetText("Shop_NotEnoughtGold"), UINoticeManager.NoticeType.Message);
            return;
        }

        UINoticeManager.OnButtonOK_click += new UINoticeManager.NoticeHandle(OnAgreeChangeAvatar);
        MessageBox.ShowDialog(GameManager.localization.GetText("WorldMap_ChangeAvatar"), UINoticeManager.NoticeType.YesNo);

    }

    private void OnAgreeChangeAvatar()
    {
        if (_manager as UISocialManager)
            ((UISocialManager)_manager).OnSelectAvatar(_avatarID);


        if (_manager as UIWorldMapManager)
            ((UIWorldMapManager)_manager).OnSelectAvatar(_avatarID);
    }

}
