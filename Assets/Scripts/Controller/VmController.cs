using UnityEngine;

public class VmController : MonoBehaviour
{
    [SerializeField] private UI_VendingMachine m_uiVM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_uiVM.Initialize(DataManager.Instance.Data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
