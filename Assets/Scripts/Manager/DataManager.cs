using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private const string ITEM_DATA_FILE_NAME = "Items.json";
    private const string USER_DATA_FILE_NAME = "UserData.json";

    private string ItemDataPath
    {
        get { return Path.Combine(Application.persistentDataPath, ITEM_DATA_FILE_NAME); }
    }

    private string UserDataPath
    {
        get { return Path.Combine(Application.persistentDataPath, USER_DATA_FILE_NAME); }
    }

    private VendingMachineData m_itemsData;
    private UserData m_userData;

    private bool m_isActive;

    public VendingMachineData ItemData => m_itemsData;
    public UserData UserData => m_userData;
    public bool IsActive => m_isActive;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadData();
    }

    #region Load & Save

    private void LoadData()
    {
        string jsonitems = LoadJsonItems();

        if (string.IsNullOrEmpty(jsonitems))
            return;

        ParseItemsData(jsonitems);

        string jsonUser = LoadUserDatas();

        ParseUserData(jsonUser);
    }


    private string LoadJsonItems()
    {
        //ItemDataPath 우선 사용, 파일 없을 시 기본 Items 사용

        if (File.Exists(ItemDataPath))
        {
            Debug.Log("Use Persistent Items Data");
            return File.ReadAllText(ItemDataPath);
        }

        TextAsset textAsset = Resources.Load<TextAsset>("Items");

        if (textAsset == null)
        {
            Debug.LogError("Resources Items.json Load Fail");
            return string.Empty;
        }

        Debug.Log("Use Resources Items Data");
        return textAsset.text;
    }

   
    private void ParseItemsData(string json)
    {
        m_itemsData = JsonUtility.FromJson<VendingMachineData>(json);

        if (m_itemsData == null)
        {
            Debug.LogError("Items.json 파싱 실패");
        }

        m_isActive = m_itemsData != null &&
            !string.IsNullOrEmpty(m_itemsData.status) &&
            m_itemsData.status.ToLower() == "active";
    }

    public void SaveItemsData()
    {
        if (m_itemsData == null)
            return;

        string json = JsonUtility.ToJson(m_itemsData, true);
        File.WriteAllText(ItemDataPath, json);

        Debug.Log("Items Save Success : " + ItemDataPath);
    }

    private string LoadUserDatas()
    {
        if (File.Exists(UserDataPath))
        {
            Debug.Log("Use Persistent Items Data");
            return File.ReadAllText(UserDataPath);
        }

        return string.Empty;
    }

    private void ParseUserData(string json)
    {
        if (json == string.Empty)
        { 
            m_userData = new UserData();
            return;
        }

        m_userData = JsonUtility.FromJson<UserData>(json);

        if (m_userData == null)
        {
            Debug.LogError("Items.json 파싱 실패");
        }
    }


    public void SaveUserData(UserData userData)
    {
        string json = JsonUtility.ToJson(userData, true);
        File.WriteAllText(UserDataPath, json);

        Debug.Log("UserData Save Success : " + UserDataPath);
    }

    public void SaveUserData()
    {

        string json = JsonUtility.ToJson(m_userData, true);
        File.WriteAllText(UserDataPath, json);

        Debug.Log("UserData Save Success : " + UserDataPath);
    }

    public Sprite LoadImg(string imagePath)
    {
        string path = Path.Combine(Application.streamingAssetsPath,imagePath);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"이미지 없음 : {path}");

            // Fallback
            return Resources.Load<Sprite>(Path.GetFileNameWithoutExtension(imagePath));
        }

        byte[] imageBytes = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(2, 2);

        if (!texture.LoadImage(imageBytes))
        {
            Debug.LogError($"이미지 로드 실패 : {path}");
            return null;
        }

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
    }
    #endregion


    #region function
    public ProductData GetProduct(int _id)
    {
        foreach (ProductData data in m_itemsData.products)
        {
            if (data.id == _id)
                return data;
        }

        Debug.LogWarning($"상품 없음: {_id}");
        return null;
    }

    public bool DecreaseStock(int _id)
    {
        foreach (ProductData data in m_itemsData.products)
        {
            if (data.id == _id)
            {
                if (data.stock > 0)
                {
                    --data.stock;
                    return true;
                }
                else
                    return false;
            }
        }
        return false;
    }

    public void SetUserData_Inventory(List<InventoryItem> _list)
    {
        m_userData.inventoryItems = _list;
    }

    public void SetUserData_Money(int _money)
    {
        m_userData.money = _money;
    }

    #endregion
}