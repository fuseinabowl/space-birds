using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float clipWidth = 100f;
    [SerializeField]
    private float clipMiddleDistance = 600f;
    [SerializeField]
    [FormerlySerializedAs("viewWidth")]
    private float viewHeight = 20f;

    [SerializeField]
    private float dragSpeed = 1f;

    public delegate Vector3 GetCursorPosition();
    private GetCursorPosition cachedCursorPositionGetter = null;

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
        camera.fieldOfView = Mathf.Rad2Deg * 2f * Mathf.Atan2(viewHeight, 2f * clipMiddleDistance);
    }

    private void Update()
    {
        SetCameraProperties();

        if (cachedCursorPositionGetter != null)
        {
            UpdateDrag(cachedCursorPositionGetter());
        }
    }

    public void StartDrag(GetCursorPosition cursorPositionGetter)
    {
        cachedCursorPositionGetter = cursorPositionGetter;
        dragClickStartPosition = cachedCursorPositionGetter();
        dragCameraStartPosition = transform.position;
        dragCameraStartPosition.Scale(new Vector3(1f, 0f, 1f));
    }

    public void StopDrag()
    {
        cachedCursorPositionGetter = null;
    }

    public void UpdateDrag(Vector3 mousePosition)
    {
        var mousePositionDelta = camera.ScreenToViewportPoint(mousePosition - dragClickStartPosition);
        mousePositionDelta.Scale(new Vector3(viewHeight * camera.aspect, viewHeight, 0f));
        var mouseWorldDelta = Grid.Swizzle(GridLayout.CellSwizzle.XZY, mousePositionDelta);
 
        transform.position = dragCameraStartPosition - mouseWorldDelta * dragSpeed + Vector3.up * clipMiddleDistance;
    }

    static private bool IsClickingOnUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
