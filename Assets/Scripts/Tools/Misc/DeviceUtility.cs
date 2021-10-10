

using UnityEngine;
using System;

public static class DeviceUtility
{
    public static readonly Vector2 screen_size = new Vector2(Screen.width, Screen.height);
    public static readonly Vector2 screen_size_half;
    static DeviceUtility()
    {
        screen_size_half = screen_size * 0.5f;
    }
}