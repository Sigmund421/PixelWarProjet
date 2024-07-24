using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Vitesse de la grenade
    [SerializeField] private float explosionDelay = 2f; // Délai avant l'explosion
    [SerializeField] private float explosionDamage = 20f;
    [SerializeField] private float splashRange = 5f;
    [SerializeField] private GameObject explosionEffect; // Effet d'explosion

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Pickup"))
        {
            // Vérifier si l'objet collisionné a le layer "Grappable"
            if (other.gameObject.layer == LayerMask.NameToLayer("Grappable"))
            {
                Explode();
                Destroy(gameObject);
                return;
            }

            Explode();
            Destroy(gameObject);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRange);
    }
}
