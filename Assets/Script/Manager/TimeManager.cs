using Sirenix.OdinInspector;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    [SerializeField] private float timeCountDown;
    private float _timeCountDown;
    [SerializeField] private TextMeshProUGUI clockText;
    private float minutes, seconds;
    private bool isPaused = false;
    private Coroutine countdownCoroutine;
    private StringBuilder sb = new();

    [Title("Hint")]
    public Image clockBG;
    public Image clockPause;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    private void Start()
    {
        _timeCountDown = timeCountDown;
    }

    private void OnEnable()
    {
        ObserverManager.OnVictory += SetIsPause;
    }

    private void OnDisable()
    {
        ObserverManager.OnVictory -= SetIsPause;
    }

    private IEnumerator Countdown()
    {
        while (_timeCountDown > 0)
        {
            if (!isPaused)
            {
                _timeCountDown -= Time.deltaTime;
                _timeCountDown = Mathf.Max(_timeCountDown, 0);
                DisplayTime(_timeCountDown);
                SetClock(minutes, seconds);
            }
            yield return null;
        }
        LevelManager.Instance.Defeat();
    }

    void DisplayTime(float time)
    {
        minutes = Mathf.FloorToInt(time / 60);
        seconds = Mathf.FloorToInt(time % 60);
    }

    void SetClock(float min, float seconds)
    {
        sb.Clear();
        sb.AppendFormat("{0:00}:{1:00}", min, seconds);
        clockText.text = sb.ToString();
    }

    public void SetIsPause(bool status)
    {
        isPaused = status;
    }
    public void StartTimer()
    {
        if (countdownCoroutine == null)
        {
            isPaused = false;
            countdownCoroutine = StartCoroutine(Countdown());
        }
    }
    public void StopTimer()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }
    public void PauseForSeconds(float duration)
    {
        if (countdownCoroutine != null)
        {
            StartCoroutine(PauseCoroutine(duration));
        }
    }

    private IEnumerator PauseCoroutine(float duration)
    {
        clockBG.gameObject.SetActive(true);
        isPaused = true;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            clockPause.fillAmount = (elapsedTime / duration); 
            yield return null;
        }
        SH_GameManager.Instance._VFX_Snow.SetActive(false);
        SH_GameManager.Instance._canPressFrozen = true;
        isPaused = false;
        clockBG.gameObject.SetActive(false);
        clockPause.fillAmount = 0;
    }
    public void ResetTimer()
    {
        StopTimer();
        _timeCountDown = timeCountDown;
        _timeCountDown *= 60f;
        DisplayTime(_timeCountDown); 
        SetClock(minutes, seconds);
    }
}
