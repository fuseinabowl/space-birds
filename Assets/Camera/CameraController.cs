using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float clipWidth = 100f;
    [SerializeField]
    private float clipMiddleDistance = 600f;
    [SerializeField]
    private float viewWidth = 20f;

    [SerializeField]
    private float dragSpeed = 1f;

    private Vector3 dragClickStartPosition = Vector3.zero;
    private Vector3 dragCameraStartPosition = Vector3.zero;
    private new Camera camera = null;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        SetCameraProperties();
        transform.position = Vector3.up * clipMiddleDistance;
    }

    private void SetCameraProperties()
    {
        camera.nearClipPlane = clipMiddleDistance - clipWidth / 2f;
        camera.farClipPlane = clipMiddleDistance + clipWidth / 2f;
        camera.fieldOfView = Mathf.Rad2Deg * 2f * Mathf.Atan2(viewWidth, 2f * clipMiddleDistance);
    }

    private void Update()
    {
        SetCameraProperties();
 
        if (Input.GetMouseButtonDown(0))
        {
            dragClickStartPosition = Input.mousePosition;
            dragCameraStartPosition = transform.position;
            dragCameraStartPosition.Scale(new Vector3(1f, 0f, 1f));
            return;
        }
 
        if (!Input.GetMouseButton(0)) return;
 
        var mousePositionDelta = camera.ScreenToViewportPoint(Input.mousePosition - dragClickStartPosition);
        Debug.Log($"Position delta: {mousePositionDelta}");
        mousePositionDelta.Scale(new Vector3(viewWidth * camera.aspect, viewWidth, 0f));
        var mouseWorldDelta = Grid.Swizzle(GridLayout.CellSwizzle.XZY, mousePositionDelta);
 
        transform.position = dragCameraStartPosition - mouseWorldDelta * dragSpeed + Vector3.up * clipMiddleDistance;
    }
}
