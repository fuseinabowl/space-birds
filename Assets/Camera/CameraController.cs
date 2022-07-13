using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Camera))]
public class CameraController : FallbackClickHandler
{
    [SerializeField]
    private float clipWidth = 100f;
    [SerializeField]
    private float clipMiddleDistance = 600f;
    [SerializeField]
    [FormerlySerializedAs("viewWidth")]
    private float viewHeight = 20f;
    [SerializeField]
    private Vector2 viewHeightRange = new Vector2(10f, 40f);
    [SerializeField]
    private float zoomScrollSpeed = 1f;
    [SerializeField]
    private float zoomEasingSpeed = 1f;

    [SerializeField]
    private float dragSpeed = 1f;

    private Vector3 dragClickStartPosition = Vector3.zero;
    private Vector3 dragCameraStartPosition = Vector3.zero;
    private new Camera camera = null;

    private float targetViewHeight = 0f;

    private bool isDragging = false;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        SetCameraProperties();
        transform.position = Vector3.up * clipMiddleDistance;
        targetViewHeight = viewHeight;
    }

    private void SetCameraProperties()
    {
        camera.nearClipPlane = clipMiddleDistance - clipWidth / 2f;
        camera.farClipPlane = clipMiddleDistance + clipWidth / 2f;
        camera.fieldOfView = Mathf.Rad2Deg * 2f * Mathf.Atan2(viewHeight, 2f * clipMiddleDistance);
    }

    private void Update()
    {
        HandleCameraZoomControls();
        HandleCameraZoomEasing();
        HandleCameraDrag();

        SetCameraProperties();
    }

    private void HandleCameraDrag()
    {
        if (isDragging)
        {
            if (!Input.GetMouseButton(0))
            {
                isDragging = false;
                return;
            }
 
            var mousePositionDelta = camera.ScreenToViewportPoint(Input.mousePosition - dragClickStartPosition);
            mousePositionDelta.Scale(new Vector3(viewHeight * camera.aspect, viewHeight, 0f));
            var mouseWorldDelta = Grid.Swizzle(GridLayout.CellSwizzle.XZY, mousePositionDelta);
     
            transform.position = dragCameraStartPosition - mouseWorldDelta * dragSpeed + Vector3.up * clipMiddleDistance;
        }
    }

    public override void OnClick()
    {
        StartDragging();
    }

    private void StartDragging()
    {
        dragClickStartPosition = Input.mousePosition;
        dragCameraStartPosition = transform.position;
        dragCameraStartPosition.Scale(new Vector3(1f, 0f, 1f));
        isDragging = true;
    }

    private void HandleCameraZoomControls()
    {
        var scroll = Input.mouseScrollDelta.y;
        // use minus scroll to scroll-zoom in the intuitive direction
        var zoomLogDelta = -scroll * zoomScrollSpeed;
        if (zoomLogDelta != 0f)
        {
            var currentViewHeightInLogSpace = Mathf.Log(targetViewHeight);
            var newViewHeightInLogSpace = currentViewHeightInLogSpace + zoomLogDelta;
            var newViewHeightInNormalSpace = Mathf.Exp(newViewHeightInLogSpace);
            targetViewHeight = Mathf.Clamp(newViewHeightInNormalSpace, viewHeightRange.x, viewHeightRange.y);
        }
    }

    private void HandleCameraZoomEasing()
    {
        var delta = targetViewHeight - viewHeight;
        viewHeight = targetViewHeight - delta * Mathf.Exp(-Time.unscaledDeltaTime * zoomEasingSpeed);
    }
}
