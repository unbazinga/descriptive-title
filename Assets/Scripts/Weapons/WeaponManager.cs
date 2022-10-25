using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WeaponManager : MonoBehaviour
{
    public float pickupDistance, pickupRadius;
    public int weaponLayer;

    public Transform weaponHolder, playerCamera, orientation;
    public GameObject player;
    public StrengthBarHandler StrengthBarHandler;
    private bool _isWeaponHeld;
    private WeaponInterface _heldWeapon;
    private float throwPowerOverTime;
    private float nextFire = 0f;
    public float throwPowerMax = .8f;
    public float timeForUIToFadeOut;
    private bool hasShot;
    private bool shouldFadeOut, shouldFadeIn;

    public float swayIntensity;
    public float swaySmoothing;

    private static Quaternion _origin;

    private void Awake()
    {
        StrengthBarHandler.maxStrength = throwPowerMax;
    }

    public bool IsWeaponHeld() => _isWeaponHeld;

    private void Update()
    {
        
        if (_isWeaponHeld)
        {
            WeaponSwayUpdate();
            if (Input.GetKey(KeyCode.Q))
            {
                shouldFadeIn = true;
                if (throwPowerOverTime < throwPowerMax)
                    throwPowerOverTime += (Time.deltaTime * 1.5f);
                else
                    throwPowerOverTime = throwPowerMax;
                StrengthBarHandler.UpdateBar(throwPowerOverTime);
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                _heldWeapon.Drop(playerCamera, throwPowerOverTime);
                _heldWeapon = null;
                _isWeaponHeld = false;
                throwPowerOverTime = 0f;
                StrengthBarHandler.ResetBar();
                shouldFadeOut = true;

            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                player.TryGetComponent(out PlayerMovement pMovement);
                foreach (var mod in pMovement.HeldAttachments)
                {
                    Debug.Log(mod.name);
                }
            }
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            var hitList = new RaycastHit[256];
            var hitNum = Physics.CapsuleCastNonAlloc(playerCamera.position,
                playerCamera.position + playerCamera.forward * pickupDistance, pickupRadius, playerCamera.forward,
                hitList);

            var tList = new List<RaycastHit>();
            for (int i = 0; i < hitNum; i++)
            {
                var hit = hitList[i];
                if (hit.transform.gameObject.layer != weaponLayer) continue;
                if (hit.point == Vector3.zero)
                {
                    tList.Add(hit);
                } else if (Physics.Raycast(playerCamera.position, hit.point - playerCamera.position, out var hitInfo,
                               hit.distance + 0.1f) && hitInfo.transform == hit.transform)
                {
                    tList.Add(hit);
                }
            }

            if (tList.Count == 0) return;
            
            tList.Sort((hit1, hit2) =>
            {
                var dist1 = GetDistanceTo(hit1);
                var dist2 = GetDistanceTo(hit2);
                return Mathf.Abs(dist1 - dist2) < 0.001f ? 0 : dist1 < dist2 ? -1 : 1;
            });

            _isWeaponHeld = true;
            _heldWeapon = tList[0].transform.GetComponent<WeaponInterface>();
            _heldWeapon.Pickup(weaponHolder);
            _origin = _heldWeapon.transform.localRotation;

        } 
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (_isWeaponHeld && _heldWeapon != null)
            {
                var fireModes = Enum.GetNames(typeof(WeaponDefinition.WeaponFireMode));
                var curFireMode = _heldWeapon.WeaponDefinition.weaponFireMode;
                Debug.Log(fireModes.Length);
                Debug.Log(curFireMode + " Before");
                
                if ((int)curFireMode < (fireModes.Length - 1))
                    curFireMode += 1;
                else
                    curFireMode = 0;
                _heldWeapon.WeaponDefinition.weaponFireMode = curFireMode;
                Debug.Log(curFireMode + " Changed");

            }
            
        }
        
        if (Input.GetMouseButton(0))
        {
            if (this._heldWeapon != null)
            {
                if (this._heldWeapon.WeaponDefinition == null)
                {
                    Debug.Log("No Weapon Definition");
                }
                else
                {
                    //Debug.Log("Weapon has Weapon Definition Script");
                    switch (this._heldWeapon.WeaponDefinition.weaponFireMode)
                    {
                        case WeaponDefinition.WeaponFireMode.AUTO:
                            //Debug.Log("Weapon is in Auto, shooting");

                            if (Time.time > nextFire + (this._heldWeapon.WeaponDefinition.weaponFireRate / 20f))
                            {
                                nextFire = Time.time;
                                this._heldWeapon.Shoot();
                            }
                            break;
                        case WeaponDefinition.WeaponFireMode.BURST:
                            if (!hasShot)
                            {
                                hasShot = true;
                                if (Time.time > nextFire + (this._heldWeapon.WeaponDefinition.weaponFireRate / 10f))
                                {
                                    for (int i = 0; i < this._heldWeapon.WeaponDefinition.weaponBulletsPerShot; i++)
                                    {

                                        nextFire = Time.time;
                                        this._heldWeapon.Shoot();
                                    }
                                }

                            }

                            break;
                        case WeaponDefinition.WeaponFireMode.SINGLE:
                            if (!hasShot)
                            {
                                hasShot = true;
                                this._heldWeapon.Shoot(); // Single Pew
                            }
                            break;
                        case WeaponDefinition.WeaponFireMode.SAFE:
                            // No Pew Pew
                            break;
                    }
                }
            }
            else
            {
                Debug.Log("Held weapon is null, doing nothing");
            }
        } else if (Input.GetMouseButtonUp(0))
        {
            if (hasShot && Time.time > nextFire + (this._heldWeapon.WeaponDefinition.weaponFireRate / 10f))
            {
                hasShot = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (_heldWeapon != null)
                if (!_heldWeapon.isADS)
                    _heldWeapon.StartCoroutine(_heldWeapon.AimDownSights(true));

        } else if (Input.GetMouseButtonUp(1))
        {
            if (_heldWeapon != null)
                if (_heldWeapon.isADS)
                    _heldWeapon.StartCoroutine(_heldWeapon.AimDownSights(false));
        }
        if (shouldFadeIn)
        {
            StrengthBarHandler.FadeIn(.35f);
            shouldFadeIn = false;
        } else if (shouldFadeOut)
        {
            StrengthBarHandler.StartCoroutine(StrengthBarHandler.FadeOut(.35f, timeForUIToFadeOut));
            shouldFadeOut = false;
        }
    }
    
    void WeaponSwayUpdate()
    {
        float xM = Input.GetAxis("Mouse X"), yM = Input.GetAxis("Mouse Y");
        if (!_heldWeapon._held) return;
        Debug.Log("Sway");
        var xAdj = Quaternion.AngleAxis(-swayIntensity * xM, Vector3.up);
        var yAdj = Quaternion.AngleAxis(swayIntensity * yM, Vector3.right);
        var targetRot = _origin * xAdj * yAdj;
        _heldWeapon.transform.localRotation =
            Quaternion.Lerp(_heldWeapon.transform.localRotation, targetRot, swaySmoothing * Time.deltaTime);


    }

    private float GetDistanceTo(RaycastHit raycastHit)
    {
        return Vector3.Distance(playerCamera.position,
            raycastHit.point == Vector3.zero ? raycastHit.transform.position : raycastHit.point);
    }
}