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
    private Vector3 _startPos;
    // Start is called before the first frame update
    void Start()
    {
        _cameraHolder = this.gameObject.transform;
    }

    private void Update()
    {
        if (!_enabled) return;
        CheckMotion();
        ResetPosition();
        _camera.LookAt(FocusTarget());
    }

    private void CheckMotion()
    {
        float speed = new Vector3(_playerMovement.GetComponent<Rigidbody>().velocity.x, 0,
            _playerMovement.GetComponent<Rigidbody>().velocity.y).magnitude;

        if (speed < _toggleSpeed) return;
        if (_playerMovement.sprinting)
            _frequency = Mathf.Clamp(_frequency * _playerMovement.runMulti, _frequency, _frequency * _playerMovement.runMulti);
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
        pos.x += Mathf.Sin(Time.time * _frequency / 2) * _amplitude * 2;
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
