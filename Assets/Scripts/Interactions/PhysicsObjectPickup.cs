using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PhysicsObjectPickup : MonoBehaviour
{
    public Transform playerCamera;
    public float throwForce; // Force at which the object is thrown at
    public float rotationSmoothing;
    public float holdDistance;
    public float moveDeadzone; // Allowed distance between held object and object holding area before moving the held object
    public float interactRange; // Allowed distance between object and player for interaction / pickups
    public float moveStrength; // Object move speed?
    public bool isHeldObjectAWeapon;
    public Transform objectHoldArea;
    public Transform objectInteractPosition;
    public LayerMask objectInteractLayers;
    public LayerMask playerLayer;
    public Transform playerOrientation;
    public Transform objectParent;
    private GameObject _heldObj;
    private Collider _targetCollider;
    private Rigidbody _targetRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!_heldObj)
            {
                Debug.Log("Pick up key pressed, no held objects");
                if (LookingAtInteractable() != null)
                {
                    var hit = LookingAtInteractable().gameObject;
                    if (hit.CompareTag("NonInteractable")) return;
                    Debug.Log("Raycast Hit an object on the physics object layer");
                    Debug.Log(hit.name);
                    Pickup(hit);
                }
                else
                {
                    Debug.Log("Not looking at a gameObject");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (_heldObj)
            {
                Debug.Log("Drop key pressed, object is held");
                if(_targetRb)
                    Drop(throwForce);
            }
        }

        if (_heldObj)
        {
            _heldObj.transform.rotation = Quaternion.RotateTowards(_heldObj.transform.rotation,
                Quaternion.LookRotation(-playerCamera.position),
                rotationSmoothing * Time.deltaTime);
            
            // Checking if there is a held object and that the distance between the object and the hold area is greater than the allowed distance
            if (Vector3.Distance(_heldObj.transform.position, objectInteractPosition.position) > moveDeadzone)
            {
                Vector3 moveDirection = (objectParent.position - _heldObj.transform.position);
                if (_heldObj.TryGetComponent(out Rigidbody _heldRB))
                {
                    _heldRB.AddForce(moveDirection * moveStrength / _heldRB.mass, ForceMode.Acceleration);
                }
            }
        }
    }

    private void FixedUpdate()
    {

        if (_heldObj && _targetRb)
        {
            Vector3 targetPos = playerCamera.position + playerCamera.forward * holdDistance;
            _targetRb.velocity = ((targetPos - _heldObj.transform.position) * moveStrength) / _targetRb.mass;
        }
    }

    void Pickup(GameObject target)
    {
        // Trying to retrieve the object's rigidbody
        // If the object has a rigidbody make it our held object
        target.TryGetComponent(out Rigidbody targetRb);
        if (targetRb)
        {
            target.TryGetComponent(out WeaponInterface weaponInterface);
            if (weaponInterface != null)
            {
                weaponInterface.HeldAsPhysicsObject = true;
                isHeldObjectAWeapon = true;
                Debug.Log("Pick up weapon object");
                _targetRb = targetRb;
                _targetRb.useGravity = false;
                _targetRb.drag = 10;
                target.transform.SetParent(objectParent);
                _heldObj = target;
            }
            else
            {
                Debug.Log("Pick up");
                _targetRb = targetRb;
                _targetRb.useGravity = false;
                _targetRb.drag = 10;

                if (target.TryGetComponent(out Collider col)) _targetCollider = col;

                target.layer = 20;
                target.transform.SetParent(objectParent);
                _heldObj = target;
            }
        }
    }

    void Drop(float force = 0f)
    {
        if (isHeldObjectAWeapon)
        {
            _heldObj.TryGetComponent(out WeaponInterface weaponInterface);
            weaponInterface.HeldAsPhysicsObject = false;
            Debug.Log("Drop");
            Debug.Log(_targetRb.velocity);
            _targetRb.transform.parent = null;
            _targetRb.useGravity = true;
            _targetRb.drag = 0;
            _targetRb.velocity = Vector3.zero;
            _targetRb.AddForce(playerOrientation.forward * (force / _targetRb.mass));
            isHeldObjectAWeapon = false;
            _heldObj = null;
        } else
        {
            Debug.Log("Drop");
            _targetRb.transform.parent = null;
            _targetRb.useGravity = true;
            _targetCollider.enabled = true;
            _targetRb.drag = 0;
            _targetRb.velocity = Vector3.zero;
            _targetRb.gameObject.layer = 9;
            _targetRb.AddForce(playerOrientation.forward * (force / _targetRb.mass));
            
            _heldObj = null;
        }
    }
    
    private GameObject LookingAtInteractable()
    {
        // Checks if there is an object which can be picked up inside the interaction range
        if (Physics.Raycast(objectInteractPosition.position, objectInteractPosition.forward, out RaycastHit hit, interactRange))
        {
            return hit.transform.gameObject ? hit.transform.gameObject : null;
        }
        return null;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 rayDirection = objectInteractPosition.forward * interactRange;
        Gizmos.DrawRay(new Ray(objectInteractPosition.position, rayDirection));
    }
}
