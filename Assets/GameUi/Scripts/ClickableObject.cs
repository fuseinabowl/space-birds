using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ClickableObject : MonoBehaviour
{
    [SerializeField]
    private float radius = 1f;

    [SerializeField]
    private float timeUntilTrackMouse = 0.1f;
    [SerializeField]
    private float lerpToMouseDuration = 0.1f;

    [SerializeField]
    private ClickableObjects clickableObjects = null;

    private float clickStartTime;
    private Vector2 preClickPosition;
    private Vector2 targetPosition;

    private void Awake()
    {
        Assert.IsNotNull(clickableObjects);
    }

    private ClickableObjects.ClickableObject remoteClickableObject = null;

    private void OnEnable()
    {
        targetPosition = new Vector2(transform.position.x, transform.position.z);
        remoteClickableObject = new ClickableObjects.ClickableObject{
            getCircle = GetCircle,
            onClicked = OnClicked,
            onUpdateClick = OnDragged,
        };
        clickableObjects.clickableObjects.Add(remoteClickableObject);
    }

    private void Update()
    {
        var timeSinceClick = Time.unscaledTime - clickStartTime;
        var timeSinceStartMoving = timeSinceClick - timeUntilTrackMouse;
        var proportionOfTimeThroughMovement = timeSinceStartMoving / lerpToMouseDuration;
        var outputPosition = Vector2.Lerp(preClickPosition, targetPosition, proportionOfTimeThroughMovement);
        transform.position = MousePositionToWorldPosition(outputPosition);
    }

    private void OnDisable()
    {
        remoteClickableObject.alive = false;
        clickableObjects.clickableObjects.Remove(remoteClickableObject);
        remoteClickableObject = null;
    }

    private ClickableObjects.Circle GetCircle()
    {
        return new ClickableObjects.Circle{
            position = new Vector2(transform.position.x, transform.position.z),
            radius = radius
        };
    }

    private void OnClicked(Vector2 mousePosition)
    {
        clickStartTime = Time.unscaledTime;
        preClickPosition = new Vector2(transform.position.x, transform.position.z);
        targetPosition = preClickPosition;
    }

    private void OnDragged(Vector2 mousePosition)
    {
        var timeSinceClick = Time.unscaledTime - clickStartTime;
        if (timeSinceClick > timeUntilTrackMouse)
        {
            targetPosition = mousePosition;
            if (timeSinceClick > timeUntilTrackMouse + lerpToMouseDuration)
            {
                // skip waiting for this class's update, guarantees to avoid one frame of latency
                preClickPosition = mousePosition;
                transform.position = MousePositionToWorldPosition(mousePosition);
            }
        }
    }

    private Vector3 MousePositionToWorldPosition(Vector2 mousePosition)
    {
        return new Vector3(mousePosition.x, 0f, mousePosition.y);
    }
}
