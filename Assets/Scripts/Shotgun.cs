using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : GunBase
{
    [SerializeField] private GameObject shotgunBulletPrefab;
    [Range(1, 10)]
    [SerializeField] private int pelletCount = 6;
    [Range(0f, 45f)]
    [SerializeField] private float spreadAngle = 15f;

    protected override void Shoot()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            float angle = spreadAngle * (i - (pelletCount - 1) / 2f) / (pelletCount - 1);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Instantiate(shotgunBulletPrefab, firingPoint.position, firingPoint.rotation * rotation);
        }
    }
}
