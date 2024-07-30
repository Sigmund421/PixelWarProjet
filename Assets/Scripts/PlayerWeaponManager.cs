using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponManager : MonoBehaviour
{
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;
    public Transform weaponHolder;
    private GameObject currentWeapon;

    // Référence à la barre de munitions globale dans l'UI
    public Slider globalAmmoBar;

    // défini l'arme de base avec laquelle le joueur commence
    void Start()
    {
        if (primaryWeapon != null)
        {
            EquipWeapon(primaryWeapon);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && primaryWeapon != null)
        {
            EquipWeapon(primaryWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && secondaryWeapon != null)
        {
            EquipWeapon(secondaryWeapon);
        }
    }

    void EquipWeapon(GameObject weapon)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weapon, weaponHolder.position, weaponHolder.rotation, weaponHolder);

        // Vérifie et configure la barre de munitions globale
        if (currentWeapon.GetComponent<GunBase>() != null)
        {
            GunBase.SetGlobalAmmoBar(globalAmmoBar);
            currentWeapon.GetComponent<GunBase>().Equip();
        }
        else if (currentWeapon.GetComponent<Flamethrower>() != null)
        {
            Flamethrower.SetGlobalAmmoBar(globalAmmoBar);
        }
        else if (currentWeapon.GetComponent<Laser_Tutorial>() != null)
        {
            Laser_Tutorial.SetGlobalAmmoBar(globalAmmoBar);
        }
        else if (currentWeapon.GetComponent<LaserGun>() != null)
        {
            LaserGun.SetGlobalAmmoBar(globalAmmoBar);
        }
        else if (currentWeapon.GetComponent<Minigun>() != null)
        {
            Minigun.SetGlobalAmmoBar(globalAmmoBar);
        }
        else if (currentWeapon.GetComponent<SemiAutoGun>() != null)
        {
            SemiAutoGun.SetGlobalAmmoBar(globalAmmoBar);
        }
    }

    public void SetPrimaryWeapon(GameObject weapon)
    {
        primaryWeapon = weapon;
        EquipWeapon(primaryWeapon);
    }

    public void SetSecondaryWeapon(GameObject weapon)
    {
        secondaryWeapon = weapon;
    }
}
