using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WeaponInterface : MonoBehaviour, IPickupable
{
    public Rigidbody _rb;
    public WeaponDefinition WeaponDefinition;
    public WeaponAttachmentDefinition[] WeaponAttachments;
    [FormerlySerializedAs("trail")] public TrailRenderer BulletTrail;
    public Transform Shell;
    public Transform ShellEjector;
    public Transform weaponHolder;
    public Transform weaponFirePoint;
    public Transform weaponSightPosition;
    public Transform weaponTracerPoint;
    public Collider[] gfxColliders;
    public GameObject[] weaponGfxs;
    public float bulletHitForce;
    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;
    public int weaponGfxLayer;
    public int weaponDefLayer;
    public bool _held;


    public bool isADS;
    public float adsSpeed;
    public bool HeldAsPhysicsObject { get; set; }
    public bool shouldBulletsSpread;
    public LayerMask shootMask;
    
    
    public void Shoot()
    {
        Debug.Log("Shooting");
        var direction = GetDirection();
        if (Physics.Raycast(weaponFirePoint.position, direction, out var hit, float.MaxValue,
                shootMask))
        {
            Debug.Log("Raycast out");
            var trail = Instantiate(BulletTrail, weaponTracerPoint.position, Quaternion.identity);
            Instantiate(Shell, ShellEjector.position, ShellEjector.rotation);
            Debug.Log("Trail Spawning");
            StartCoroutine(SpawnTrail(trail, hit));
            Debug.Log(hit.transform.name);
            if (hit.transform.TryGetComponent(out Rigidbody hitRb))
                hitRb.AddForce(gameObject.transform.forward * bulletHitForce, ForceMode.Impulse);
            else
                Debug.Log("Object shot has no rigidbody");
        }
    }

    public IEnumerator AimDownSights(bool shouldADS)
    {
        if (!_held) yield break;
        if (HeldAsPhysicsObject) yield break;
        Vector3 oldPos = this.transform.localPosition;
        while (transform.localPosition != weaponSightPosition.position)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, weaponSightPosition.position,
                adsSpeed * Time.deltaTime);
        }
        
        yield return new WaitWhile(() => transform.localPosition != weaponSightPosition.position);
        
        //transform.localPosition = Vector3.Lerp(transform.localPosition, shouldADS ? weaponSightPosition.position : Vector3.zero, Time.deltaTime * adsSpeed);
    }
    public void Pickup(Transform t)
    {
        if (_held) return;
        if (HeldAsPhysicsObject) return;
        
        Debug.Log("Pickup :))");
        _rb.velocity = Vector3.zero;
        Destroy(_rb);
        transform.parent = weaponHolder;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        foreach (var collider in gfxColliders) collider.enabled = false;

        foreach (var gfx in weaponGfxs) gfx.layer = weaponGfxLayer;

        _held = true;
    }
    

    public void Drop(Transform orientation, float f)
    {
        if (!_held) return;
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.mass = 0.1f;
        var forward = orientation.forward;
        forward.y = 0f;
        _rb.velocity = Vector3.zero;
        _rb.velocity = forward * (throwForce * f);
        _rb.velocity += Vector3.up * (throwExtraForce * f);
        _rb.angularVelocity = Random.onUnitSphere * rotationForce;
        transform.parent = null;
        foreach (var collider in gfxColliders) collider.enabled = true;

        foreach (var gfx in weaponGfxs) gfx.layer = weaponDefLayer;

        _held = false;
    }

    private Vector3 GetDirection()
    {
        var spread = new Vector3(WeaponDefinition.weaponSpread, WeaponDefinition.weaponSpread,
            WeaponDefinition.weaponSpread);
        var dir = weaponFirePoint.forward;
        if (shouldBulletsSpread)
        {
            dir += new Vector3(
                Random.Range(-spread.x, spread.x),
                Random.Range(-spread.y, spread.y),
                Random.Range(-spread.z, spread.z));
            dir.Normalize();
        }

        return dir;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit hit)
    {
        Debug.Log("Spawning Trail");
        float time = 0;
        var startPosition = Trail.transform.position;
        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / Trail.time;
            yield return null;
        }

        Trail.transform.position = hit.point;

        Destroy(Trail.gameObject, Trail.time);
    }

}