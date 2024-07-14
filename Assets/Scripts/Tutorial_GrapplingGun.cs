using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_GrapplingGun : MonoBehaviour
{
    public PlayerController script;

    [Header("Scripts Ref:")]
    public Tutorial_GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private int grappableLayerNumber = 9;
    [SerializeField] private LayerMask ignoreLayer; // Ajouter une LayerMask pour ignorer les layers

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)][SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistance = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float baseLaunchSpeed = 0.8f; // Vitesse de propulsion de base
    private float currentLaunchSpeed;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequency = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private int consecutiveGrapples = 0; // Compteur de grappinages consécutifs
    private float originalGravityScale;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        currentLaunchSpeed = baseLaunchSpeed; // Initialisation de la vitesse de propulsion
        originalGravityScale = m_rigidbody.gravityScale; // Stocker l'échelle de gravité d'origine
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (launchType == LaunchType.Transform_Launch)
                {
                    Vector2 firePointDistance = firePoint.position - gunHolder.localPosition;
                    Vector2 targetPos = grapplePoint - firePointDistance;
                    gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * currentLaunchSpeed);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            m_rigidbody.gravityScale = originalGravityScale; // Réinitialiser la gravité du joueur après le grappin
            consecutiveGrapples = 0; // Réinitialiser le compteur de grappinages consécutifs
            currentLaunchSpeed = baseLaunchSpeed; // Réinitialiser la vitesse de propulsion

            // Conservation de la vélocité
            m_rigidbody.velocity = m_rigidbody.velocity;
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
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

    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, Mathf.Infinity, ~ignoreLayer);
        while (_hit.transform != null && _hit.transform.gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            // Continuer le raycast depuis le point de contact actuel pour ignorer les pickups
            _hit = Physics2D.Raycast(_hit.point + distanceVector.normalized * 0.1f, distanceVector.normalized, Mathf.Infinity, ~ignoreLayer);
        }
        if (_hit.transform != null && (_hit.transform.gameObject.layer == grappableLayerNumber || grappleToAll))
        {
            if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistance || !hasMaxDistance)
            {
                grapplePoint = _hit.point;
                grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                grappleRope.enabled = true;

                // Augmenter la vitesse de propulsion pour les grappinages consécutifs
                currentLaunchSpeed = baseLaunchSpeed + (consecutiveGrapples * 2);
                consecutiveGrapples++;
            }
        }
    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequency;
        }
        if (!launchToPoint)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            switch (launchType)
            {
                case LaunchType.Physics_Launch:
                    m_springJoint2D.connectedAnchor = grapplePoint;

                    Vector2 distanceVector = firePoint.position - gunHolder.position;

                    m_springJoint2D.distance = distanceVector.magnitude;
                    m_springJoint2D.frequency = currentLaunchSpeed;
                    m_springJoint2D.enabled = true;
                    break;
                case LaunchType.Transform_Launch:
                    m_rigidbody.gravityScale = 0; // Désactiver la gravité pendant le grappin
                    m_rigidbody.velocity = Vector2.zero;
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }
}
