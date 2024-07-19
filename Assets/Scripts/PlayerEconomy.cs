using UnityEngine;
using UnityEngine.UI;

public class PlayerEconomy : MonoBehaviour
{
    public int money = 0;
    public Text moneyText;

    void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    void UpdateMoneyUI()
    {
        moneyText.text = "Money: " + money.ToString();
    }
}
