using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;
    public Transform weaponHolder;
    private GameObject currentWeapon;

    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(primaryWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(secondaryWeapon);
        }
    }

    void EquipWeapon(GameObject weapon)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(weapon, weaponHolder.position, weaponHolder.rotation, weaponHolder);
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

