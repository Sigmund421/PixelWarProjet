using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Range(1, 200)]
    [SerializeField] private float speed = 80f;

    [Range(1, 100)]
    [SerializeField] private float lifeTime = 3f;

    [SerializeField] private float explosionDamage = 20f;
    [SerializeField] private float splashRange = 0f;
    [SerializeField] private GameObject explosionEffect; // Effet d'explosion

    [SerializeField] private float maxRange = 10f; // Range max

    protected Rigidbody2D rb;
    public float damageAmount = 20f;

    protected Vector3 startPosition;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; // Save the starting position
        Destroy(gameObject, lifeTime);
    }

    protected virtual void FixedUpdate()
    {
        rb.velocity = transform.up * speed; //Tir propulsion

        // Regarde la distance parcourue
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

    protected void ApplyDamage(Collider2D other) //Dégâts, d'abord au bouclier et après à la vie si le bouclier est détruit
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

    private void Explode() // TOUTES LES EXPLOSIONS DANS LES SCRIPTS ont la même logique, Faire + de dégâts au centre qu'a l'extérieur
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

    protected void DestroyGameObject()
    {
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
}
