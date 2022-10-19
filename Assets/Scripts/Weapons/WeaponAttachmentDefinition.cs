using UnityEngine;

[CreateAssetMenu(menuName = "DumDum Weapons / Create Weapon Attachment", fileName = "New Weapon Attachment")]
public class WeaponAttachmentDefinition : ScriptableObject, IPickupable
{
    public GameObject weaponAttachmentModel;
    public new string name;
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