using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class Laser_Tutorial : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 100f; // Maximum range of the laser
    [SerializeField] private float damageInterval = 0.1f; // Interval between each damage application
    [SerializeField] private float damageAmount = 10f; // Amount of damage per interval

    public Transform laserFirePoint;
    public LineRenderer m_lineRenderer;
    private Transform m_transform;

    

    public Transform gunHolder;
    public Transform gunPivot;
    public Camera m_camera;

    [Header("Ammo")]
    [Range(1, 100)]
    [SerializeField] protected int shotsPerReload = 25;
    [Range(0.1f, 5f)]
    [SerializeField] protected float reloadTime = 2f;
    [Range(0.1f, 5f)]
    [SerializeField] protected float reloadDelay = 0.5f;

    [Header("UI")]
    [SerializeField] protected Slider ammoBar;
    private static Slider globalAmmoBar;

    public static void SetGlobalAmmoBar(Slider ammoBar)
    {
        globalAmmoBar = ammoBar;
    }

    [Header("Layer Settings")]
    [SerializeField] private LayerMask ignoreLayer; // LayerMask to ignore specific layers

    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();
    private float damageTimer;

    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
    }

    private void Start()
    {
        ignoreLayer = LayerMask.GetMask("Pickup");
        currentAmmo = shotsPerReload;
        if (ammoBar != null)
        {
            ammoBar.maxValue = shotsPerReload;
            ammoBar.value = currentAmmo;
        }
    }

    private void Update()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gunPivot.eulerAngles = new Vector3(0, 0, angle);
        Debug.Log(angle);

        if (Input.GetKey(KeyCode.Mouse0) && currentAmmo > 0 && !isReloading)
        {
            ShootLaser();
            m_lineRenderer.enabled = true; // Enable the line renderer when shooting

            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                ApplyDamage();
                damageTimer = 0f;
                currentAmmo--;
                UpdateAmmoBar();
            }
            reloadTimer = 0f;
        }
        else
        {
            m_lineRenderer.enabled = false; // Disable the line renderer when not shooting
            hitTargets.Clear(); // Clear hit targets when not shooting
            damageTimer = 0f; // Reset damage timer

            if (currentAmmo < shotsPerReload)
            {
                reloadTimer += Time.deltaTime;
                if (reloadTimer >= reloadDelay && !isReloading)
                {
                    StartCoroutine(Reload());
                }
            }
        }

        if (globalAmmoBar != null)
        {
            globalAmmoBar.maxValue = shotsPerReload;
            globalAmmoBar.value = currentAmmo;
        }
    }

    void ShootLaser()
    {
        Vector2 startPos = laserFirePoint.position;
        Vector2 direction = m_transform.right;

        // Use LayerMask to ignore specific layers
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, defDistanceRay, ~ignoreLayer);
        while (hit.collider != null && (ignoreLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
        {
            // Continue the raycast from the hit point, ignoring the layer
            startPos = hit.point + direction * 0.1f; // Move the start point slightly forward to avoid hitting the same collider again
            hit = Physics2D.Raycast(startPos, direction, defDistanceRay, ~ignoreLayer);
        }

        if (hit.collider != null)
        {
            Draw2DRay(startPos, hit.point);
            hitTargets.Add(hit.collider);
        }
        else
        {
            Vector2 endPos = startPos + direction * defDistanceRay;
            Draw2DRay(startPos, endPos);
        }
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    

    private void ApplyDamage()
    {
        foreach (var target in hitTargets)
        {
            ShieldSystem shieldSystem = target.GetComponent<ShieldSystem>();
            HealthSystem healthSystem = target.GetComponent<HealthSystem>();

            if (shieldSystem != null && healthSystem != null)
            {
                if (shieldSystem.GetCurrentShield() > 0)
                {
                    shieldSystem.TakeShieldDamage(damageAmount);
                }
                else
                {
                    healthSystem.TakeDamage(damageAmount);
                }
            }
        }
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
