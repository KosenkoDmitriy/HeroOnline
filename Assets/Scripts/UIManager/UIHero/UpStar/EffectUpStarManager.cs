using UnityEngine;
using System.Collections;

public class EffectUpStarManager : MonoBehaviour {

    public enum State
    {
        Idle,
        PlayEffect,
        EndEffect,
        Finished,
    }

    public enum Dir
    {
        None,
        Up,
        Left,
        Right,
        Down
    };

    public enum LineState
    {
        None,
        Start,
        Mid,
        End,
        Finished
    };

    [System.Serializable]
    public class EffectLine
    {
        public int index;
        public float timer;
        public float speed;
        public float startTimer;
        public float delay;
        public LineState state;
        public Transform start;
        public Transform mid;
        public Transform end;
        public Transform particle;

        public int dimensionXStart;
        public int dimensionXMid;
        public int dimensionXEnd;
    }

    public float speedMin = 200;
    public float speedMax = 360;
    public EffectLine[] lines;
    public float resourceCount = 5;
    public UIUpStarManager manager;

    public GameObject particleOutLineRoot;
    public GameObject particleOutLine;

    public GameObject particleCardRoot;
    public GameObject particleCard;

    public GameObject particleStarRoot;
    public GameObject particleStar;

    public State state;
    private int _countFinished;

	void Start () {
        state = State.Idle;
        Initlines();
	}

    void Initlines()
    {
        foreach (EffectLine line in lines)
        {
            line.start.localScale = new Vector3(0, 1, 1);
            if (line.mid != null)
                line.mid.localScale = new Vector3(0, 1, 1);
            line.end.localScale = new Vector3(0, 1, 1);
            line.state = LineState.Start;
            line.particle.localPosition = line.start.localPosition;
            line.particle.gameObject.SetActive(false);
            line.timer = Time.time;
            line.speed = Random.Range(speedMin, speedMax);
            line.startTimer = Time.time;
        }
    }
	
	void Update () {
        switch (state)
        {
            case State.PlayEffect:
                UpdateLines();
                break;
            case State.EndEffect:
                {
                    GameObject go = GameObject.Instantiate(particleCard) as GameObject;
                    go.transform.parent = particleCardRoot.transform;
                    go.transform.localScale = new Vector3(1, 1, 1);
                    go.transform.localPosition = new Vector3(0, 0, 0);
                    Destroy(go, 4);
                    Invoke("ShowEffectStar", 1.0f);
                    state = State.Finished;
                }
                break;
        }
	}

    public void OnButtonEvolve_Click()
    {
        state = State.PlayEffect;
        _countFinished = 0;
        Initlines();
    }

    #region private methods
    private void ShowEffectStar()
    {
        float StartX = -350;
        StartX += 100 * (manager._curRoleUpgrade.Base.Grade);
        Vector3 pos = particleStarRoot.transform.localPosition;
        pos.x=StartX;
        particleStarRoot.transform.localPosition = pos;

        GameObject go = GameObject.Instantiate(particleStar) as GameObject;
        go.transform.parent = particleStarRoot.transform;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.localPosition = new Vector3(0, 0, 0);
        Destroy(go, 2);

        Invoke("OnFinishedEffect", 0.8f);
    }
    private void OnFinishedEffect()
    {       
        manager.OnFinishedEffect();
    }
    private void UpdateLines()
    {
        foreach (EffectLine line in lines)
        {
            if (Time.time - line.startTimer < line.delay)
            {
                line.timer = Time.time;
                continue;
            }

            if (line.index <= resourceCount)
            {
                if (!line.particle.gameObject.activeInHierarchy && line.state != LineState.Finished)
                    line.particle.gameObject.SetActive(true);
                switch (line.state)
                {
                    case LineState.Start:
                        {
                            if (!ScaleLine(line, line.start, Dir.Up, line.dimensionXStart))//line.start, line.dimensionXStart, Dir.Up, line.particle))
                            {
                                line.state = LineState.Mid;
                                line.timer = Time.time;
                            }
                            break;
                        }
                    case LineState.Mid:
                        {
                            if (line.mid != null)
                            {
                                Dir dir = Dir.Right;
                                if (line.index == 3 || line.index == 5) dir = Dir.Left;
                                if (!ScaleLine(line, line.mid, dir, line.dimensionXMid))//line.mid, line.dimensionXMid, dir, line.particle))
                                {
                                    line.state = LineState.End;
                                    line.timer = Time.time;
                                }
                            }
                            else
                                line.state = LineState.End;
                            break;
                        }
                    case LineState.End:
                        {
                            if (!ScaleLine(line, line.end, Dir.Up, line.dimensionXEnd))//line.end, line.dimensionXEnd, Dir.Up, line.particle))
                            {
                                GameObject go = GameObject.Instantiate(particleOutLine) as GameObject;
                                go.transform.parent = particleOutLineRoot.transform;
                                go.transform.localScale = new Vector3(1, 1, 1);
                                go.transform.localPosition = new Vector3(0, 0, 0);
                                Destroy(go, 2);
                                Debug.Log("End " + line.index);
                                line.timer = Time.time;
                                line.particle.gameObject.SetActive(false);
                                line.state = LineState.Finished;
                                _countFinished++;
                                if (_countFinished == resourceCount)
                                    state = State.EndEffect;
                            }
                            break;
                        }
                }
            }
        }
    }
    private bool ScaleLine(EffectLine line, Transform curLine, Dir dirOfParticle, float length)
    {
        float distCovered = (Time.time - line.timer) * line.speed;
        float dt = distCovered / length;

        float scaleValue = Mathf.Lerp(0, 1, dt);
        scaleValue = Mathf.Min(scaleValue, 1);

        curLine.localScale = new Vector3(scaleValue, 1, 1);

        Vector3 pos = curLine.transform.localPosition;
        if (dirOfParticle == Dir.Up)
        {
            pos.y = curLine.localPosition.y + scaleValue * length;
        }
        if (dirOfParticle == Dir.Left)
        {
            pos.x = curLine.localPosition.x - scaleValue * length;
        }
        if (dirOfParticle == Dir.Right)
        {
            pos.x = curLine.localPosition.x + scaleValue * length;
        }

        line.particle.transform.localPosition = pos;

        if (scaleValue == 1) return false;
        return true;
    }
    #endregion
}
