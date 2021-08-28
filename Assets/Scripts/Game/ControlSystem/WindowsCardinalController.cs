
using UnityEngine;

public class WindowsCardinalController : IController
{
    public bool IsRunning { get; private set; }
    static readonly Vector2 nullvector = Vector2.zero;
    Vector2 left_dir;
    Vector2 right_dir;
    Vector2 up_dir;
    Vector2 down_dir;
    public Vector2 Axis { get { return (left_dir + right_dir + up_dir + down_dir).normalized; } }
    public WindowsCardinalController()
    {
        IsRunning = true;
    }
    public void Update()
    {
        left_dir = Input.GetKey(KeyCode.LeftArrow) ? Vector2.left : nullvector;
        right_dir = Input.GetKey(KeyCode.RightArrow) ? Vector2.right : nullvector;
        up_dir = Input.GetKey(KeyCode.UpArrow) ? Vector2.up : nullvector;
        down_dir = Input.GetKey(KeyCode.DownArrow) ? Vector2.down : nullvector;
    }
}