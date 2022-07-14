using System.Collections;
using System.Collections.Generic;
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
    private float extendVelocityDistanceWhenPlacingMarker = 20f;

    private ShipMover shipMover = null;

    private void Awake()
    {
        Assert.IsNotNull(turnSettings);
        Assert.IsNotNull(nextTurnEndMarker);

        shipMover = GetComponent<ShipMover>();
    }

    private void OnEnable()
    {
        turnSettings.turnListeners.Add(this);
    }

    private void Update()
    {
        shipMover.UpdateFromPlanner(turnSettings.TurnLengthInSeconds);
    }

    private void OnDisable()
    {
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
        MoveMovementMarkerToContinuedMovementPosition();
        ShowMovementMarker();
    }

    private void HideMovementMarker()
    {
        nextTurnEndMarker.gameObject.SetActive(false);
    }

    private void MoveMovementMarkerToContinuedMovementPosition()
    {
        var endVelocity = shipMover.TurnEndPosition - shipMover.MidPoint;
        var extendedPosition = shipMover.TurnEndPosition + extendVelocityDistanceWhenPlacingMarker * endVelocity.normalized;
        nextTurnEndMarker.transform.position = extendedPosition.ToWorldPosition();
    }

    private void ShowMovementMarker()
    {
        nextTurnEndMarker.gameObject.SetActive(true);
    }
}
