using System;
using System.Collections.Generic;

#region Vm 
[Serializable]
public class ProductData
{
    public int id;
    public string name;
    public int price;
    public int stock;
    public string type;
    public string imageUrl;
}

[Serializable]
public class VendingMachineData
{
    public string machineId;
    public string status;
    public string updatedAt;
    public List<ProductData> products;
}
#endregion

#region User
[Serializable]
public class UserData
{
    public int money;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
}

[Serializable]
public class InventoryItem
{
    public int productid;
    public int stock;
}
#endregion

