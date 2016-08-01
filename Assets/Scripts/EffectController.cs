using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour {

    public enum EffectState
    {
        Idle,
        MoveTopLeft,
        End
    }

    public Vector3 EndPos = new Vector3(-3.29f, 4.76f, -6.005052f);
    public float timeWait = 2;
    public float velocity = 1;
    public EffectState curState;

    public ParticleSystem[] particles;

    private float _timer;
    private Transform _transform;
  


	void Start () {
       
        curState = EffectState.Idle;
        _timer = Time.time;
        _transform = transform;

	}
	    
	void Update () {
        switch (curState)
        {
            case EffectState.Idle:
                Idle();
                break;
            case EffectState.MoveTopLeft:
                MoveTopLeft();
                break;
            case EffectState.End:
                End();
                break;
        }
	}

    private void Idle()
    {
        if (Time.time - _timer >= timeWait)
        {
            curState = EffectState.MoveTopLeft;
            _transform.parent = null;
        }
    }

    private void MoveTopLeft()
    {
        if (Vector3.Distance(EndPos, _transform.position) > 0.01f)
        {
            _transform.position = Vector3.Lerp(_transform.position, EndPos, Time.deltaTime * velocity);
        }
        else
        {
            curState = EffectState.End;
        }
    }

    private void End()
    {
        Destroy(gameObject, 0.5f);
    }
}
