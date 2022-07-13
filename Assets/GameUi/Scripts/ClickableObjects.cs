using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Clickable Objects", menuName="Clickable Objects")]
public class ClickableObjects : ScriptableObject
{
    public struct Circle
    {
        public Vector2 position;
        public float radius;
    }
    public delegate Circle GetCircle();
    public delegate void Event(Vector2 cursorWorldPosition);
    public class ClickableObject
    {
        public GetCircle getCircle;
        public Event onClicked;
        public Event onUpdateClick;
        public Event onRelease;

        public bool alive = true;
    }

    public List<ClickableObject> clickableObjects = new List<ClickableObject>();
}
