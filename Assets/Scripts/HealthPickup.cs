using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 20f;
    // Donne de la vie au joueur qui le récupère
    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthSystem healthSystem = other.GetComponent<HealthSystem>();
        if (healthSystem != null && healthSystem.GetCurrentHealth() < healthSystem.maxHealth)
        {
            healthSystem.Heal(healAmount);
            Destroy(gameObject); 
        }
    }
}
