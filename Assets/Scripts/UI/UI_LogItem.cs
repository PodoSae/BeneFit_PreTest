using TMPro;
using UnityEngine;

public class UI_LogItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_textLog;
    public void Initialize(string _msg, LogState _state)
    {
        m_textLog.text = _msg;

        if (_state == LogState.Error)
            m_textLog.color = Color.red;
    }
}


