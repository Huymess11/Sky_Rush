using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    [SerializeField] private float timeCountDown;
    [SerializeField] private TextMeshProUGUI clockText;
    private float minutes, seconds;
    private bool isPaused = false;
    private Coroutine countdownCoroutine;
    private StringBuilder sb = new();

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void OnEnable()
    {
        ObserverManager.OnVictory += SetIsPause;
        StartTimer();
    }

    private void OnDisable()
    {
        ObserverManager.OnVictory -= SetIsPause;
        StopTimer(); 
    }

    private IEnumerator Countdown()
    {
        while (timeCountDown > 0)
        {
            if (!isPaused)
            {
                timeCountDown -= Time.deltaTime;
                timeCountDown = Mathf.Max(timeCountDown, 0);
                DisplayTime(timeCountDown);
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
}
