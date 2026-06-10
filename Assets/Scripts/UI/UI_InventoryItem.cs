using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_textItem;

    [SerializeField] private TextMeshProUGUI m_textStock;
    [SerializeField] private Image m_imgItem;

    [SerializeField] private Button m_buttonInvenItem;

    public Action<int> OnClickInvenItem;

    public void Refresh(InventoryViewData _item)
    {
        m_textItem.text = _item.productName;
        m_textStock.text = string.Format("{0} ea", _item.productStock);

        m_imgItem.sprite = _item.productSprite;

        m_buttonInvenItem.onClick.AddListener(() => OnClickInvenItem?.Invoke(_item.productId));
    }

    public void Refresh(int _stock)
    { 
        m_textStock.text = string.Format("{0} ea", _stock);
    }

}
