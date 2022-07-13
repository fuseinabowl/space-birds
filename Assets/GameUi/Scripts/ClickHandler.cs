using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class ClickHandler : MonoBehaviour
{
    [SerializeField]
    private FallbackClickHandler fallbackClickHandler = null;

    private void Awake()
    {
        Assert.IsNotNull(fallbackClickHandler);
    }

    private void Update()
    {
        HandleClicking();
    }

    private void HandleClicking()
    {
        if (Input.GetMouseButtonDown(0) && !IsClickingOnUi())
        {
            fallbackClickHandler.OnClick();
        }
    }

    static private bool IsClickingOnUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
