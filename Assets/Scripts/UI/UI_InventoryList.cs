using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryList : MonoBehaviour
{
    [SerializeField] private Transform m_parentInventory;
    [SerializeField] private UI_InventoryItem m_itemInventory;

    Dictionary<int, UI_InventoryItem> m_dicInventoryItem = new Dictionary<int, UI_InventoryItem>();

    public Action<int> OnUseInventoryItem;

    public void RefreshStock(InventoryItem _item)
    {
        if (_item.stock <= 0 && m_dicInventoryItem[_item.productid].gameObject.activeInHierarchy)
            m_dicInventoryItem[_item.productid].gameObject.SetActive(false);

        if (_item.stock > 0 && !m_dicInventoryItem[_item.productid].gameObject.activeInHierarchy)
            m_dicInventoryItem[_item.productid].gameObject.SetActive(true);

        m_dicInventoryItem[_item.productid].Refresh(_item.stock);
    }

    public void Initialize(InventoryViewData _item)
    {
        var item = Instantiate(m_itemInventory, m_parentInventory);

        m_dicInventoryItem.Add(_item.productId, item);

        item.OnClickInvenItem += UseInventoryItem;
        item.Refresh(_item);

        
    }

    private void UseInventoryItem(int _id)
    {
        OnUseInventoryItem?.Invoke(_id);
    }


    public bool HasItem(int _id)
    {
        return m_dicInventoryItem.ContainsKey(_id);
    }

}
