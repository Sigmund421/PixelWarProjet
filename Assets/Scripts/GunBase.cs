using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunBase : MonoBehaviour
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firingPoint;
    [Range(0.1f, 10f)]
    [SerializeField] protected float fireRate = 0.2f;

    [Header("Ammo")]
    [Range(1, 100)]
    [SerializeField] protected int shotsPerReload = 25;
    [Range(0.1f, 5f)]
    [SerializeField] protected float reloadTime = 2f;
    [Range(0.1f, 5f)]
    [SerializeField] protected float reloadDelay = 0.5f;

    public Transform gunHolder;
    public Transform gunPivot;

    [Header("Rotation:")]
    [SerializeField] protected bool rotateOverTime = true;
    [Range(0, 60)][SerializeField] protected float rotationSpeed = 4;

    [Header("UI")]
    [SerializeField] protected Slider ammoBar;

    [Header("Precision:")]
    [Range(0f, 1f)]
    [SerializeField] protected float precision = 1f; // 1 is perfect precision, 0 is very inaccurate

    [Header("Crosshair:")]
    [SerializeField] protected GameObject crosshair; // Crosshair GameObject
    [SerializeField] protected float maxCrosshairDistance = 5f; // Max distance for the crosshair

    public Camera m_camera;

    protected float fireTimer;
    protected int currentAmmo;
    protected bool isReloading = false;
    protected float reloadTimer;

    void Start()
    {
        currentAmmo = shotsPerReload;
        if (ammoBar != null)
        {
            ammoBar.maxValue = shotsPerReload;
            ammoBar.value = currentAmmo;
        }
        Cursor.visible = false; // Hide the default cursor
    }

    void Update()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        RotateGun(mousePos, true);
        UpdateCrosshairPosition(mousePos);

        if (Input.GetKey(KeyCode.Mouse0) && fireTimer <= 0f && currentAmmo > 0)
        {
            Shoot();
            fireTimer = fireRate;
            currentAmmo--;
            UpdateAmmoBar();
            reloadTimer = 0f;
        }
        else
        {
            fireTimer -= Time.deltaTime;
        }

        if (currentAmmo < shotsPerReload && !Input.GetKey(KeyCode.Mouse0))
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= reloadDelay)
            {
                if (!isReloading)
                {
                    StartCoroutine(Reload());
                }
            }
        }
    }

    protected virtual void Shoot()
    {
        // Calculate a random spread angle based on precision
        float spreadAngle = (1f - precision) * 10f; // Adjust the multiplier as needed for the spread
        float angle = Random.Range(-spreadAngle, spreadAngle);

        // Apply the spread angle to the firing direction
        Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
        Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation * spreadRotation);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        while (currentAmmo < shotsPerReload && !Input.GetKey(KeyCode.Mouse0))
        {
            yield return new WaitForSeconds(reloadTime / shotsPerReload);
            currentAmmo++;
            UpdateAmmoBar();
        }
        isReloading = false;
        reloadTimer = 0f;
    }

    private void UpdateAmmoBar()
    {
        if (ammoBar != null)
        {
            ammoBar.value = currentAmmo;
        }
    }

    private void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void UpdateCrosshairPosition(Vector2 mousePos)
    {
        Vector2 gunPosition = gunHolder.position;
        Vector2 direction = (mousePos - gunPosition).normalized;
        float distance = Vector2.Distance(mousePos, gunPosition);

        if (distance > maxCrosshairDistance)
        {
            crosshair.transform.position = gunPosition + direction * maxCrosshairDistance;
        }
        else
        {
            crosshair.transform.position = mousePos;
        }
    }
}
