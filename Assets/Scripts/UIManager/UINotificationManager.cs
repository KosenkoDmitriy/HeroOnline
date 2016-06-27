using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UINotificationManager : MonoBehaviour {

    public enum State
    {
        None,
        ScaleOut,
        Show,
        ScaleIn,
        Hide
    }

    public GameObject root;
    public UILabel lblText;
    public float speedOut = 5;
    public float speedHide = 5;
    public float timeShow = 2.5f;
    public static UINotificationManager Instance;

    private Queue<string> Contains;
    private State _state;
    private float _timer;

    public void Awake()
    {
        Contains = new Queue<string>();
        _state = State.None;    
        _timer = Time.time;
    }

    public void AddText(string s, bool System = false)
    {
        if (Contains == null)
            Awake();

        Contains.Enqueue(s);
    }

    void Update()
    {
        switch (_state)
        {
            case State.None:
                SetText();
                break;
            case State.ScaleOut:
                ScaleOut();
                break;
            case State.Show:
                Show();
                break;
            case State.ScaleIn:
                ScaleIn();
                break;
        }
    }

    private void ScaleIn()
    {
        if (root.transform.localScale.y <= 0)
        {
            root.transform.localScale = new Vector3(1, 0, 1);
            _state = State.None;
            return;
        }
        float curSpeed = CalculatorFadeSpeed(speedHide);
        float scale = root.transform.localScale.y;
        scale -= Time.deltaTime * curSpeed;
        root.transform.localScale = new Vector3(1, scale, 1);
    }

    private void Show()
    {
        if (Time.time - _timer > timeShow)
        {
            _state = State.ScaleIn;
        }
    }

    private void ScaleOut()
    {
        if (root.transform.localScale.y >= 1)
        {
            root.transform.localScale = new Vector3(1, 1, 1);
            _state = State.Show;
            _timer = Time.time;
            return;
        }

        float curSpeed = CalculatorFadeSpeed(speedOut);

        float scale = root.transform.localScale.y;
        scale += Time.deltaTime * curSpeed;
        root.transform.localScale = new Vector3(1, scale, 1);           
    }

    private float CalculatorFadeSpeed(float speed)
    {
        float result = 0;

        float count = Mathf.Max(1, Contains.Count);
        float maxCount = 10;
        float percent = count / maxCount;
        percent = Mathf.Min(percent, 2);
        result = speed + speed * percent;   

        return result;
    }

    private void SetText()
    {
        if (Contains.Count <= 0) return;

        string s = Contains.Dequeue();

        lblText.text = s;

        root.SetActive(true);

        _state = State.ScaleOut;
    }
}
