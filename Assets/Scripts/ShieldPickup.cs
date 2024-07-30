using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    public float shieldAmount = 15f;

    //Recharge le bouclier quand on passe dessus
    private void OnTriggerEnter2D(Collider2D other)
    {
        ShieldSystem shieldSystem = other.GetComponent<ShieldSystem>();
        if (shieldSystem != null && shieldSystem.GetCurrentShield() < shieldSystem.maxShield)
        {
            shieldSystem.RechargeShield(shieldAmount);
            Destroy(gameObject);
        }
    }
}
