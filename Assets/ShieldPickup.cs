using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    public float shieldAmount = 15f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ShieldSystem shieldSystem = other.GetComponent<ShieldSystem>();
        if (shieldSystem != null)
        {
            shieldSystem.RechargeShield(shieldAmount);
            Destroy(gameObject); // Détruire l'objet de récupération après l'utilisation
        }
    }
}
