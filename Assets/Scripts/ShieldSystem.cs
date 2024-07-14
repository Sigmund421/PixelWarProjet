using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldSystem : MonoBehaviour
{
    public float maxShield = 50f;
    private float currentShield;
    public GameObject shieldVisual; // Assurez-vous d'assigner ce GameObject dans l'inspecteur
    public Slider shieldBar; // Assurez-vous d'assigner ce Slider dans l'inspecteur

    private void Start()
    {
        currentShield = maxShield;
        UpdateShieldVisual();
        UpdateShieldBar();
    }

    public void TakeShieldDamage(float damage)
    {
        currentShield -= damage;
        if (currentShield < 0)
        {
            currentShield = 0;
        }
        UpdateShieldVisual();
        UpdateShieldBar();
    }

    public void RechargeShield(float amount)
    {
        currentShield += amount;
        if (currentShield > maxShield)
        {
            currentShield = maxShield;
        }
        UpdateShieldVisual();
        UpdateShieldBar();
    }

    private void UpdateShieldVisual()
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(currentShield > 0);
        }
    }

    private void UpdateShieldBar()
    {
        if (shieldBar != null)
        {
            shieldBar.value = currentShield;
            shieldBar.gameObject.SetActive(currentShield > 0);
        }
    }

    public float GetCurrentShield()
    {
        return currentShield;
    }
}
