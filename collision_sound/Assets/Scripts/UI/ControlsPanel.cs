using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ControlsPanel : MonoBehaviour
{
    public float initialTimer = 5;
    public float animTime = 0.3f;

    private bool _show = false;
    private RectTransform _rectTransform;
    private Vector3 _showPos;
    private Vector3 _hidePos;
    private float _animTimer;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _showPos = _rectTransform.localPosition;
        _hidePos = _showPos;
        _hidePos.y -= _rectTransform.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialTimer >= 0) {
            initialTimer -= Time.deltaTime;
            _show = true;
        }
        else if (Input.GetButton("Controls")) {
            _show = true;
        }
        else _show = false;

        if (_show) {
            if (_animTimer >= animTime) _animTimer = animTime;
            else _animTimer += Time.deltaTime;
        } else {
            if (_animTimer <= 0) _animTimer = 0;
            else _animTimer -= Time.deltaTime;
        }

        float animPercent = _animTimer / animTime;
        _rectTransform.localPosition = Vector3.Lerp(_hidePos, _showPos, animPercent);
    }
}
