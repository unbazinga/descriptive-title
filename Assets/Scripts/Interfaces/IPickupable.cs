using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    public abstract void Pickup(Transform t);
    public abstract void Drop(Transform t, float f);
}
