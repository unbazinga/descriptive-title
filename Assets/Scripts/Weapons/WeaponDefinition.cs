using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

[CreateAssetMenu(menuName = "DumDum Weapons / Create Weapon", fileName = "New Weapon")]
public class WeaponDefinition : ScriptableObject
{
    public enum WeaponFireMode
    {
        SINGLE,
        BURST,
        AUTO,
        SAFE
    };

    public WeaponFireMode weaponFireMode;

    public float weaponKick;
    public float weaponScreenShake;
    public float weaponSpread;
    public float weaponBulletsPerShot;
    public float weaponFireRate;

    public string weaponItemName;
    public string weaponItemDescription;
    public Sprite weaponItemSprite;

}