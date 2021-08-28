using UnityEngine;
using System;

public class Clickable<T> : IManualUpdatable
{
    public static bool IsAnyOverlapping = false;
    
    public bool IsRunning { get { return !IsAnyOverlapping; } set { IsAnyOverlapping = !value; } }
    public T args;
    public Action<T> OnClick { get; set; }

    private bool Istrigger;
    private Vector2 prevMousePos;
    private readonly Transform transform;
    private readonly int radius;
    public Clickable(Transform transform, int radius = 100)
    {
        this.transform = transform;
        this.radius = radius;
    }
    public virtual void Update()
    {
        bool istrue = Conditions.IsInCircle(radius, transform.position, InputManager.GetInputPosition());
       
        if (InputManager.IsInputDown() && istrue)
        {
            Istrigger = true;
            prevMousePos = InputManager.GetInputPosition();
        }
        if (Istrigger && InputManager.IsInputUp() && !IsMouseMoved())
        {
            
            if (OnClick != null)
                OnClick(args);
            Istrigger = false;
        }
    }
    private bool IsMouseMoved()
    {
        return prevMousePos != InputManager.GetInputPosition();
    }
}