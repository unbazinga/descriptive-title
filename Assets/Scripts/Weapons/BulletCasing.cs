using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BulletCasing : MonoBehaviour
{
    public float minEjectForce, maxEjectForce;
    public float shellLifetime, shellFadeTime;
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out Rigidbody _rb) && _rb != null)
        {
            float force = Random.Range(minEjectForce, maxEjectForce);
            _rb.AddForce(transform.right * force);
            _rb.AddTorque(Random.insideUnitSphere * force);

            StartCoroutine(FadeShell());
        }

    }

    IEnumerator FadeShell()
    {
        yield return new WaitForSeconds(shellLifetime);
        float shellFadePercent = 0f, shellFadeSpeed = 1f / shellFadeTime;
        Material mat = GetComponentInChildren<Renderer>().material;
        Color initialShellColor = mat.color;
        while (shellFadePercent < 1)
        {
            shellFadePercent += Time.deltaTime * shellFadeSpeed;
            mat.color = Color.Lerp(initialShellColor, Color.clear, shellFadePercent);
            yield return null;
        } 
        Destroy(this.gameObject);
    }

}
