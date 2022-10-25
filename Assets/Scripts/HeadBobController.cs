using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool _enabled;

    [SerializeField] private float _amplitude;
    [SerializeField] private float _frequency;

    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private PlayerMovement _playerMovement;
    private float _toggleSpeed = 3.0f;
    [SerializeField] private float lerpSpeed;
    private Vector3 _startPos;

    private float _maxFreqVal;
    private float _maxAmpVal;
    private float _defFreqVal;
    private bool _shouldChangeFreqAmp = true;
    private float _defAmpVal;
    // Start is called before the first frame update
    void Start()
    {
        _cameraHolder = this.gameObject.transform;
        _defFreqVal = _frequency;
        _defAmpVal = _amplitude;
        _maxFreqVal = _frequency * (_frequency + (_playerMovement.runMulti / 2));
        _maxAmpVal = _amplitude + (_amplitude * (_playerMovement.runMulti / 2));
    }

    private void Update()
    {
        if (!_enabled) return;
        CheckMotion();
        ResetPosition();
        Debug.Log(CheckSpeed());
        _camera.LookAt(FocusTarget());
    }

    float CheckSpeed()
    {
        return new Vector3(_playerMovement.GetComponent<Rigidbody>().velocity.x, 0,
            _playerMovement.GetComponent<Rigidbody>().velocity.z).magnitude;
    }
    private void CheckMotion()
    {
        float speed = CheckSpeed();

        if (speed < _toggleSpeed) return;
        if (_playerMovement.sprinting && _shouldChangeFreqAmp)
        {
            if(_frequency != _maxFreqVal) {
                _frequency = Mathf.Lerp(_frequency, _maxFreqVal, lerpSpeed * Time.deltaTime);
                _amplitude = Mathf.Lerp(_amplitude, _maxAmpVal, lerpSpeed * Time.deltaTime);
                _shouldChangeFreqAmp = false;
            }
        }else if (!_playerMovement.sprinting && Math.Abs(_frequency - _defFreqVal) > 0.85f && !_shouldChangeFreqAmp)
        {
            _frequency = _defFreqVal;
            _amplitude = _defAmpVal;
            _shouldChangeFreqAmp = true;
        }
        if (!_playerMovement.grounded) return;

        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }
    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }
    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Sin(Time.time * _frequency / 4) * _amplitude / 4;
        return pos;
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y,
            transform.position.z);
        pos += _cameraHolder.forward * 15.0f;
        return pos;
    }
}
