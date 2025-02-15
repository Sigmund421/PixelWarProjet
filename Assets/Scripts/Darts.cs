using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darts : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Vitesse de la grenade
    [SerializeField] private float explosionDelay = 2f; // D�lai avant l'explosion
    [SerializeField] private float explosionDamage = 20f;
    [SerializeField] private float splashRange = 5f;
    [SerializeField] private GameObject explosionEffect; // Effet d'explosion
    public float damageAmount = 20f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
        StartCoroutine(ExplodeAfterDelay());
    }

    private void FixedUpdate()
    {
        // Appliquer une chute progressive
        rb.velocity += Vector2.down * Time.fixedDeltaTime;
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
        Destroy(gameObject);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRange);
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
