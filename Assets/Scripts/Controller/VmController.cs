using System;
using System.Collections.Generic;
using UnityEngine;

public class VmController : MonoBehaviour
{
    [SerializeField] private UI_VMInfo m_uiVM;
    [SerializeField] private UI_ProductList m_uiProductList;
    [SerializeField] private UI_Money m_uiMoney;
    [SerializeField] private UI_InventoryList m_uiInventory;
    [SerializeField] private UI_LogList m_uiLogList;

    #region Unity Base
    void Start()
    {
        InitLog();

        ApplyUserData();

        InitInfo();
        InitProductList();
        InitMoney();
        InitInventory();
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnAddItem -= RefreshInventory;
        LogManager.Instance.OnAddLog -= m_uiLogList.AddLog;
        m_uiInventory.OnUseInventoryItem -= UseInventory;
        m_uiMoney.OnClickAddMoney -= AddMoney;
        m_uiProductList.OnSelectProduct -= BuyProduct;
        MoneyManager.Instance.OnMoneyChanged -= m_uiMoney.RefreshUI;
    }

    private void ApplyUserData()
    {
        UserData userData = DataManager.Instance.UserData;

        if (userData == null)
            return;

        MoneyManager.Instance.SetMoney(userData.money);
        InventoryManager.Instance.SetInventoryItems(userData.inventoryItems);
    }

    #endregion


    #region Info
    private void InitInfo()
    {
        InfoViewData data = new InfoViewData() { machineId = DataManager.Instance.ItemData.machineId, isActive = DataManager.Instance.IsActive };

        m_uiVM.Initialize(data);

        if (!DataManager.Instance.IsActive)
            LogManager.Instance.AddLog("Vending Machine is InActive", LogState.Error);
    }
    #endregion

    #region ProductList
    private void InitProductList()
    {
        List<ProductViewData> m_listProductViewData = new List<ProductViewData>();

        // Product Init
        foreach (ProductData data in DataManager.Instance.ItemData.products)
        {
            Sprite itemImg = DataManager.Instance.LoadImg(data.imageUrl);

            if (itemImg == null)
                itemImg = DataManager.Instance.LoadImg(data.name);

            m_listProductViewData.Add(new ProductViewData() { productId = data.id, productName = data.name, productPrice = data.price, productStock = data.stock, productSprite = itemImg });
        }

        m_uiProductList.Initialize(m_listProductViewData);
        m_uiProductList.OnSelectProduct += BuyProduct;
    }

    private void BuyProduct(int _productId)
    {
        if (!DataManager.Instance.IsActive)
        {
            LogManager.Instance.AddLog("Action blocked - Machine is inactive", LogState.Error);
            return;
        }

        ProductData data = DataManager.Instance.GetProduct(_productId);

        if (data == null)
        {
            // 상품이 존재하지 않음
            LogManager.Instance.AddLog(string.Format("Buy Fail - Has no Product. Id: {0}", _productId), LogState.Error);
            return;
        }


        if (data.stock <= 0)
        {
            // 재고가 있는지 확인
            LogManager.Instance.AddLog(string.Format("Buy Fail - Has no Product Stock. Id: {0}", _productId), LogState.Error);
            return;
        }

        if (MoneyManager.Instance.TrySpend(data.price))
        {   
            // 재고 감소  - 저장
            if (DataManager.Instance.DecreaseStock(data.id))
            {
                m_uiProductList.RefreshStock(data.id, data.stock);
                
                // inventory 추가
                InventoryManager.Instance.AddItem(data.id);
                SaveMoney();
                SaveInventory();
                DataManager.Instance.SaveItemsData();

                // 구매 성공
                LogManager.Instance.AddLog(string.Format("Buy Success. Id: {0}", _productId));
            }
        }
        else
        {
            // 금액 불가로 구매 실패
            LogManager.Instance.AddLog(string.Format("Buy Fail - Has no Money. Id: {0}", _productId), LogState.Error);
        }

    }
    #endregion

    #region Money
    private void InitMoney()
    {
        m_uiMoney.OnClickAddMoney += AddMoney;
        MoneyManager.Instance.OnMoneyChanged += m_uiMoney.RefreshUI;

        m_uiMoney.RefreshUI(MoneyManager.Instance.CurrentMoney);
    }

    private void AddMoney(int _amount)
    {
        if (!DataManager.Instance.IsActive)
        {
            LogManager.Instance.AddLog("Action blocked - Machine is inactive", LogState.Error);
            return;
        }

        // read only money 를 받아와서 충전량 log 출력

        int before = MoneyManager.Instance.CurrentMoney;

        MoneyManager.Instance.AddMoney(_amount);

        int gap = MoneyManager.Instance.CurrentMoney - before;

        if (gap > 0)
        { 
            LogManager.Instance.AddLog(string.Format("Add Money Success : {0} won", gap));
            SaveMoney();
        }
        else
            LogManager.Instance.AddLog("Add Money Fail - Max Money", LogState.Error);
    }

    #endregion

    #region Inventory
    private void InitInventory()
    {
        InventoryManager.Instance.OnAddItem += RefreshInventory;
        m_uiInventory.OnUseInventoryItem += UseInventory;

        foreach (InventoryItem item in InventoryManager.Instance.GetAllInventoryItems())
        {
            RefreshInventory(item);
        }
    }

    private void RefreshInventory(InventoryItem _item)
    {
        if (!DataManager.Instance.IsActive)
        {
            LogManager.Instance.AddLog("Action blocked - Machine is inactive", LogState.Error);
            return;
        }

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

        SaveInventory();
    }

    private void UseInventory(int _id)
    {
        if (!DataManager.Instance.IsActive)
        {
            LogManager.Instance.AddLog("Action blocked - Machine is inactive", LogState.Error);
            return;
        }

        if (InventoryManager.Instance.TryUseItem(_id))
        {
            // 아이템 사용 됨 ui refresh
            InventoryItem item = new InventoryItem() { productid = _id, stock = InventoryManager.Instance.GetStock(_id) };

            m_uiInventory.RefreshStock(item);

            SaveInventory();
            LogManager.Instance.AddLog(string.Format("Use Inventory Item. Id: {0}", _id));
        }
        else
        {
            // 아이템 사용 안됨 ui refresh
        }
    }

    public void SaveInventory()
    {
        List<InventoryItem> items = InventoryManager.Instance.GetAllInventoryItems();
        DataManager.Instance.SetUserData_Inventory(items);
        DataManager.Instance.SaveUserData();
    }

    public void SaveMoney()
    {
        int money = MoneyManager.Instance.CurrentMoney;
        DataManager.Instance.SetUserData_Money(money);
        DataManager.Instance.SaveUserData();
    }
    
    #endregion

    #region Log
    private void InitLog()
    {
        LogManager.Instance.OnAddLog += m_uiLogList.AddLog;
    }
    #endregion
}

[Serializable]
public struct InfoViewData
{
    public string machineId;
    public bool isActive;
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
