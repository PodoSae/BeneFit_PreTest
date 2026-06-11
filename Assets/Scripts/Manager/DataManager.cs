using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private VendingMachineData m_parseData;

    private bool m_isActive;

    public VendingMachineData Data => m_parseData;
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
        string json = LoadJson();

        if (string.IsNullOrEmpty(json))
            return;

        ParseData(json);
    }

    private string LoadJson()
    {
        // Items 데이터 변경 가능성이 있음 - StreamingAssets 우선 사용, 파일 없을 시 기본 Items 사용

        string path = Path.Combine(Application.streamingAssetsPath,"Items.json");

        if (File.Exists(path))
        {
            Debug.Log("Use Streaming Data");
            return File.ReadAllText(path);
        }

        TextAsset textAsset = Resources.Load<TextAsset>("Items");

        if (textAsset == null)
        {
            Debug.LogError("Items.json 로드 실패");
            return string.Empty;
        }

        Debug.Log("Use Resource Data");
        return textAsset.text;
    }

    private void ParseData(string json)
    {
        m_parseData = JsonUtility.FromJson<VendingMachineData>(json);

        if (m_parseData == null)
        {
            Debug.LogError("Items.json 파싱 실패");
        }

        m_isActive = m_parseData != null &&
            !string.IsNullOrEmpty(m_parseData.status) &&
            m_parseData.status.ToLower() == "active";
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
        foreach (ProductData data in m_parseData.products)
        {
            if (data.id == _id)
                return data;
        }

        Debug.LogWarning($"상품 없음: {_id}");
        return null;
    }

    #endregion
}