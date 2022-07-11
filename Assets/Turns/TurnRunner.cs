using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class TurnRunner : MonoBehaviour
{
    [SerializeField]
    private int prewarmEffectsSetupSteps = 5;
    [SerializeField]
    private int prewarmEffectsDuration = 15;

    [SerializeField]
    private int turnLengthInFixedUpdates = 60;
    private int fixedUpdatesRemainingThisTurn = 0;

    [SerializeField]
    public UnityEvent onStartGame = null;

    private void Start()
    {
        StartCoroutine(PrewarmEffectsThenStartGame());
    }

    private IEnumerator PrewarmEffectsThenStartGame()
    {
        Time.timeScale = 100f;

        // Unity has to spend some whole frames setting up the engine and the scene before the VFXes start running
        for (var i = 0; i < prewarmEffectsSetupSteps; ++i)
        {
            yield return 0;
        }
        // the VFXes will have started now, run them
        yield return new WaitForSeconds(prewarmEffectsDuration);

        fixedUpdatesRemainingThisTurn = 0;
        PauseGame();
        onStartGame?.Invoke();
    }

    private void FixedUpdate()
    {
        if (--fixedUpdatesRemainingThisTurn == 0)
        {
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
