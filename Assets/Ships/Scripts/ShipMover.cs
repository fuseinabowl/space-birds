using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShipMover : MonoBehaviour, ITurnListener
{
    [SerializeField]
    private TurnSettings turnSettings = null;

    [SerializeField]
    private Transform nextTurnEndPosition = null;

    private Vector2 turnStartPosition;
    private Vector2 turnEndPosition;
    private Vector2 midPoint;
    private float turnStartTime;

    private void Awake()
    {
        Assert.IsNotNull(turnSettings);
        Assert.IsNotNull(nextTurnEndPosition);
    }

    private void Start()
    {
        turnStartPosition = new Vector2(transform.position.x, transform.position.z);
        midPoint = turnStartPosition;
    }

    private void OnEnable()
    {
        turnSettings.turnListeners.Add(this);
    }

    private void Update()
    {
        var elapsedTimeThisTurn = Time.time - turnStartTime;
        var elapsedTimeProportionThisTurn = elapsedTimeThisTurn / turnSettings.TurnLengthInSeconds;
        var position = QuadBezier(turnStartPosition, midPoint, turnEndPosition, elapsedTimeProportionThisTurn);

        transform.position = new Vector3(position.x, 0f, position.y);
    }

    private void OnDisable()
    {
        turnSettings.turnListeners.Remove(this);
    }

    private void PrepareForNextTurn(float turnStartTime, Vector2 nextEndPosition)
    {
        var previousTurnEndVelocity = turnEndPosition - midPoint;
        this.turnStartTime = turnStartTime;
        turnStartPosition = turnEndPosition;
        turnEndPosition = nextEndPosition;

        midPoint = turnStartPosition + previousTurnEndVelocity; 
    }

    private Vector2 QuadBezier(Vector2 start, Vector2 mid, Vector2 end, float time)
    {
        return Vector2.Lerp(
            Vector2.Lerp(start, mid, time),
            Vector2.Lerp(mid, end, time),
            time
        );
    }

    void ITurnListener.OnTurnStarted(float turnStartTime)
    {
        var nextEndPosition = new Vector2(nextTurnEndPosition.position.x, nextTurnEndPosition.position.z);
        PrepareForNextTurn(turnStartTime, nextEndPosition);
    }

    void ITurnListener.OnTurnEnded()
    {
    }
}
