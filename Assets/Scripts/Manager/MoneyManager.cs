using System;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    private int m_currentMoney;
    public int CurrentMoney => m_currentMoney;

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
        m_currentMoney += amount;

        m_currentMoney = Mathf.Min(m_currentMoney, 10000);

        OnMoneyChanged?.Invoke(m_currentMoney);
    }

    public bool TrySpend(int amount)
    {
        if (m_currentMoney < amount)
            return false;

        m_currentMoney -= amount;

        OnMoneyChanged?.Invoke(m_currentMoney);
        return true;
    }
}
