using UnityEngine;
using System.Collections;

public class UIBattleBonus : MonoBehaviour {

    public UIBattleResult result;
    public UILabel lblHeader;
    public UILabel lblFooter;
    public UILabel lblRewardHeader;

    public UIInformationItemManager itemBonus;
    public GameObject informationReward;
    public UILabel lblreward;

    public GameObject resultChest;
    public GameObject particleOpen;
    //private TweenPosition _tweenPos;
   // public UIDungeonManager dungeon;
    public void Start()
    {      
        //_tweenPos = resultChest.GetComponent<TweenPosition>();
        Localization();
    }

    public void Init(string s)
    {
        GetComponent<TweenScale>().PlayForward();
        lblHeader.text = s;
    }

    public void ButtonTreasure1_Click()
    {
      //  _tweenPos.from = new Vector3(-230, 0, 0);
      //  resultChest.SetActive(true);
    }

    public void ButtonTreasure2_Click()
    {
       // _tweenPos.from = new Vector3(0, 0, 0);
      //  resultChest.SetActive(true);
    }

    public void ButtonTreasure3_Click()
    {
      //  _tweenPos.from = new Vector3(230, 0, 0);
      //  resultChest.SetActive(true);
    }
  

    public void OpenChest()
    {
        result._reward = true;
        resultChest.GetComponent<UISpriteAnimation>().enabled = true;
        particleOpen.SetActive(true);

        //Battle GetReward
        Invoke("ShowInfo", 1.5f);
    }

    private void ShowInfo()
    {
        //if (GameManager.rewardEx.ItemId != 0)
        //{
        //    itemBonus.gameObject.SetActive(true);
        //    informationReward.SetActive(true);
        //    lblreward.text = "Nhận được item " + GameManager.rewardEx.ItemId;
        //}
        //else
        //{
        //informationReward.SetActive(true);
        //if (GameManager.rewardEx.Gold > 0)
        //    lblreward.text = string.Format(GameManager.localization.GetText("Reward_Result_Gold"), GameManager.rewardEx.Gold);
        //else if (GameManager.rewardEx.Silver > 0)
        //    lblreward.text = string.Format(GameManager.localization.GetText("Reward_Result_Silver"), GameManager.rewardEx.Silver);
        //else if (GameManager.rewardEx.UserExp > 0)
        //{
         //   lblreward.text = string.Format(GameManager.localization.GetText("Reward_Result_EXPUser"), GameManager.rewardEx.UserExp);

            /* if (GameManager.rewardEx.RoleExp > 0)
             {
                 lblreward.text += "\n" + string.Format(GameManager.localization.GetText("Reward_Result_EXPRole"), GameManager.rewardEx.RoleExp);
             }*/
        //}
        //}
        particleOpen.SetActive(false);
    }

    private void Localization()
    {
       // lblHeader.text = GameManager.localization.GetText("Reward_Header");
        lblFooter.text = GameManager.localization.GetText("Dungeon_Loop_Footer");
       // lblRewardHeader.text = GameManager.localization.GetText("Reward_Result_Header");
    }

    public void OnButtonCancel_Click()
    {
        UIPlayTween tween = new UIPlayTween();
        tween.playDirection = AnimationOrTween.Direction.Reverse;
        tween.tweenTarget = gameObject;
        tween.Play(true);

        //result.OnReciveReward();
        //dungeon.OnResume();
    }
}
