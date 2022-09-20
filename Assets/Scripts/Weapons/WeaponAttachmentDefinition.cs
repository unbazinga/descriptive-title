using UnityEngine;

public class WeaponAttachmentDefinition : ScriptableObject, IPickupable
{
    public GameObject weaponAttachmentModel;
    public float weaponFireRateBuff;
    public float weaponAccuracyBuff;
    public float weaponDamageBuff;
    public float weaponSpreadBuff;

    public void Pickup(Transform t)
    {
        
    }

    public void Drop(Transform t, float f)
    {
        
    }
}