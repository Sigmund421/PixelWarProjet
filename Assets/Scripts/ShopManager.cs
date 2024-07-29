using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public GameObject shopUI;
    public PlayerEconomy playerEconomy;
    public GameObject[] weaponPrefabs; // Assigner vos prefabs d'armes ici
    public int[] weaponPrices;
    public Transform primaryWeaponSlot;
    public Transform secondaryWeaponSlot;
    public GameObject player;
    public Image crosshairImage; // Assigner l'image du crosshair dans l'inspecteur

    public List<GameObject> weapons; // List of weapon prefabs
    private List<bool> purchasedWeapons; // List of purchased weapons

    private bool isShopOpen = false;
    private Dictionary<int, Vector2> initialWeaponPositions = new Dictionary<int, Vector2>();

    void Start()
    {
        purchasedWeapons = new List<bool>(new bool[weaponPrefabs.Length]);
        // Si non assigné dans l'inspecteur, trouver le joueur par tag
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        // Cacher l'UI du shop au début
        shopUI.SetActive(false);

        // Stocker les positions initiales des icônes d'armes
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
            // Afficher le curseur
            Cursor.visible = true;
        }
        else
        {
            // Cacher le curseur
            Cursor.visible = false;
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
            // Marquer l'arme comme achetée
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

        // Revenir à la position initiale si le drop n'a pas été réussi
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
            // Ici, nous vérifions si l'arme à équiper est un prefab ou une instance
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


    public void ResetWeaponIconPosition(WeaponIcon weaponIcon)
    {
        if (initialWeaponPositions.TryGetValue(weaponIcon.weaponIndex, out Vector2 initialPosition))
        {
            weaponIcon.transform.SetParent(weaponIcon.originalParent); // Restaurer le parent d'origine
            weaponIcon.GetComponent<RectTransform>().anchoredPosition = initialPosition; // Restaurer la position d'origine
        }
    }

}
