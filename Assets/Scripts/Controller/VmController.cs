using System;
using System.Collections.Generic;
using UnityEngine;

public class VmController : MonoBehaviour
{
    [SerializeField] private UI_VMInfo m_uiVM;
    [SerializeField] private UI_ProductList m_uiProductList;
    [SerializeField] private UI_Money m_uiMoney;
    [SerializeField] private UI_InventoryList m_uiInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_uiVM.Initialize(DataManager.Instance.Data);
        InitProductList();
        InitMoney();
        InitInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region ProductList
    private void InitProductList()
    {
        List<ProductViewData> m_listProductViewData = new List<ProductViewData>();

        foreach (ProductData data in DataManager.Instance.Data.products)
        {
            Sprite itemImg = DataManager.Instance.LoadImg(data.imageUrl);

            if (itemImg == null)
                itemImg = DataManager.Instance.LoadImg(data.name);

            m_listProductViewData.Add(new ProductViewData() {productId = data.id, productName = data.name, productPrice = data.price, productStock = data.stock, productSprite = itemImg });
        }

        m_uiProductList.Initialize(m_listProductViewData);

        m_uiProductList.OnSelectProduct += BuyProduct;
    }

    private void BuyProduct(int _productId)
    {
        //id 가 data 에 있는지 확인
        ProductData data = DataManager.Instance.GetProduct(_productId);

        if (data == null)
        {
            //log - 상품 id 가 없다
            return;
        }

        // 재고 가 있는지 확인
        if (data.stock <= 0)
        {
            //log - 상품 재고가 없다
            return;
        }

        if (MoneyManager.Instance.TrySpend(data.price))
        {
            //log - 구매 성공
            // inventory 추가
            InventoryManager.Instance.AddItem(data.id);
            // 재고 감소  - 저장

        }
        else
        { 
            //log - 구매 실패
        }

    }
    #endregion

    #region Money
    private void InitMoney()
    {
        m_uiMoney.OnClickAddMoney += AddMoney;
        MoneyManager.Instance.OnMoneyChanged += m_uiMoney.RefreshUI;
    }

    private void AddMoney(int _amount)
    {
        MoneyManager.Instance.AddMoney(_amount);
    }

    #endregion

    #region Inventory
    private void InitInventory()
    {
        InventoryManager.Instance.OnAddItem += RefreshInventroy;

        m_uiInventory.OnUseInventoryItem += UseInventory;
    }

    private void RefreshInventroy(InventoryItem _item)
    {
        ProductData data = DataManager.Instance.GetProduct(_item.productid);

        if (m_uiInventory.HasItem(_item.productid))
        {
            m_uiInventory.RefreshStock(_item);
        }
        else
        {
            Sprite itemImg = DataManager.Instance.LoadImg(data.imageUrl);

            if (itemImg == null)
                itemImg = DataManager.Instance.LoadImg(data.name);

            InventoryViewData inventoryData = new InventoryViewData() { productId = _item.productid, productName = data.name, productStock = _item.stock, productSprite = itemImg };
            m_uiInventory.Initialize(inventoryData);
        }

    }

    private void UseInventory(int _id)
    {
        if (InventoryManager.Instance.TryUseItem(_id))
        {
            // 아이템 사용 됨 ui refresh
            InventoryItem item = new InventoryItem() { productid = _id, stock = InventoryManager.Instance.GetStock(_id) };

            m_uiInventory.RefreshStock(item);
        }
        else
        { 
            // 아이템 사용 안됨 ui refresh
        }


    }
    #endregion

}

[Serializable]
public struct ProductViewData
{
    public int productId;
    public string productName;
    public int productPrice;
    public int productStock;
    public Sprite productSprite;
}

[Serializable]
public struct InventoryViewData
{
    public int productId;
    public string productName;
    public int productStock;
    public Sprite productSprite;
}
