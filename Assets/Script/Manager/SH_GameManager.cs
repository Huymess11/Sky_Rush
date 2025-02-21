using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SH_GameManager : MonoBehaviour
{
    public static SH_GameManager Instance;
    private bool canPressFrozen = true;
    private bool canPressDestroy = true;

    [SerializeField] private LevelData levelData;
    private GameObject level;
    int index = 0;
    public GameObject gameOverPanel;

    [Title("VFX")]
    [SerializeField] private GameObject VFX_Confetti;
    [SerializeField] private GameObject VFX_Snow;
    public GameObject _VFX_Confetti => VFX_Confetti;
    public GameObject _VFX_Snow => VFX_Snow;

    private void Start()
    {
        SpawnLevel();
    }
    public void SpawnLevel()
    {
        TimeManager.Instance.ResetTimer();
        int index = PlayerPrefs.GetInt("index", 0);
        index = (index + 1 > levelData.levelInfors.Count) ? 1 : index + 1;
        PlayerPrefs.SetInt("index", index);
        level = Instantiate(levelData.levelInfors[index-1].levelPrefabs);

    }

    public bool _canPressFrozen
    {
        get
        {
            return canPressFrozen;
        }
        set
        {
            canPressFrozen = value;
        }
    }
    public bool _canPressDestroy
    {
        get
        {
            return canPressDestroy;
        }
        set
        {
            canPressDestroy = value;
        }
    }

    private void Awake()
    {
        if(Instance == null) { Instance = this; }
    }
    #region HINT
    public void FrozenTime()
    {
        if (!canPressFrozen) return;
        canPressFrozen = false;
        _VFX_Snow.SetActive(true);
        TimeManager.Instance.PauseForSeconds(10f);
    }
    public void DestroyBlock()
    {
        if (!canPressDestroy) return;
        canPressDestroy = false;
        ObserverManager.HintDestroy(true);
    }
    public void ShowGOPanel()
    {
     gameOverPanel.SetActive(true);    
    }
    #endregion
}
