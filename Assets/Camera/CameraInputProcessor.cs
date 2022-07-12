using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class CameraInputProcessor : MonoBehaviour
{
    [SerializeField]
    private CameraController cameraController = null;

    private void Awake()
    {
        Assert.IsNotNull(cameraController);
    }

    public void OnClicked(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cameraController.StartDrag(GetCursorPosition);
        }
        else if (context.canceled)
        {
            cameraController.StopDrag();
        }
    }

    private Vector3 GetCursorPosition()
    {
        var pointerPosition = Pointer.current.position.ReadValue();
        return new Vector3(pointerPosition.x, pointerPosition.y, 0f);
    }
}
