using UnityEngine;
using System.Collections;

public class UILevelUp : MonoBehaviour {

    public UISprite number1;
    public UISprite number2;
        
    public void Init(int level)
    {
        string s = level.ToString("D2");

        number1.spriteName = s[0].ToString();
        number2.spriteName = s[1].ToString();

        Destroy(gameObject, 2);
    }
}
