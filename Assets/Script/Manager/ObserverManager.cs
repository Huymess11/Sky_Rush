using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObserverManager
{
    public static event Action OnDefrost;
    public static event Action<bool> OnVictory;
    public static event Action<bool> OnHintDestroy;
    public static event Action OnReviewGate;

    public static void Defrost()
    {
        OnDefrost?.Invoke();
    }
    public static void Victory(bool status)
    {
        OnVictory?.Invoke(status);
    }
    public static void HintDestroy(bool status)
    {
        OnHintDestroy?.Invoke(status);
    }
    public static void ReviewGate()
    {
        OnReviewGate?.Invoke(); 
    }

}