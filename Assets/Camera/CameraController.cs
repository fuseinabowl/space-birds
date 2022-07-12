using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform = null;

    [SerializeField]
    private float clipWidth = 100f;
    [SerializeField]
    private float clipMiddleDistance = 600f;
    [SerializeField]
    private float viewWidth = 20f;

    private new Camera camera = null;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        SetCameraProperties();
    }

    private void SetCameraProperties()
    {
        camera.nearClipPlane = clipMiddleDistance - clipWidth / 2f;
        camera.farClipPlane = clipMiddleDistance + clipWidth / 2f;
        camera.fieldOfView = Mathf.Rad2Deg * 2f * Mathf.Atan2(viewWidth, 2f * clipMiddleDistance);
    }

    private void Update()
    {
        transform.position = targetTransform.position + Vector3.up * clipMiddleDistance;
        SetCameraProperties();
    }
}
