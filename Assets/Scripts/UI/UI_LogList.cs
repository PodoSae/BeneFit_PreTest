using UnityEngine;

public class UI_LogList : MonoBehaviour
{
    [SerializeField] private Transform m_parentLog;
    [SerializeField] private UI_LogItem m_itemLog;


    public void AddLog(string _msg, LogState _state)
    {
        var item = Instantiate(m_itemLog, m_parentLog);

        item.Initialize(_msg, _state);
    }

}
