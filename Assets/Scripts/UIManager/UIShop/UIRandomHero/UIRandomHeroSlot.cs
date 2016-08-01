using UnityEngine;
using System.Collections;

public class UIRandomHeroSlot : MonoBehaviour {

    public UITexture background;
    public UITexture iconHero;
    public UITexture Border;

    public int Index = 0;

    public void SetSlot(int id, int quality,int _index)
    {
        iconHero.mainTexture = Helper.LoadTextureForHero(id);
        background.mainTexture = Helper.LoadTextureElement(quality);
        Border.gameObject.SetActive(false);
        Index = _index;
    }

    public void OnSelect()
    {
        Border.gameObject.SetActive(true);
    }

    public void OnDeSelect()
    {
        Border.gameObject.SetActive(false);
    }
}
