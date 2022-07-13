using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class TurnRunner : MonoBehaviour
{
    private int fixedUpdatesRemainingThisTurn = 0;

    [SerializeField]
    public UnityEvent onStartGame = null;

    [SerializeField]
    private TurnSettings turnSettings = null;

    private float turnStartTime = 0f;

    private void Awake()
    {
        Assert.IsNotNull(turnSettings);
    }

    private void Start()
    {
        fixedUpdatesRemainingThisTurn = 0;
        PauseGame();
        turnStartTime = Time.time - turnSettings.TurnLengthInSeconds;
        onStartGame?.Invoke();
    }

    private void FixedUpdate()
    {
        if (fixedUpdatesRemainingThisTurn > 0)
        {
            if (--fixedUpdatesRemainingThisTurn == 0)
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        Assert.AreEqual(fixedUpdatesRemainingThisTurn, 0);
        Time.timeScale = 0f;

        foreach (var listener in turnSettings.turnListeners)
        {
            listener.OnTurnEnded();
        }

        var cachedOnTurnComplete = callerOnTurnComplete;
        callerOnTurnComplete = null;
        cachedOnTurnComplete?.Invoke();
    }

    public delegate void OnTurnComplete();
    private OnTurnComplete callerOnTurnComplete = null;
    public void ResumeGame(OnTurnComplete onTurnComplete)
    {
        Assert.AreEqual(fixedUpdatesRemainingThisTurn, 0);
        Assert.AreEqual(callerOnTurnComplete, null);

        turnStartTime += turnSettings.TurnLengthInSeconds;

        Time.timeScale = 1f;
        fixedUpdatesRemainingThisTurn = turnSettings.TurnLengthInFixedUpdates;

        foreach (var listener in turnSettings.turnListeners)
        {
            listener.OnTurnStarted(turnStartTime);
        }

        callerOnTurnComplete = onTurnComplete;
    }
}
