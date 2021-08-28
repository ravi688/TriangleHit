using UnityEngine;
using System.Collections;

public class Utility
{
    private Utility() { } 


    public bool IsInBounds(Vector2 point, Bounds bounds , Vector2 center)
    {
        Vector2 TopRightCorner = center + (Vector2)bounds.extents;
        Vector2 BottomLeftCorner = center - (Vector2)bounds.extents;
        return TopRightCorner.y > point.y && BottomLeftCorner.y < point.y && BottomLeftCorner.x < point.x && TopRightCorner.x > point.x; 
    }
}


