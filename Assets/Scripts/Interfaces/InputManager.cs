
using UnityEngine;




public static class InputManager
{
    public static int reserved_finger_id = -1;
    public static bool isInputPresent
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
                return Input.touchCount > 0;
            else
            {
                bool isPresent;
                isPresent = Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height
                            && Input.mousePosition.x > 0 && Input.mousePosition.x > 0;
                return isPresent;
            }
        }
    }
    public static Touch GetReservedTouch()
    {
        Touch out_touch = new Touch();
        if (reserved_finger_id == -1)
        {
            out_touch = Input.GetTouch(0);
            reserved_finger_id = out_touch.fingerId;
        }
        else
        {
            Touch[] touches = Input.touches;
            for (int i = 0; i < Input.touchCount; i++)
                if (touches[i].fingerId == reserved_finger_id)
                    out_touch = touches[i];
                else
                    continue;
        }
        return out_touch;
    }
    public static Touch GetAvailableTouch()
    {
        Touch out_touch = new Touch();
        if (reserved_finger_id == -1)
        {
            out_touch = Input.GetTouch(0);
            reserved_finger_id = out_touch.fingerId;
        }
        else
        {
            Touch[] touches = Input.touches;
            for (int i = 0; i < Input.touchCount; i++)
                if (touches[i].fingerId != reserved_finger_id)
                    out_touch = touches[i];
                else
                    continue;

        }
        return out_touch;
    }
    public static Vector2 GetInputPosition()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
                return Input.GetTouch(0).position;
            else
                return Vector2.zero; 
        }
        else if (Input.mousePresent)
            return Input.mousePosition;
        return new Vector2(-1000, -1000);
    }
    public static bool IsInputMoved()
    {
        if (Application.platform == RuntimePlatform.Android)
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                    return true;
                else
                    return false;
            }
        return false;
    }
    public static bool IsInputStationary()
    {
        if (Application.platform == RuntimePlatform.Android)
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Stationary)
                    return true;
                else
                    return false;
            }
        return true;
    }
    public static bool IsInput()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    return true;
                else
                    return false;
            }
        }
        else return Input.GetMouseButton(0);
        return false;
    }
    public static bool IsInputDown()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                    return true;
                else
                    return false;
            }
        }
        else
            return Input.GetMouseButtonDown(0);
        return false;
    }
    public static bool IsInputUp()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                    return true;
                else
                    return false;
            }
        }
        else
            return Input.GetMouseButtonUp(0);
        return false;
    }
}
