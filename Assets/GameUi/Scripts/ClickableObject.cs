using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ClickableObject : MonoBehaviour
{
    [SerializeField]
    private float radius = 1f;

    [SerializeField]
    private ClickableObjects clickableObjects = null;

    private void Awake()
    {
        Assert.IsNotNull(clickableObjects);
    }

    private ClickableObjects.ClickableObject remoteClickableObject = null;

    private void OnEnable()
    {
        remoteClickableObject = new ClickableObjects.ClickableObject{
            getCircle = GetCircle,
            onClicked = OnClicked
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
            position = new Vector2(transform.position.x, transform.position.y),
            radius = radius
        };
    }

    private void OnClicked()
    {
    }
}
