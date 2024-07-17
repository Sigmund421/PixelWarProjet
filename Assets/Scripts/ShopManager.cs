using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour, IDropHandler
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
        // Si non assigné dans l'inspecteur, trouver le joueur par tag
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        // Cacher l'UI du shop au début
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
            // Désactiver les actions du joueur et afficher le curseur
            player.GetComponent<PlayerController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            crosshairImage.enabled = false;
        }
        else
        {
            // Activer les actions du joueur et cacher le curseur
            player.GetComponent<PlayerController>().enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            crosshairImage.enabled = true;
        }
    }

    public void BuyWeapon(int weaponIndex)
    {
        if (playerEconomy.SpendMoney(weaponPrices[weaponIndex]))
        {
            // Ajouter l'arme au dictionnaire des armes achetées
            if (!purchasedWeapons.ContainsKey(weaponIndex))
            {
                purchasedWeapons.Add(weaponIndex, weaponPrefabs[weaponIndex]);
            }
            Debug.Log("Bought weapon: " + weaponPrefabs[weaponIndex].name);
        }
        else
        {
            Debug.Log("Not enough money to buy weapon: " + weaponPrefabs[weaponIndex].name);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        WeaponIcon weaponIcon = eventData.pointerDrag.GetComponent<WeaponIcon>();

        if (weaponIcon != null)
        {
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
            }
        }
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
}
