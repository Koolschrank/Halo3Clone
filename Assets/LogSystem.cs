using System;
using UnityEngine;

public class LogSystem : MonoBehaviour
{
    public Action<string> OnLogPrinted;

    // singelton
    public static LogSystem logSystem;

    public void Awake()
    {
        logSystem = this;
    }



    public void PrintLog(string value)
    {
        OnLogPrinted?.Invoke(value);
    }

    public void PlayerKilled(string killer,  string killed)
    {
        PrintLog(killer + " => " + killed);
    }
}
