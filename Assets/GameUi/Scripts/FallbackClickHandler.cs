using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FallbackClickHandler : MonoBehaviour
{
    public abstract void OnClick();
    public abstract Vector2 MousePositionToGamePoint(Vector2 mousePosition);
}
