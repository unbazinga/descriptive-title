using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectInteracting : MonoBehaviour
{
    public float moveForce = 100;
    public Vector3 objectPosition;
    public Vector3 holdOffset = new Vector3(2,0,0);
    public float distance;

    public static bool canInteract;
    public static bool isHolding;
    private static GameObject gameObject;
    private static Rigidbody _targetRb;
    [FormerlySerializedAs("TempParent")] public static GameObject tempParent;
    public static GameObject pCam = null;
    private static bool _isHolding;

    void Update()
    {
        if (_isHolding)
        {
            _targetRb.velocity = Vector3.zero;
            _targetRb.angularVelocity = Vector3.zero;
            gameObject.transform.SetParent(tempParent.transform);
            if (Vector3.Distance(_targetRb.position, PlayerMovement.holdArea.transform.position) > 0.1f)
            {
                var moveDir = (PlayerMovement.holdArea.transform.position - this.transform.position);
                _targetRb.AddForce(moveDir * moveForce);
            }
        }
    }

    public static void PickUp(GameObject target)
    {
        _isHolding = true;
        isHolding = true;
        canInteract = false;

        target.TryGetComponent(out Rigidbody targetRb);
        if (targetRb)
        {
            _targetRb = targetRb;
            targetRb.useGravity = false;
            targetRb.drag = 10;
            targetRb.detectCollisions = false;
            targetRb.transform.SetParent(PlayerMovement.holdArea.transform);
        }

        gameObject = target;
        
    }

    public static void Drop(float force = 0f)
    {
        _isHolding = false;
        _targetRb.drag = 1;
        _targetRb.useGravity = true;
        _targetRb.detectCollisions = true;
        _targetRb.AddForce(PlayerMovement.holdArea.transform.forward * force);
        _targetRb.transform.SetParent(null);
    }
    
}
