using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex; // 1 pour premier, 2 pour second
    private WeaponIcon currentWeaponIcon; // Reference à l'icone d'arme actuelle du slot

    public void OnDrop(PointerEventData eventData)
    {
        WeaponIcon weaponIcon = eventData.pointerDrag.GetComponent<WeaponIcon>();

        if (weaponIcon != null)
        {
            ShopManager shopManager = FindObjectOfType<ShopManager>();
            if (shopManager != null && shopManager.HasPurchasedWeapon(weaponIcon.weaponIndex))
            {
                if (currentWeaponIcon != null)
                {
                    // Retour de l'icone d'arme actuelle à son emplacement de départ
                    shopManager.ResetWeaponIconPosition(currentWeaponIcon);
                    currentWeaponIcon.isInSlot = false; // Désactive l'état dans le slot pour l'icône remplacée
                }

                // Met la nouvelle icône d'arme comme celle actuelle
                currentWeaponIcon = weaponIcon;
                weaponIcon.transform.SetParent(transform); 
                weaponIcon.transform.position = GetComponent<RectTransform>().position;
                weaponIcon.SetOriginalPosition(weaponIcon.GetComponent<RectTransform>().anchoredPosition); 
                weaponIcon.isInSlot = true;

                shopManager.EquipWeapon(slotIndex, shopManager.GetPurchasedWeapon(weaponIcon.weaponIndex));
            }
        }
    }

    public void ClearSlot()
    {
        if (currentWeaponIcon != null)
        {
            ShopManager shopManager = FindObjectOfType<ShopManager>();
            if (shopManager != null)
            {
                shopManager.ResetWeaponIconPosition(currentWeaponIcon);
                currentWeaponIcon.isInSlot = false; // Désactive l'état dans le slot
            }
            currentWeaponIcon = null;
        }
    }
}
