using System;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    public event Action<string, LogState> OnAddLog;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddLog(string _log , LogState _state = LogState.Normal)
    {
        OnAddLog?.Invoke(_log, _state);    
    }
}

public enum LogState
{ 
    Normal,
    Error,
}
