using System.Collections;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class Flamethrower : MonoBehaviour
{
    [SerializeField] private GameObject flamePrefab; // Prefab pour le jet de feu
    [SerializeField] private Transform firingPoint; // Point de tir
    
    [SerializeField] private float fireRate = 0.1f; // Vitesse de tir du lance-flammes

    [Header("Ammo")]
    
    [SerializeField] private int shotsPerReload = 100;
    [Range(0.1f, 5f)]
    [SerializeField] private float reloadTime = 2f;
    [Range(0.1f, 5f)]
    [SerializeField] private float reloadDelay = 0.5f;

    public Transform gunHolder;
    public Transform gunPivot;

    public static void SetGlobalAmmoBar(Slider ammoBar)
    {
        globalAmmoBar = ammoBar;
    }

    [Header("UI")]
    [SerializeField] private Slider ammoBar;
    private static Slider globalAmmoBar;

    [Header("Precision:")]
    [Range(0f, 1f)]
    [SerializeField] private float precision = 1f; // 1 is perfect precision, 0 is very inaccurate

    public Camera m_camera;

    [Header("Shooting")]
    [SerializeField] private bool canShoot = true;

    private float fireTimer;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer;

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
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gunPivot.eulerAngles = new Vector3(0, 0, angle);
        Debug.Log(angle);

        if (globalAmmoBar != null)
        {
            globalAmmoBar.maxValue = shotsPerReload;
            globalAmmoBar.value = currentAmmo;
        }

        if (canShoot)
        {
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
    }

    private void Shoot()
    {
        // Calculate a random spread angle based on precision
        float spreadAngle = (1f - precision) * 30f; // Adjust the multiplier as needed for the spread
        float angle = Random.Range(-spreadAngle, spreadAngle);

        // Apply the spread angle to the firing direction
        Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
        Instantiate(flamePrefab, firingPoint.position, firingPoint.rotation * spreadRotation);
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

    
}
