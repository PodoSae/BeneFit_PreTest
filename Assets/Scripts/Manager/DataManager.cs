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

    private static readonly string[] IMAGE_EXTENSIONS =
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".webp"
    };

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

    // Load & Save itemsData
    #region ItemsData
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

        m_itemsData.updatedAt = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        string json = JsonUtility.ToJson(m_itemsData, true);
        File.WriteAllText(ItemDataPath, json);

        Debug.Log("Items Save Success : " + ItemDataPath);
    }
    #endregion

    // Load & Save userData
    #region UserData
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
        if (string.IsNullOrWhiteSpace(json))
        {
            m_userData = new UserData();
            return;
        }

        m_userData = JsonUtility.FromJson<UserData>(json);

        if (m_userData == null)
        {
            Debug.LogError("Items.json 파싱 실패");
        }

        if (m_userData.inventoryItems == null)
            m_userData.inventoryItems = new List<InventoryItem>();
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
    #endregion

    // Load Item Images
    #region Img
    public Sprite LoadImg(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return null;

        Sprite sprite = LoadSpriteFromPersistentPath(imageUrl);

        if (sprite != null)
            return sprite;

        return LoadSpriteFromResources(imageUrl);
    }

    private string FindImageFilePath(string imageUrl)
    {
        string relativePath = imageUrl.Replace("\\", "/");
        string path = Path.Combine(Application.persistentDataPath, relativePath);

        if (File.Exists(path))
            return path;

        string extension = Path.GetExtension(path);

        if (!string.IsNullOrEmpty(extension))
            return string.Empty;

        foreach (string ext in IMAGE_EXTENSIONS)
        {
            string pathWithExt = path + ext;

            if (File.Exists(pathWithExt))
                return pathWithExt;
        }

        return string.Empty;
    }

    private Sprite LoadSpriteFromPersistentPath(string imageUrl)
    {
        string path = FindImageFilePath(imageUrl);

        if (string.IsNullOrEmpty(path))
            return null;

        byte[] bytes = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(2, 2);

        if (!texture.LoadImage(bytes))
        {
            Debug.LogWarning("이미지 파일 로드 실패 : " + path);
            return null;
        }

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }

    private Sprite LoadSpriteFromResources(string imageUrl)
    {
        string fileName = Path.GetFileNameWithoutExtension(imageUrl);

        Sprite sprite = Resources.Load<Sprite>(fileName);

        if (sprite == null)
            Debug.LogWarning("Resources 이미지 로드 실패 : " + fileName);

        return sprite;
    }
    #endregion

    // Function
    #region Function
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
