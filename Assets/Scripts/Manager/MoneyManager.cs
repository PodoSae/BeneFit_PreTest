using System;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int CurrentMoney { get; private set; }

    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddMoney(int amount)
    {
        CurrentMoney += amount;

        CurrentMoney = Mathf.Min(CurrentMoney, 10000);

        OnMoneyChanged?.Invoke(CurrentMoney);
    }

    public bool TrySpend(int amount)
    {
        if (CurrentMoney < amount)
            return false;

        CurrentMoney -= amount;

        OnMoneyChanged?.Invoke(CurrentMoney);
        return true;
    }
}
