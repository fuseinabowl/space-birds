using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMover : MonoBehaviour
{
    [SerializeField]
    private Transform rotateOrigin;

    [SerializeField]
    [Tooltip("Degrees per second")]
    private float rotateSpeed;

    private float cachedDistanceFromOrigin = 0;
    private float rotationStart = 0;
    private float timeStart = 0;

    private void Start()
    {
        cachedDistanceFromOrigin = (transform.position - rotateOrigin.position).magnitude;
        rotationStart = Mathf.Atan2(transform.position.x - rotateOrigin.position.x, transform.position.y - rotateOrigin.position.y);
        timeStart = Time.time;
    }

    private void Update()
    {
        var elapsedTime = Time.time - timeStart;
        var totalRotation = rotationStart + elapsedTime * rotateSpeed;
        var rotation = Quaternion.AngleAxis(totalRotation, Vector3.up);

        transform.rotation = rotation;
        transform.position =  rotation * Vector3.left * cachedDistanceFromOrigin + rotateOrigin.position;
    }
}
