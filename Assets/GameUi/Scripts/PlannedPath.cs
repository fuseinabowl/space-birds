using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(LineRenderer))]
public class PlannedPath : MonoBehaviour
{
    [SerializeField]
    private ClickableObject nextTurnEndMarker = null;
    private LineRenderer pathIndicator = null;

    private void Awake()
    {
        Assert.IsNotNull(nextTurnEndMarker);
        pathIndicator = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }
}
