using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private VendingMachineData m_parseData;

    public VendingMachineData Data => m_parseData;

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
    }
}