using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_VMInfo : MonoBehaviour
{
    [Header("Top")]
    [SerializeField] private TextMeshProUGUI m_idText;
    [SerializeField] private Image powerLight;

    public void Initialize(InfoViewData _data)
    {
        m_idText.text = _data.machineId.ToString();

        powerLight.color = _data.isActive
                ? Color.green
                : Color.red;       
    }
}
