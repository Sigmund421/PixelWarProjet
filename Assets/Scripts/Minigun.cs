using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Minigun : MonoBehaviour
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

    [Header("Minigun Specifics")]
    [SerializeField] private float spinUpTime = 1f; // Time to wait before starting to shoot

    public Transform gunHolder;
    public Transform gunPivot;

    [Header("Rotation:")]
    [SerializeField] protected bool rotateOverTime = true;
    [Range(0, 60)][SerializeField] protected float rotationSpeed = 4;

    [Header("UI")]
    [SerializeField] private Slider ammoBar;

    [Header("Precision:")]
    [Range(0f, 1f)]
    [SerializeField] private float precision = 1f; // 1 is perfect precision, 0 is very inaccurate

    public Camera m_camera;

    private float fireTimer;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer;
    private bool isSpinningUp = false;
    private bool canShoot = false;

    void Start()
    {
        currentAmmo = shotsPerReload;
        if (ammoBar != null)
        {
            ammoBar.maxValue = shotsPerReload;
            ammoBar.value = currentAmmo;
        }
    }

    void Update()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        RotateGun(mousePos, true);

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isSpinningUp && currentAmmo > 0 && !isReloading)
        {
            StartCoroutine(SpinUp());
        }

        if (Input.GetKey(KeyCode.Mouse0) && fireTimer <= 0f && currentAmmo > 0 && !isReloading && canShoot)
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
            if (reloadTimer >= reloadDelay && !isReloading)
            {
                StartCoroutine(Reload());
            }
        }
    }

    private IEnumerator SpinUp()
    {
        isSpinningUp = true;
        canShoot = false; // Disable shooting during spin up
        yield return new WaitForSeconds(spinUpTime);
        canShoot = true; // Enable shooting after spin up
        isSpinningUp = false;
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
}
