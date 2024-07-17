using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    public int amount = 5;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerEconomy playerEconomy = other.GetComponent<PlayerEconomy>();
            if (playerEconomy != null)
            {
                playerEconomy.AddMoney(amount);
                Destroy(gameObject);
            }
        }
    }
}
