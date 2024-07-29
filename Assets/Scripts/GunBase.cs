using System.Collections;
using UnityEngine;
using CodeMonkey.Utils;
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

    
    

    [Header("Precision:")]
    [Range(0f, 1f)]
    [SerializeField] protected float precision = 1f; // 1 is perfect precision, 0 is very inaccurate

    public Camera m_camera;

    [Header("Shooting")]
    [SerializeField] protected bool canShoot = true; // New property to determine if the gun can shoot

    protected float fireTimer;
    protected int currentAmmo;
    protected bool isReloading = false;
    protected float reloadTimer;

    // Référence à la barre de munitions globale
    private static Slider globalAmmoBar;

    public static void SetGlobalAmmoBar(Slider slider)
    {
        globalAmmoBar = slider;
    }

    void Start()
    {
        currentAmmo = shotsPerReload;
        if (globalAmmoBar != null)
        {
            globalAmmoBar.maxValue = shotsPerReload;
            globalAmmoBar.value = currentAmmo;
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

        if (canShoot) // Check if the gun can shoot
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
        if (globalAmmoBar != null)
        {
            globalAmmoBar.value = currentAmmo;
        }
    }

    public void Equip()
    {
        // Initialiser ou réinitialiser l'état de l'arme lors de l'équipement
        currentAmmo = shotsPerReload;
        UpdateAmmoBar();
    }

    public void Unequip()
    {
        // Vous pouvez ajouter ici des actions spécifiques lors du déséquipement de l'arme
    }
}
