using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Money : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_textCurrentMoney;
    [SerializeField] private Button m_buttonEarn100;
    [SerializeField] private Button m_buttonEarn1000;

    public event Action<int> OnClickAddMoney;

    private void Awake()
    {
        m_buttonEarn100.onClick.AddListener(() => OnClickAddMoney?.Invoke(100));
        m_buttonEarn1000.onClick.AddListener(() => OnClickAddMoney?.Invoke(1000));
    }


    public void RefreshUI(int _currentMoney)
    {
        m_textCurrentMoney.text = string.Format("money : {0} won", _currentMoney);
    }

}
