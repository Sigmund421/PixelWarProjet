using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBulletAwp : MonoBehaviour
{
    [Range(1, 200)]
    [SerializeField] private float speed = 80f;

    [Range(1, 100)]
    [SerializeField] private float lifeTime = 3f;

    [SerializeField] private float explosionDamage = 20f;
    [SerializeField] private float splashRange = 0f;
    [SerializeField] private GameObject explosionEffect; // Effet d'explosion

    [SerializeField] private float maxRange = 10f; // Range maxi
    [SerializeField] private LayerMask ignoreLayer; // Ignore certains layers

    [SerializeField] private LineRenderer lineRenderer;

    private Rigidbody2D rb;
    private Vector3 startPosition;

    [SerializeField] private float damageAmount = 20f;
    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();
    [SerializeField] private float fadeTime = 0.5f; // temps du fade du line renderer
    [SerializeField] private float fadeDelay = 2f; // Délai avant de commencer le fade

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startPosition = transform.position; // Sauvegarde la position actuelle
        Destroy(gameObject, lifeTime);

        
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            lineRenderer.enabled = true; 
        }

        
        StartCoroutine(FadeOutAfterDelay());
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;

        // Met à jour le line renderer pour suivre la balle
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, transform.position);
        }

        // Distance parcourue
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            DestroyGameObject();
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

    private void DestroyGameObject()
    {
        // arrête les mouvements de la balle
        rb.velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject);
    }

    private IEnumerator FadeOutAfterDelay()
    {
        // Attend le fade delay
        yield return new WaitForSeconds(fadeDelay);

        // Commence à faire le fade du line renderer
        StartCoroutine(FadeOutLineRenderer());
    }

    private IEnumerator FadeOutLineRenderer()
    {
        float startAlpha = lineRenderer.startColor.a;
        float endAlpha = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeTime);

            Color startColor = lineRenderer.startColor;
            startColor.a = newAlpha;
            lineRenderer.startColor = startColor;

            Color endColor = lineRenderer.endColor;
            endColor.a = newAlpha;
            lineRenderer.endColor = endColor;

            yield return null;
        }

        Destroy(lineRenderer.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (splashRange > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, splashRange);
        }
    }
}
