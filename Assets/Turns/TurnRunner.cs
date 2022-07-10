using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TurnRunner : MonoBehaviour
{
    [SerializeField]
    private int turnLengthInFixedUpdates = 60;
    private int fixedUpdatesRemainingThisTurn = 0;

    private void Start()
    {
        fixedUpdatesRemainingThisTurn = 0;
        PauseGame();
    }

    private void FixedUpdate()
    {
        if (--fixedUpdatesRemainingThisTurn <= 0)
        {
            // FixedUpdate can be called before PauseGame can take effect
            // from Start() on the first frame
            fixedUpdatesRemainingThisTurn = 0;
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Assert.AreEqual(fixedUpdatesRemainingThisTurn, 0);
        Time.timeScale = 0f;

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
        Time.timeScale = 1f;
        fixedUpdatesRemainingThisTurn = turnLengthInFixedUpdates;

        callerOnTurnComplete = onTurnComplete;
    }
}
