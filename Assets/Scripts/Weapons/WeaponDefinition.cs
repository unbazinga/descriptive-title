using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

[CreateAssetMenu(menuName = "DumDum Weapons / Create Weapon", fileName = "New Weapon")]
public class WeaponDefinition : ScriptableObject
{
    public enum WeaponFireMode
    {
        SAFE,
        SINGLE,
        BURST,
        AUTO,
    };

    
    public WeaponFireMode weaponFireMode;
    public Transform weaponSightPosition;
    
    public float weaponKick;
    public float weaponScreenShake;
    public float weaponSpread;
    public float weaponBulletsPerShot;
    public float weaponFireRate;

    
    public string weaponItemName;
    public string weaponItemDescription;
    public Sprite weaponItemSprite;
    
    
    public static List<Tuple<string, bool>> AllowedFireModes;
    public static List<T> GetEnumList<T>() where T : Enum 
        => ((T[])Enum.GetValues(typeof(T))).ToList();
}