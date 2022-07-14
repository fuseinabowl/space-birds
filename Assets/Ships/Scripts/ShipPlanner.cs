using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(ShipMover))]
public class ShipPlanner : MonoBehaviour, ITurnListener
{
    [SerializeField]
    private TurnSettings turnSettings = null;

    [SerializeField]
    private ClickableObject nextTurnEndMarker = null;

    [SerializeField]
    private LineRenderer plannedPath = null;

    [SerializeField]
    private float extendVelocityDistanceWhenPlacingMarker = 20f;

    [SerializeField]
    private float maxTurnAngle = 45f;
    [SerializeField]
    private float minSpeed = 20f;
    [SerializeField]
    private float maxSpeed = 20f;

    private ShipMover shipMover = null;

    private void Awake()
    {
        Assert.IsNotNull(turnSettings);
        Assert.IsNotNull(nextTurnEndMarker);
        Assert.IsNotNull(plannedPath);

        shipMover = GetComponent<ShipMover>();
    }

    private void Start()
    {
        nextTurnEndMarker.positionClamper = ClampTargetPosition;
    }

    private void OnEnable()
    {
        turnSettings.turnListeners.Add(this);
        nextTurnEndMarker.onMoved += OnEndMarkerMoved;
    }

    private void Update()
    {
        shipMover.UpdateFromPlanner(turnSettings.TurnLengthInSeconds);
    }

    private void OnDisable()
    {
        nextTurnEndMarker.onMoved -= OnEndMarkerMoved;
        turnSettings.turnListeners.Remove(this);
    }

    void ITurnListener.OnTurnStarted(float turnStartTime)
    {
        var nextEndPosition = nextTurnEndMarker.TargetPosition;
        shipMover.PrepareForNextTurn(turnStartTime, nextEndPosition);

        HideMovementMarker();
    }

    void ITurnListener.OnTurnEnded()
    {
        var nextMoveInitialEndPosition = CalculateNextMoveInitialEndPosition();
        MoveMovementMarkerToContinuedMovementPosition(nextMoveInitialEndPosition);
        UpdatePlannedPathIndicator(nextMoveInitialEndPosition);
        UpdateMovementMarkerRotation(nextMoveInitialEndPosition);
        ShowMovementMarker();
    }

    private void HideMovementMarker()
    {
        plannedPath.gameObject.SetActive(false);
        nextTurnEndMarker.gameObject.SetActive(false);
    }

    private Vector2 CalculateNextMoveInitialEndPosition()
    {
        var endVelocity = shipMover.TurnEndPosition - shipMover.MidPoint;
        var extendedPosition = shipMover.TurnEndPosition + endVelocity + extendVelocityDistanceWhenPlacingMarker * endVelocity.normalized;
        return extendedPosition;
    }

    private void MoveMovementMarkerToContinuedMovementPosition(Vector2 nextMoveInitialEndPosition)
    {
        nextTurnEndMarker.transform.position = nextMoveInitialEndPosition.ToWorldPosition();
    }

    private void ShowMovementMarker()
    {
        nextTurnEndMarker.gameObject.SetActive(true);
        plannedPath.gameObject.SetActive(true);
    }

    private void OnEndMarkerMoved(Vector2 newPosition)
    {
        UpdatePlannedPathIndicator(newPosition);
        UpdateMovementMarkerRotation(newPosition);
    }

    private void UpdatePlannedPathIndicator(Vector2 nextMoveInitialEndPosition)
    {
        var previousTurnFinalVelocity = shipMover.TurnEndPosition - shipMover.MidPoint;
        plannedPath.SetPositions(Enumerable.Range(0, plannedPath.positionCount).Select(pointIndex => {
            var pointTime = (float)pointIndex / (plannedPath.positionCount - 1);
            return ShipMover.QuadBezier(shipMover.TurnEndPosition, shipMover.TurnEndPosition + previousTurnFinalVelocity, nextMoveInitialEndPosition, pointTime).ToWorldPosition();
        }).ToArray());
    }

    private void UpdateMovementMarkerRotation(Vector2 nextMoveEndPosition)
    {
        var previousTurnFinalVelocity = shipMover.TurnEndPosition - shipMover.MidPoint;
        var nextTurnMidPoint = shipMover.TurnEndPosition + previousTurnFinalVelocity;
        var endDirection = nextMoveEndPosition - nextTurnMidPoint;
        nextTurnEndMarker.transform.rotation = Quaternion.LookRotation(endDirection.ToWorldPosition(), Vector3.up);
    }

    private Vector2 ClampTargetPosition(Vector2 inputPosition)
    {
        var previousTurnFinalVelocity = shipMover.TurnEndPosition - shipMover.MidPoint;
        var nextTurnMidPoint = shipMover.TurnEndPosition + previousTurnFinalVelocity;
        var nextTurnIntendedVelocity = inputPosition - nextTurnMidPoint;

        var polarAngle = Vector2.SignedAngle(previousTurnFinalVelocity, nextTurnIntendedVelocity);

        if (polarAngle < -maxTurnAngle)
        {
            return nextTurnMidPoint + ProjectOntoBorderLine(previousTurnFinalVelocity, -maxTurnAngle, nextTurnIntendedVelocity);
        }
        else if (polarAngle > maxTurnAngle)
        {
            return nextTurnMidPoint + ProjectOntoBorderLine(previousTurnFinalVelocity, maxTurnAngle, nextTurnIntendedVelocity);
        }
        else
        {
            var polarRadius = nextTurnIntendedVelocity.magnitude;
            var clampedRadius = Mathf.Clamp(polarRadius, minSpeed, maxSpeed);
            return nextTurnMidPoint + nextTurnIntendedVelocity / polarRadius * clampedRadius;
        }
    }

    private Vector2 ProjectOntoBorderLine(Vector2 baseLine, float additionalAngle, Vector2 pointToProject)
    {
        var borderWorldAngle = -Vector2.SignedAngle(Vector2.up, baseLine) - additionalAngle;
        var borderLineDirection = new Vector2(Mathf.Sin(borderWorldAngle * Mathf.Deg2Rad), Mathf.Cos(borderWorldAngle * Mathf.Deg2Rad));
        var closestPointOnBorderRadius = Vector2.Dot(pointToProject, borderLineDirection);
        var clampedRadius = Mathf.Clamp(closestPointOnBorderRadius, minSpeed, maxSpeed);
        return borderLineDirection * clampedRadius;
    }
}
