using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Range(1, 200)]
    [SerializeField] private float initialSpeed = 10f; // Vitesse initiale du missile
    [Range(1, 200)]
    [SerializeField] private float maxSpeed = 50f; // Vitesse maximale du missile
    [SerializeField] private float acceleration = 5f; // Acc�l�ration du missile
    [SerializeField] private float lifeTime = 5f; // Dur�e de vie du missile
    [SerializeField] private float damageAmount = 20f; // Quantit� de d�g�ts inflig�s

    [SerializeField] private GameObject explosionEffect; // Effet d'explosion
    [SerializeField] private float splashRange = 0f; // Port�e de l'explosion

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // D�finir le Rigidbody2D comme Kinematic
    }

    private void Start()
    {
        rb.velocity = transform.up * initialSpeed; // D�finir la vitesse initiale du missile
        Destroy(gameObject, lifeTime); // D�truire le missile apr�s un certain temps
    }

    private void FixedUpdate()
    {
        // Acc�l�rer le missile jusqu'� atteindre la vitesse maximale
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.velocity += (Vector2)transform.up * acceleration * Time.deltaTime;
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed; // Limiter la vitesse maximale
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Grappable"))
        {
            DestroyGameObject();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // V�rifier que le missile ne touche pas des objets � ignorer (comme les Pickups)
        if (other.gameObject.layer != LayerMask.NameToLayer("Pickup"))
        {
            // V�rifier si l'objet touch� est dans la couche "Grappable"
            if (other.gameObject.layer == LayerMask.NameToLayer("Grappable"))
            {
                DestroyGameObject();
                return; // Exit early if we don't need to do anything else
            }

            ApplyDamage(other);

            if (splashRange > 0)
            {
                Explode();
            }

            DestroyGameObject();
        }
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
                    var finalDamage = damageAmount * damagePercent;

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
