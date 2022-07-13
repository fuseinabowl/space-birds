using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnListener
{
    void OnTurnStarted(float turnStartTime);
    void OnTurnEnded();
}
