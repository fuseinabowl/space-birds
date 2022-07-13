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
    private ClickableObjects clickableObjects = null;

    private float clickStartTime;

    private void Awake()
    {
        Assert.IsNotNull(clickableObjects);
    }

    private ClickableObjects.ClickableObject remoteClickableObject = null;

    private void OnEnable()
    {
        remoteClickableObject = new ClickableObjects.ClickableObject{
            getCircle = GetCircle,
            onClicked = OnClicked,
            onUpdateClick = OnDragged,
        };
        clickableObjects.clickableObjects.Add(remoteClickableObject);
    }

    private void OnDisable()
    {
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
    }

    private void OnDragged(Vector2 mousePosition)
    {
        var timeSinceClick = Time.unscaledTime - clickStartTime;
        if (timeUntilTrackMouse < timeSinceClick)
        {
            transform.position = new Vector3(mousePosition.x, 0f, mousePosition.y);
        }
    }
}
