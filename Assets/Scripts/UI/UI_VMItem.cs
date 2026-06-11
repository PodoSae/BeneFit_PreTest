using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_VMItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_textName;
    [SerializeField] private TextMeshProUGUI m_textPrice;
    [SerializeField] private TextMeshProUGUI m_textStock;
    [SerializeField] private Image m_imgVM;

    [SerializeField] private Button m_buttonVMItem;

    public Action<int> OnClickVMItem;

    public void Initialize(int _id, string _name,  int _price , int _stock)
    {
        m_textName.text = _name;
        m_textPrice.text = string.Format("{0} Won", _price);
        m_textStock.text = string.Format("{0} ea", _stock);

        m_buttonVMItem.onClick.AddListener(() => OnClickVMItem?.Invoke(_id));
    }
    
    public void InitImage(Sprite _sprite)
    {
        m_imgVM.sprite = _sprite;
    }

    public void RefreshStock(int stock)
    {
        m_textStock.text = string.Format("{0} ea", stock);
    }
}
