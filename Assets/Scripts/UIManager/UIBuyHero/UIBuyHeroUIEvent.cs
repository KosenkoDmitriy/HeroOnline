using UnityEngine;
using System.Collections;

public class UIBuyHeroUIEvent : MonoBehaviour {

    public enum Type
    {
        None,
        Silver,
        Gold,
        Rune
    }

    public UIBuyHeroManager manager;
    public Type type;

    //Drag event this gameoject,event Unity
    void OnDragStart()
    {
        if (type == Type.Gold)
            manager.OnDragGold();
        else if (type == Type.Silver)
            manager.OnDragSilver();
    }

    
    void OnDrop()
    {
        if (type == Type.Rune)
        {
            manager.OnDropRun();
        }
        else
        {
            manager.OnDragRelease();
        }
    }

    //Click event this gameoject , event Unity
    void OnClick()
    {
        OnDrop();
    }
}
