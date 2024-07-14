using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 20f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.Heal(healAmount);
            Destroy(gameObject); // D�truire l'objet de r�cup�ration apr�s l'utilisation
        }
    }
}

