using System.Collections;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class Minigun : MonoBehaviour
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

    [SerializeField] private float spinUpTime = 1f;
    private bool isSpinningUp = false;

    [Header("UI")]
    [SerializeField] protected Slider ammoBar;
    private static Slider globalAmmoBar;

    public static void SetGlobalAmmoBar(Slider ammoBar)
    {
        globalAmmoBar = ammoBar;
    }

    [Header("Precision:")]
    [Range(0f, 1f)]
    [SerializeField] protected float precision = 1f; 

    public Camera m_camera;

    [Header("Shooting")]
    [SerializeField] protected bool canShoot = true; 

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

        if (isSpinningUp == true)
        {
            canShoot= false;
        }

    }

    void Update()
    {

        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gunPivot.eulerAngles = new Vector3(0, 0, angle);
        Debug.Log(angle);

        if (m_camera == null)
        {
            Debug.LogError("Camera reference not set in GunBase.");
            return;
        }

        if (globalAmmoBar != null)
        {
            globalAmmoBar.maxValue = shotsPerReload;
            globalAmmoBar.value = currentAmmo;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isSpinningUp && currentAmmo > 0)
        {
            StartCoroutine(SpinUp());
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

    protected virtual void Shoot()
    {
        
        float spreadAngle = (1f - precision) * 10f; 
        float angle = Random.Range(-spreadAngle, spreadAngle);

        
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

    private IEnumerator SpinUp()
    {
        isSpinningUp = true;
        canShoot = false; 
        yield return new WaitForSeconds(spinUpTime);
        canShoot = true; 
        isSpinningUp = false;
    }

}
