using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    Dictionary<int, InventoryItem> m_dicInventoryItem = new Dictionary<int, InventoryItem>();

    public event Action<InventoryItem> OnAddItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddItem(int _id)
    {
        if (m_dicInventoryItem.ContainsKey(_id))
            ++m_dicInventoryItem[_id].stock;
        else
            m_dicInventoryItem.Add(_id, new InventoryItem() { productid = _id, stock = 1 });

        OnAddItem?.Invoke(m_dicInventoryItem[_id]);
    }

    public bool TryUseItem(int _id)
    {
        if (m_dicInventoryItem.ContainsKey(_id))
        {
            if (m_dicInventoryItem[_id].stock <= 0)
            {
                // 인벤토리 재고가 없으면
                return false;
            }
            else
            {
                --m_dicInventoryItem[_id].stock;
                // 있으면
                return true;
            }
        }
        else
            //자체를 안가지고 있음
            return false;
    }

    public int GetStock(int _id)
    {
        if (m_dicInventoryItem.ContainsKey(_id))
            return m_dicInventoryItem[_id].stock;
        else
            return 0;
    }

}

