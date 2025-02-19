using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    public void Victory()
    {
        Debug.Log("Chuc mung ban da thang !");
        ObserverManager.Victory(true);
    }
    public void Defeat()
    {
        Time.timeScale = 0f;
        Debug.Log("Con ga con !");
    }
}
