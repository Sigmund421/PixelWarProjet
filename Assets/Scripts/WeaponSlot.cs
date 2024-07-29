using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex; // 1 pour premier, 2 pour second
    private WeaponIcon currentWeaponIcon; // Reference � l'icone d'arme actuelle du slot

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
                    // Retour de l'icone d'arme actuelle � son emplacement de d�part
                    shopManager.ResetWeaponIconPosition(currentWeaponIcon);
                    currentWeaponIcon.isInSlot = false; // D�sactive l'�tat dans le slot pour l'ic�ne remplac�e
                }

                // Met la nouvelle ic�ne d'arme comme celle actuelle
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
                currentWeaponIcon.isInSlot = false; // D�sactive l'�tat dans le slot
            }
            currentWeaponIcon = null;
        }
    }
}
