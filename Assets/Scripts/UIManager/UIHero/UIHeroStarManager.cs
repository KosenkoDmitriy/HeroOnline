using UnityEngine;
using System.Collections;

public class UIHeroStarManager : MonoBehaviour {

    public GameObject[] starList;

    public void SetStart(int count)
    {
        for (int i = 0; i < starList.Length; i++)
        {
            if (i < count)
            {
                starList[i].SetActive(true);
            }
            else
            {
                starList[i].SetActive(false);
            }
        }
    }
}
