using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex; // 1 for primary, 2 for secondary
    private WeaponIcon currentWeaponIcon; // Reference to the current weapon icon in the slot

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
                    // Return the current weapon icon to its original position in the shop UI
                    shopManager.ResetWeaponIconPosition(currentWeaponIcon);
                    currentWeaponIcon.isInSlot = false; // Désactiver l'état dans le slot pour l'icône remplacée
                }

                // Set the new weapon icon as the current one
                currentWeaponIcon = weaponIcon;
                weaponIcon.transform.SetParent(transform); // Set the parent to this slot
                weaponIcon.transform.position = GetComponent<RectTransform>().position;
                weaponIcon.SetOriginalPosition(weaponIcon.GetComponent<RectTransform>().anchoredPosition); // Update original position
                weaponIcon.isInSlot = true; // Activer l'état dans le slot pour la nouvelle icône

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
                currentWeaponIcon.isInSlot = false; // Désactiver l'état dans le slot
            }
            currentWeaponIcon = null;
        }
    }
}
