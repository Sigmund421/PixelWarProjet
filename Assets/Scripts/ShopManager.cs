using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public GameObject shopUI;
    public PlayerEconomy playerEconomy;
    public GameObject[] weaponPrefabs; 
    public int[] weaponPrices;
    public Transform primaryWeaponSlot;
    public Transform secondaryWeaponSlot;
    public GameObject player;
    public Image crosshairImage; 

    public List<GameObject> weapons; // La liste des prefabs
    private List<bool> purchasedWeapons; // La liste des armes achetées

    private bool isShopOpen = false;
    private Dictionary<int, Vector2> initialWeaponPositions = new Dictionary<int, Vector2>();

    void Start()
    {
        purchasedWeapons = new List<bool>(new bool[weaponPrefabs.Length]);
        
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        
        shopUI.SetActive(false);

        // Stocke les positions initiales des icônes d'armes
        WeaponIcon[] weaponIcons = shopUI.GetComponentsInChildren<WeaponIcon>();
        foreach (WeaponIcon icon in weaponIcons)
        {
            initialWeaponPositions[icon.weaponIndex] = icon.GetComponent<RectTransform>().anchoredPosition;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopUI.SetActive(isShopOpen);

        if (isShopOpen)
        {
            
            Cursor.visible = true;
        }
        
    }

    public bool IsShopOpen()
    {
        return isShopOpen;
    }

    public void BuyWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= purchasedWeapons.Count)
        {
            Debug.LogError("Invalid weapon index: " + weaponIndex);
            return;
        }

        if (!purchasedWeapons[weaponIndex] && playerEconomy.SpendMoney(weaponPrices[weaponIndex]))
        {
            // Marque l'arme comme achetée
            purchasedWeapons[weaponIndex] = true;
            Debug.Log("Bought weapon: " + weaponPrefabs[weaponIndex].name);
        }
        else
        {
            Debug.Log("Not enough money to buy weapon or weapon already purchased: " + weaponPrefabs[weaponIndex].name);
        }
    }

    public bool OnDropWeapon(WeaponIcon weaponIcon)
    {
        if (!IsShopOpen() || !HasPurchasedWeapon(weaponIcon.weaponIndex))
            return false;

        RectTransform slotTransform = null;
        WeaponSlot slot = null;

        if (RectTransformUtility.RectangleContainsScreenPoint(primaryWeaponSlot.GetComponent<RectTransform>(), Input.mousePosition))
        {
            slotTransform = primaryWeaponSlot.GetComponent<RectTransform>();
            slot = primaryWeaponSlot.GetComponent<WeaponSlot>();
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(secondaryWeaponSlot.GetComponent<RectTransform>(), Input.mousePosition))
        {
            slotTransform = secondaryWeaponSlot.GetComponent<RectTransform>();
            slot = secondaryWeaponSlot.GetComponent<WeaponSlot>();
        }

        if (slotTransform != null && slot != null)
        {
            slot.OnDrop(new PointerEventData(EventSystem.current) { pointerDrag = weaponIcon.gameObject });
            return true;
        }

        // Reviens à la position initiale si on a drop a côté
        weaponIcon.ReturnToOriginalPosition();
        return false;
    }

    public void EquipWeapon(int slotIndex, GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon prefab is null!");
            return;
        }

        PlayerWeaponManager weaponManager = FindObjectOfType<PlayerWeaponManager>();
        if (weaponManager != null)
        {
            // Vérifie si l'arme à équiper est un prefab ou une instance
            GameObject weaponInstance = weaponPrefab;

            if (slotIndex == 1)
            {
                weaponManager.SetPrimaryWeapon(weaponInstance);
            }
            else if (slotIndex == 2)
            {
                weaponManager.SetSecondaryWeapon(weaponInstance);
            }
        }
    }


    public bool HasPurchasedWeapon(int index)
    {
        return index >= 0 && index < purchasedWeapons.Count && purchasedWeapons[index];
    }


    public GameObject GetPurchasedWeapon(int index)
    {
        if (index >= 0 && index < weaponPrefabs.Length)
        {
            return weaponPrefabs[index];
        }
        return null;
    }

    // Remet l'icône à sa position actuelle quand c'est necessaire
    public void ResetWeaponIconPosition(WeaponIcon weaponIcon)
    {
        if (initialWeaponPositions.TryGetValue(weaponIcon.weaponIndex, out Vector2 initialPosition))
        {
            weaponIcon.transform.SetParent(weaponIcon.originalParent); // Restaure le parent d'origine
            weaponIcon.GetComponent<RectTransform>().anchoredPosition = initialPosition; // Restaure la position d'origine
        }
    }

}
