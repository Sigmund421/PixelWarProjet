using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    [Range(1, 200)]
    [SerializeField] private float speed = 80f;

    [Range(1, 100)]
    [SerializeField] private float lifeTime = 3f;

    [SerializeField] private float explosionDamage = 20f;
    [SerializeField] private float splashRange = 0f;
    [SerializeField] private GameObject explosionEffect; // Effet d'explosion

    [SerializeField] private float maxRange = 10f; // Maximum range of the bullet
    [SerializeField] private LayerMask ignoreLayer; // LayerMask to ignore specific layers

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int maxBounces = 2; // Maximum number of bounces

    private Vector2 direction;

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private int bounceCount = 0; // Counter for the number of bounces
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float fadeTime = 0.25f; // Time for the line renderer to fade out (adjusted for faster fade out)
    [SerializeField] private float destroyDelay = 0.3f; // Delay before destroying the bullet to allow the line renderer to fade out
    [SerializeField] private float startFadeTime = 0.5f; // Time to wait before starting the fade out

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        
        startPosition = transform.position; // Save the starting position
        Destroy(gameObject, lifeTime + destroyDelay);
        

        // Initialize the line renderer positions
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            lineRenderer.enabled = true; // Enable the line renderer
        }

        // Start the coroutine to fade out the line renderer after a delay
        StartCoroutine(StartFadeOutAfterDelay());
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;

        // Update the line renderer to follow the bullet
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, startPosition);
        }

        // Check the distance traveled
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            StartCoroutine(FadeOutLineRenderer());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        ApplyDamage(collision.collider);

        if (splashRange > 0)
        {
            Explode();
        }

        DestroyGameObject();

    }


    private void ApplyDamage(Collider2D other)
    {
        ShieldSystem shieldSystem = other.GetComponent<ShieldSystem>();
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();

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

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        var hitColliders = Physics2D.OverlapCircleAll(transform.position, splashRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null)
            {
                ShieldSystem shieldSystem = hitCollider.GetComponent<ShieldSystem>();
                HealthSystem healthSystem = hitCollider.GetComponent<HealthSystem>();

                if (shieldSystem != null || healthSystem != null)
                {
                    var closestPoint = hitCollider.ClosestPoint(transform.position);
                    var distance = Vector3.Distance(closestPoint, transform.position);
                    var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                    var finalDamage = explosionDamage * damagePercent;

                    if (shieldSystem != null)
                    {
                        shieldSystem.TakeShieldDamage(finalDamage);
                    }

                    if (healthSystem != null)
                    {
                        healthSystem.TakeDamage(finalDamage);
                    }
                }
            }
        }
    }

    private IEnumerator StartFadeOutAfterDelay()
    {
        yield return new WaitForSeconds(startFadeTime);
        StartCoroutine(FadeOutLineRenderer());
    }

    private IEnumerator FadeOutLineRenderer()
    {
        Vector3 startPos = lineRenderer.GetPosition(1);
        Vector3 endPos = lineRenderer.GetPosition(0);

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float progress = t / fadeTime;
            Vector3 currentEndPos = Vector3.Lerp(startPos, endPos, progress);
            lineRenderer.SetPosition(1, currentEndPos);
            yield return null;
        }

        // Destroy the game object after the line renderer has faded out
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (splashRange > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, splashRange);
        }
    }

    public void Shoot(Vector2 direction)
    {
        this.direction = direction;
        
        
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);

    }
}
