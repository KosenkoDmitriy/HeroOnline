using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMusicSilde : MonoBehaviour {

    public UIMusicSetting setting;
    public UIMusicSetting.SlideType type;

    public float value;
    public UIToggle toggle;
    public UIGrid root;
    public GameObject tickObject;


    private List<UIMusicTick> tickObjects;

	public void Init () 
    {
        tickObjects = new List<UIMusicTick>();
        for (int i = 0; i < 10; i++)
        {
            GameObject go = NGUITools.AddChild(root.gameObject, tickObject);
            UIMusicTick tick = go.GetComponent<UIMusicTick>();
            tick.index = i;
            tick.slider = this;
            tick.Init();
            go.SetActive(true);
            tickObjects.Add(tick);
        }
        root.Reposition();
    }

    public void SetSliderIndex(int index)
    {
        value = (float)(index + 1) / 10.0f;
        setting.OnChangeValue(type, value);

        for (int i = 0; i < 10; i++)
        {
            if (i > index)
            {
                tickObjects[i].sprite.alpha = 0.3f;
            }
            else
            {
                tickObjects[i].sprite.alpha = 1;
            }
        }
        root.Reposition();

    }

    public void OnToggle()
    {
        if (toggle.value == false)
        {
            SetSliderIndex(-1);
        }
    }

    public void SetVolumn(float value)
    {
        if (value == 0)
        {
            toggle.value = false;
            SetSliderIndex(-1);
        }
        else
        {
            toggle.value = true;
            SetSliderIndex((int)(value * 10) - 1);
        }
    }

}
