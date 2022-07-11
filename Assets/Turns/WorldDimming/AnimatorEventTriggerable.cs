using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorEventTriggerable : MonoBehaviour
{
    public void SetBool(string parameterName)
    {
        AssignBool(parameterName, true);
    }

    public void ClearBool(string parameterName)
    {
        AssignBool(parameterName, false);
    }

    private void AssignBool(string parameterName, bool newValue)
    {
        var siblingAnimator = GetComponent<Animator>();
        siblingAnimator.SetBool(parameterName, newValue);
    }
}
