using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunBase : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [Range(0.1f, 10f)]
    [SerializeField] private float fireRate = 0.2f;

    [Header("Ammo")]
    [Range(1, 100)]
    [SerializeField] private int shotsPerReload = 25;
    [Range(0.1f, 5f)]
    [SerializeField] private float reloadTime = 2f;
    [Range(0.1f, 5f)]
    [SerializeField] private float reloadDelay = 0.5f;

    public Transform gunHolder;
    public Transform gunPivot;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)][SerializeField] private float rotationSpeed = 4;

    [Header("UI")]
    [SerializeField] private Slider ammoBar;

    public Camera m_camera;

    private float fireTimer;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = shotsPerReload;
        if (ammoBar != null)
        {
            ammoBar.maxValue = shotsPerReload;
            ammoBar.value = currentAmmo;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        RotateGun(mousePos, true);

        if (Input.GetKey(KeyCode.Mouse0) && fireTimer <= 0f && currentAmmo > 0)
        {
            Shoot();
            fireTimer = fireRate;
            currentAmmo--;
            UpdateAmmoBar();
            reloadTimer = 0f; // Reset reload timer when shooting
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

    private void Shoot()
    {
        Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
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
        reloadTimer = 0f; // Reset reload timer when reloading is complete
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
}
