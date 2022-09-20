using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    public float throwForce, throwExtraForce, rotationForce;
    public Collider[] gfxColliders;
    public GameObject[] weaponGfxs;
    public string itemDescription;
    public int weaponGfxLayer;
    private bool _scoping;
    private bool _held;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 0.1f;
        
    }

    public void PickUp(Transform weaponHolder)
    {
        if (_held) return;
        Debug.Log("Pickup :))");
        Destroy(_rb);
        transform.parent = weaponHolder;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        foreach (var collider in gfxColliders)
        {
            collider.enabled = false;
        }

        foreach (var gfx in weaponGfxs)
        {
            gfx.layer = weaponGfxLayer;
        }
        _held = true;
    }

    public void Drop(Transform orientation, float throwHeld)
    {
        if (!_held) return;
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.mass = 0.1f;
        var forward = orientation.forward;
        forward.y = 0f;
        _rb.velocity = forward * (throwForce * throwHeld);
        _rb.velocity += Vector3.up * (throwExtraForce * throwHeld);
        _rb.angularVelocity = Random.onUnitSphere * rotationForce;
        transform.parent = null;
        foreach (var collider in gfxColliders)
        {
            collider.enabled = true;
        }

        foreach (var gfx in weaponGfxs)
        {
            gfx.layer = 0;
        }
        _held = false;
    }

    public bool Scoping => _scoping;
}
