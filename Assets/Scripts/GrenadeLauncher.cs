using System.Collections;
using UnityEngine;

public class GrenadeLauncher : GunBase
{
    [SerializeField] private float grenadeSpeed = 10f; // Vitesse de la grenade
    [SerializeField] private GameObject grenadePrefab; // Préfab de la grenade

    protected override void Shoot()
    {
        float spreadAngle = (1f - precision) * 10f;
        float angle = Random.Range(-spreadAngle, spreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
        GameObject grenade = Instantiate(grenadePrefab, firingPoint.position, firingPoint.rotation * spreadRotation);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        rb.velocity = firingPoint.up * grenadeSpeed;
    }
}
