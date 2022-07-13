using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Assertions;

public class ShipMover : MonoBehaviour, ITurnListener
{
    [SerializeField]
    private TurnSettings turnSettings = null;

    [SerializeField]
    private ClickableObject nextTurnEndMarker = null;

    [SerializeField]
    private float extendVelocityDistanceWhenPlacingMarker = 20f;

    [SerializeField]
    private float rollMultiplier = 1f;
    [SerializeField]
    private AnimationCurve rollMultiplierThroughoutTurn = AnimationCurve.Constant(0f, 1f, 1f);

    private Vector2 turnStartPosition;
    private Vector2 turnEndPosition;
    private Vector2 midPoint;
    private float turnStartTime;

    private void Awake()
    {
        Assert.IsNotNull(turnSettings);
        Assert.IsNotNull(nextTurnEndMarker);
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
        var delta = 2f * elapsedTimeProportionThisTurn * (turnStartPosition - 2f * midPoint + turnEndPosition)
                  +                                 1f * (-2f * turnStartPosition + 2f * midPoint);

        var angle = Mathf.Atan2(delta.x, delta.y);
        var rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up);

        var acceleration = 2f * (turnStartPosition - 2f * midPoint + turnEndPosition);
        var rollMagnitude = delta.normalized.x * acceleration.y - delta.normalized.y * acceleration.x;
        var roll = Quaternion.AngleAxis(rollMagnitude * rollMultiplier * rollMultiplierThroughoutTurn.Evaluate(elapsedTimeProportionThisTurn), Vector3.forward);

        transform.position = FlatPositionToWorldPosition(position);
        transform.rotation = rotation * roll;
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
        var nextEndPosition = nextTurnEndMarker.TargetPosition;
        PrepareForNextTurn(turnStartTime, nextEndPosition);

        HideMovementMarker();
    }

    void ITurnListener.OnTurnEnded()
    {
        MoveMovementMarkerToContinuedMovementPosition();
        ShowMovementMarker();
    }

    private void HideMovementMarker()
    {
        nextTurnEndMarker.gameObject.SetActive(false);
    }

    private void MoveMovementMarkerToContinuedMovementPosition()
    {
        var endVelocity = turnEndPosition - midPoint;
        var extendedPosition = turnEndPosition + extendVelocityDistanceWhenPlacingMarker * endVelocity.normalized;
        nextTurnEndMarker.transform.position = FlatPositionToWorldPosition(extendedPosition);
    }

    private void ShowMovementMarker()
    {
        nextTurnEndMarker.gameObject.SetActive(true);
    }

    private Vector3 FlatPositionToWorldPosition(Vector2 vector)
    {
        return new Vector3(vector.x, 0f, vector.y);
    }
}
