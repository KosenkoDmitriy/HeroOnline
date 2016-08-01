using UnityEngine;
using System.Collections;

public class UIAnimationTexture : MonoBehaviour {

    public Texture2D[] textures;
    public float fps = 5;
    public float delay = 1;

    private UITexture _texture;
    private float _waitTime;
    private int _curIndexFrame;
    private int _frameCount;

	void Start () {
        _waitTime = 1 / (fps);
        _curIndexFrame = 0;
        _frameCount = textures.Length;
        _texture = GetComponent<UITexture>();
        StartCoroutine(UpdateTexture());
	}
	

	private IEnumerator UpdateTexture () {

        yield return new WaitForSeconds(delay);

        while (true)//_curIndexFrame < _frameCount)
        {
            //_texture.mainTexture = textures[_curIndexFrame];
            _texture.material.mainTextureOffset = new Vector2(_curIndexFrame, _curIndexFrame);
            _curIndexFrame++;
            if (_curIndexFrame >= _frameCount) _curIndexFrame = 0;
            yield return new WaitForSeconds(_waitTime);
        }

	}
}
