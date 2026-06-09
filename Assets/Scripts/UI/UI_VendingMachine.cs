using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_VendingMachine : MonoBehaviour
{
    [Header("Top")]
    [SerializeField] private TextMeshProUGUI m_idText;
    [SerializeField] private Image powerLight;

    [Header("Body")]
    [SerializeField] private Transform m_parentProduct;
    [SerializeField] private UI_VMItem m_itemVM;

    public void Initialize(VendingMachineData _data)
    {
        m_idText.text = _data.machineId;

        bool isActive = _data.status.ToLower() == "active";

        powerLight.color = isActive
                ? Color.green
                : Color.red;       

        foreach (var product in DataManager.Instance.Data.products)
        {
            var item = Instantiate(m_itemVM, m_parentProduct);

            item.Initialize(product);
        }


    }
}
