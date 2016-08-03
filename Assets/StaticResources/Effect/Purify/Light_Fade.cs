using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class Light_Fade : MonoBehaviour {
	
	public bool _loop = false;
	public float _duration= 1.0f;
	public float _delay = 0.0f;

	public AnimationCurve _intensityCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

	private Light _light;
	private float _intensity;
	private float _time;

	private void Start() {
		_light = GetComponent<Light>();
		_intensity = _light.intensity;
	}

	private void Update() {
		_time += Time.deltaTime / _duration;
		
		if (_time < 0.0f) return;

		GetComponent<Light>().enabled = true;

		if (_time > 1.0f) {
			_time = 0.0f;
			if (!_loop){
				enabled = false;
				GetComponent<Light>().enabled = false;
			}
		}

		GetComponent<Light>().intensity = _intensity * _intensityCurve.Evaluate(_time);
	}

	private void OnEnable() {
		_time = -_delay / _duration;
	}
}
