using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DEngine.Common.GameLogic;

public class UIBattleHeroSlotController : MonoBehaviour {


    public GameObject UIBattleHeroSlotPrefab;
    public GameObject UIHeroRoot;
    public GameObject information;

	void Start () {

        Invoke("Init", 0.5f);
	}
   
    void Init()
    {
        if (GameManager.battleType != BattleMode.RandomPvE)
        {
            information.SetActive(true);
        }

        int i = 0;
        int iRight = 0;
        GameObject[] roleSet = (from hero in GameplayManager.Instance.heroPlayerSet select hero.gameObject).ToArray();
        

        foreach (GameObject role in roleSet)
        {
            if (role.GetComponent<Controller>().role.Base.Type != DEngine.Common.GameLogic.RoleType.Mob && role.GetComponent<Controller>().role.Base.Type != DEngine.Common.GameLogic.RoleType.Hostage)
            {
                GameObject go = NGUITools.AddChild(UIHeroRoot, UIBattleHeroSlotPrefab);
                UIHeroSlotController slot = go.GetComponent<UIHeroSlotController>();
                slot.setRole(role.GetComponent<Controller>());
                go.transform.localPosition = new Vector3((i + iRight) * 140, 0, 0);
                //go.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                i++;
                if (i > 2)
                {
                    iRight = 2;
                }
                Vector3 worldPos = Vector3.zero;

                Vector3 screenPos = NGUITools.FindCameraForLayer(go.layer).WorldToScreenPoint(go.transform.position);

                worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y - 10, 7));

                slot.worldPos = worldPos;
            }
        }
    }

   
}
