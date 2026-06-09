using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_VMItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_textName;
    [SerializeField] private TextMeshProUGUI m_textPrice;
    [SerializeField] private TextMeshProUGUI m_textStock;
    [SerializeField] private Image m_imgVM;

    public void Initialize(ProductData _product)
    {
        m_textName.text = _product.name;
        m_textPrice.text = string.Format("{0} Won", _product.price);
        m_textStock.text = string.Format("{0} ea", _product.stock);

        m_imgVM.sprite = Resources.Load<Sprite>(_product.name);

    }
}
