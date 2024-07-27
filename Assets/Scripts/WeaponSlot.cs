using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex; // 1 for primary, 2 for secondary

    public void OnDrop(PointerEventData eventData)
    {
        WeaponIcon weaponIcon = eventData.pointerDrag.GetComponent<WeaponIcon>();

        if (weaponIcon != null)
        {
            ShopManager shopManager = FindObjectOfType<ShopManager>();
            if (shopManager != null && shopManager.HasPurchasedWeapon(weaponIcon.weaponIndex))
            {
                shopManager.EquipWeapon(slotIndex, shopManager.GetPurchasedWeapon(weaponIcon.weaponIndex));
                weaponIcon.transform.position = GetComponent<RectTransform>().position;
            }
        }
    }
}