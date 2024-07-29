using System.Collections;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class GunBase : MonoBehaviour
{
    [SerializeField] protected GameObject bulletPrefab; // Prefab de la balle correspondante
    [SerializeField] protected Transform firingPoint; // Point de là où le tire part
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

    // PETITE INFO : Tous les scripts d'armes, que ce soit celles qui dérivent de ce script ou les autres (Minigun, FlameThrower...) ont quasiment la même logique
    // Je dis ça pour pas avoir à annoter chaque scripts un à un. J'annote pas tout, l'anglais parle de lui même.

    [Header("Precision:")]
    [Range(0f, 1f)]
    [SerializeField] protected float precision = 1f; // 1 est precision parfaite

    public Camera m_camera;

    [Header("Shooting")]
    [SerializeField] protected bool canShoot = true;

    protected float fireTimer;
    protected int currentAmmo;
    protected bool isReloading = false;
    protected float reloadTimer;

    
    private static Slider globalAmmoBar; // Référence à la barre de munitions globale

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
        Debug.Log(angle); // Les 4 lignes + celle ci c'est la logique qui fait que l'arme suit le curseur de la souris

        if (m_camera == null)
        {
            Debug.LogError("Camera reference not set in GunBase.");
            return;
        }

        if (canShoot) // Logique d'application de tir, de rechargement et de munitions
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
        // Champ pour la précision plus ou moins élevée
        float spreadAngle = (1f - precision) * 10f; 
        float angle = Random.Range(-spreadAngle, spreadAngle);

        // Le champ de précision est mis pour la trajectoire de tir
        Quaternion spreadRotation = Quaternion.Euler(0, 0, angle);
        Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation * spreadRotation);
    }

    private IEnumerator Reload() //Reloading de manière à recharger automatiquement les balles si on ne tire pas
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
        // Initialise ou réinitialise l'état de l'arme lors de l'équipement
        currentAmmo = shotsPerReload;
        UpdateAmmoBar();
    }

    public void Unequip()
    {
        
    }
}
