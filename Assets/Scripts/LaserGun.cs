using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LaserGun : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float defDistanceRay = 100;
    [SerializeField] private Transform laserFirePoint;
    [SerializeField] private LineRenderer m_lineRenderer;

    [Header("Gun Settings")]
    [Range(0.1f, 10f)]
    [SerializeField] protected float fireRate = 0.2f;
    [Range(1, 100)]
    [SerializeField] protected int shotsPerReload = 25;
    [Range(0.1f, 5f)]
    [SerializeField] protected float reloadTime = 2f;
    [Range(0.1f, 5f)]
    [SerializeField] protected float reloadDelay = 0.5f;

    public Transform gunHolder;
    public Transform gunPivot;

    [Header("Rotation")]
    [SerializeField] protected bool rotateOverTime = true;
    [Range(0, 60)]
    [SerializeField] protected float rotationSpeed = 4;

    [Header("UI")]
    [SerializeField] protected Slider ammoBar;

    public Camera m_camera;
    [SerializeField] protected bool canShoot = true;

    protected float fireTimer;
    protected int currentAmmo;
    protected bool isReloading = false;
    protected float reloadTimer;

    private Transform m_transform;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
    }

    private void Start()
    {
        currentAmmo = shotsPerReload;
        if (ammoBar != null)
        {
            ammoBar.maxValue = shotsPerReload;
            ammoBar.value = currentAmmo;
        }
    }

    private void Update()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        RotateGun(mousePos, true);

        if (canShoot)
        {
            if (Input.GetKey(KeyCode.Mouse0) && fireTimer <= 0f && currentAmmo > 0)
            {
                ShootLaser();
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
        else
        {
            // If not shooting, ensure the laser line renderer is disabled
            m_lineRenderer.enabled = false;
        }
    }

    private void ShootLaser()
    {
        m_lineRenderer.enabled = true; // Ensure the line renderer is enabled when shooting
        if (Physics2D.Raycast(m_transform.position, transform.right))
        {
            RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, m_transform.right);
            Draw2DRay(laserFirePoint.position, _hit.point);
        }
        else
        {
            Draw2DRay(laserFirePoint.position, laserFirePoint.transform.right * defDistanceRay);
        }
    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
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
