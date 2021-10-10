
using UnityEngine; 

public static class Conditions
{
    public static bool IsInCircle(float radius, Vector2 center, Vector2 point)
    {
        return (point - center).sqrMagnitude < radius * radius;
    }

    public static bool IsInBounds(Bounds bounds, Vector2 center, Vector2 point)
    {
        Vector2 TopRightCorner = center + (Vector2)bounds.extents;
        Vector2 BottomLeftCorner = center - (Vector2)bounds.extents;
        return (TopRightCorner.y > point.y) && (BottomLeftCorner.y < point.y) && (BottomLeftCorner.x < point.x) && (TopRightCorner.x > point.x);
    }
}

