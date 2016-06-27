using UnityEngine;
using System.Collections;

public class UIChestDrop : MonoBehaviour {

    public enum State
    {
        Idle,
        MoveToHome,
        Destroy,
        End
    }
    public UIChestManager chestManager;
    public Transform home;
    public float idleTimer = 1;
    public float speed = 8;

    private State _state;
    private float _timer;
    private float _length;
    private Transform _transform;

	void Start () {
        _state = State.Idle;
        _transform = transform;
        Invoke("Move", idleTimer);
	}

    private void Move()
    {
        _state = State.MoveToHome;
        _timer = Time.time;
        _length = Vector3.Distance(home.localPosition, _transform.localPosition);
    }
	

	void Update () {
        switch (_state)
        {
            case State.MoveToHome:
                float time = Time.time - _timer;
                float dt = time / _length * speed;
                _transform.localPosition = Vector3.Lerp(_transform.localPosition, home.localPosition, dt);
                if (Vector3.Distance(home.localPosition, _transform.localPosition) <= 30)
                {
                    _state = State.Destroy;
                }
                break;
            case State.Destroy:
                Destroy(gameObject);
                chestManager.OnDestroyChest();
                _state = State.End;
                break;
        }
	}
}
