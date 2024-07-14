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
            Destroy(gameObject); // D�truire l'objet de r�cup�ration apr�s l'utilisation
        }
    }
}
