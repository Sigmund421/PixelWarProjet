using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Range(1, 200)]
    [SerializeField] private float initialSpeed = 10f; // Vitesse initiale du missile
    [Range(1, 200)]
    [SerializeField] private float maxSpeed = 50f; // Vitesse maximale du missile
    [SerializeField] private float acceleration = 5f; // Accélération du missile
    [SerializeField] private float lifeTime = 5f; // Durée de vie du missile
    [SerializeField] private float damageAmount = 20f; // Quantité de dégâts infligés
    [SerializeField] private float explosionDamage = 10f;
    [SerializeField] private GameObject explosionEffect; // Effet d'explosion
    [SerializeField] private float splashRange = 0f; // Portée de l'explosion
    [SerializeField] private float maxRange = 10f; // Maximum range of the bullet
    protected Vector3 startPosition;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Start()
    {
        rb.velocity = transform.up * initialSpeed; // Définir la vitesse initiale du missile
        Destroy(gameObject, lifeTime); // Détruire le missile après un certain temps
    }

    private void FixedUpdate()
    {
        // Accélérer le missile jusqu'à atteindre la vitesse maximale
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.velocity += (Vector2)transform.up * acceleration * Time.deltaTime;
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed; // Limiter la vitesse maximale
            }
        }

        // Check the distance traveled
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            DestroyGameObject();
        }

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
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
            ShieldSystem shieldSystem = hitCollider.GetComponent<ShieldSystem>();
            HealthSystem healthSystem = hitCollider.GetComponent<HealthSystem>();

            if (shieldSystem != null || healthSystem != null)
            {
                var closestPoint = hitCollider.ClosestPoint(transform.position);
                var distance = Vector3.Distance(closestPoint, transform.position);
                var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                var finalDamage = explosionDamage * damagePercent;

                if (shieldSystem.GetCurrentShield() > 0)
                {
                    shieldSystem.TakeShieldDamage(finalDamage);
                }
                else
                {
                    healthSystem.TakeDamage(finalDamage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (splashRange > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, splashRange);
        }
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
