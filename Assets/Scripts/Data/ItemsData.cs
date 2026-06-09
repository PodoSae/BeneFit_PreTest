using System;
using System.Collections.Generic;

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