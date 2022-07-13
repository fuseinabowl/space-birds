using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Turn Listener List", menuName="Turn Listener List")]
public class TurnSettings : ScriptableObject
{
    [SerializeField]
    private int turnLengthInFixedUpdates = 60; 
    public int TurnLengthInFixedUpdates => turnLengthInFixedUpdates;
    public float TurnLengthInSeconds => Time.fixedDeltaTime * turnLengthInFixedUpdates;

    public List<ITurnListener> turnListeners = new List<ITurnListener>();
}
