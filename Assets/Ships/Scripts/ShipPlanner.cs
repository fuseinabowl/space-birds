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

    private ShipMover shipMover = null;

    private void Awake()
    {
        Assert.IsNotNull(turnSettings);
        Assert.IsNotNull(nextTurnEndMarker);
        Assert.IsNotNull(plannedPath);

        shipMover = GetComponent<ShipMover>();
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
    }

    private void UpdatePlannedPathIndicator(Vector2 nextMoveInitialEndPosition)
    {
        var previousTurnFinalVelocity = shipMover.TurnEndPosition - shipMover.MidPoint;
        plannedPath.SetPositions(Enumerable.Range(0, plannedPath.positionCount).Select(pointIndex => {
            var pointTime = (float)pointIndex / (plannedPath.positionCount - 1);
            return ShipMover.QuadBezier(shipMover.TurnEndPosition, shipMover.TurnEndPosition + previousTurnFinalVelocity, nextMoveInitialEndPosition, pointTime).ToWorldPosition();
        }).ToArray());
    }
}
