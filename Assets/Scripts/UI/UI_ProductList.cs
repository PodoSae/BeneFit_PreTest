using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_ProductList : MonoBehaviour
{
    public event Action<int> OnSelectProduct;

    [Header("Body")]
    [SerializeField] private Transform m_parentProduct;
    [SerializeField] private UI_VMItem m_itemVM;

    public void Initialize(List<ProductViewData> _listProduct)
    {
        foreach (ProductViewData viewData in _listProduct)
        {
            var item = Instantiate(m_itemVM, m_parentProduct);

            item.Initialize(viewData.productId, viewData.productName, viewData.productPrice, viewData.productStock);
            item.InitImage(viewData.productSprite);

            item.OnClickVMItem += ChooseVMItem;

        }
    }

    private void ChooseVMItem(int _id)
    {
        OnSelectProduct?.Invoke(_id);
    }
}
