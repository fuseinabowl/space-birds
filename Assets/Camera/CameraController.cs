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

    private new Camera camera = null;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        camera.nearClipPlane = clipMiddleDistance - clipWidth / 2f;
        camera.farClipPlane = clipMiddleDistance + clipWidth / 2f;
    }

    void Update()
    {
        transform.position = targetTransform.position + Vector3.up * clipMiddleDistance;
    }
}
