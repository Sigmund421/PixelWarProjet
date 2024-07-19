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

    private bool isShopOpen = false;
    private Dictionary<int, GameObject> purchasedWeapons = new Dictionary<int, GameObject>();

    void Start()
    {
        // Si non assign� dans l'inspecteur, trouver le joueur par tag
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        // Cacher l'UI du shop au d�but
        shopUI.SetActive(false);
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
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // Cacher le curseur
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public bool IsShopOpen()
    {
        return isShopOpen;
    }

    public void BuyWeapon(int weaponIndex)
    {
        if (!purchasedWeapons.ContainsKey(weaponIndex) && playerEconomy.SpendMoney(weaponPrices[weaponIndex]))
        {
            // Ajouter l'arme au dictionnaire des armes achet�es
            purchasedWeapons.Add(weaponIndex, weaponPrefabs[weaponIndex]);
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

        if (RectTransformUtility.RectangleContainsScreenPoint(primaryWeaponSlot.GetComponent<RectTransform>(), Input.mousePosition))
        {
            slotTransform = primaryWeaponSlot.GetComponent<RectTransform>();
            EquipWeapon(1, purchasedWeapons[weaponIcon.weaponIndex]);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(secondaryWeaponSlot.GetComponent<RectTransform>(), Input.mousePosition))
        {
            slotTransform = secondaryWeaponSlot.GetComponent<RectTransform>();
            EquipWeapon(2, purchasedWeapons[weaponIcon.weaponIndex]);
        }

        if (slotTransform != null)
        {
            weaponIcon.transform.position = slotTransform.position;
            return true;
        }

        return false;
    }

    public void EquipWeapon(int slot, GameObject weapon)
    {
        if (slot == 1)
        {
            if (primaryWeaponSlot.childCount > 0)
                Destroy(primaryWeaponSlot.GetChild(0).gameObject);
            Instantiate(weapon, primaryWeaponSlot);
            player.GetComponent<PlayerWeaponManager>().SetPrimaryWeapon(weapon);
        }
        else if (slot == 2)
        {
            if (secondaryWeaponSlot.childCount > 0)
                Destroy(secondaryWeaponSlot.GetChild(0).gameObject);
            Instantiate(weapon, secondaryWeaponSlot);
            player.GetComponent<PlayerWeaponManager>().SetSecondaryWeapon(weapon);
        }
    }

    public bool HasPurchasedWeapon(int weaponIndex)
    {
        return purchasedWeapons.ContainsKey(weaponIndex);
    }

    public GameObject GetPurchasedWeapon(int weaponIndex)
    {
        if (purchasedWeapons.TryGetValue(weaponIndex, out GameObject weapon))
        {
            return weapon;
        }
        return null;
    }
}
