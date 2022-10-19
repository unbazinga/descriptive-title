using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentInterface : MonoBehaviour, IPickupable
{
    public Rigidbody _rb;
    public WeaponAttachmentDefinition AttachmentDefinition;
    
    public void Pickup(Transform t)
    {
        throw new System.NotImplementedException();
    }

    public void Drop(Transform t, float f)
    {
        throw new System.NotImplementedException();
    }
}
