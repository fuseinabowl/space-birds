using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class ClickHandler : MonoBehaviour
{
    [SerializeField]
    private FallbackClickHandler fallbackClickHandler = null;

    [SerializeField]
    private ClickableObjects clickableObjects = null;

    private bool fallbackHandlerClicked = false;

    private void Awake()
    {
        Assert.IsNotNull(fallbackClickHandler);
        Assert.IsNotNull(clickableObjects);
    }

    private void Update()
    {
        HandleClicking();
    }

    private void HandleClicking()
    {
        if (Input.GetMouseButtonDown(0) && !IsClickingOnUi())
        {
            if (!TryClickClickableObjects())
            {
                fallbackClickHandler.OnClick();
                fallbackHandlerClicked = true;
            }
        }
        else if (fallbackHandlerClicked)
        {
            if (Input.GetMouseButton(0))
            {
                fallbackClickHandler.OnUpdateClick();
            }
            else
            {
                fallbackClickHandler.OnRelease();
                fallbackHandlerClicked = false;
            }
        }
    }

    private bool TryClickClickableObjects()
    {
        var mouseClickPoint = fallbackClickHandler.MousePositionToGamePoint(Input.mousePosition);
        var clickableObject = GetBestClickableObject(mouseClickPoint);
        if (clickableObject != null)
        {
            clickableObject.onClicked.Invoke();
            return true;
        }

        return false;
    }

    private struct BestClickable
    {
        public ClickableObjects.ClickableObject clickable;
        public float scaledRadius;
    }
    private ClickableObjects.ClickableObject GetBestClickableObject(Vector2 mousePosition)
    {
        return clickableObjects.clickableObjects
            .Select(obj => {
                var circle = obj.getCircle();
                return new {obj=obj, circle=circle, distanceSquare=(circle.position - mousePosition).sqrMagnitude};
            })
            .Where(objProcessed => objProcessed.distanceSquare < objProcessed.circle.radius * objProcessed.circle.radius)
            .Aggregate(new BestClickable{clickable=null, scaledRadius=float.PositiveInfinity},
                (bestSoFar, objProcessed) => {
                    var scaledRadius = Mathf.Sqrt(objProcessed.distanceSquare) / objProcessed.circle.radius;
                    if (scaledRadius < bestSoFar.scaledRadius)
                    {
                        return new BestClickable{
                            clickable = objProcessed.obj,
                            scaledRadius = scaledRadius
                        };
                    }
                    else
                    {
                        return bestSoFar;
                    }
                }
            )
            .clickable;
    }

    static private bool IsClickingOnUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
