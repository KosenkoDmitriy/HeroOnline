using UnityEngine;
using System.Collections;

public class UIMusicTick : MonoBehaviour {

    public int index = 0;
    public UIMusicSilde slider;
    public UISprite sprite;

    public void Init()
    {
        sprite = GetComponent<UISprite>();
    }

    public void OnClick()
    {
        if (slider.toggle.value == false) return;
        slider.SetSliderIndex(index);
    }


    public void OnHover(bool press)
    {
        if (slider.toggle.value == false) return;
        if (press)
            slider.SetSliderIndex(index);
    }
}
