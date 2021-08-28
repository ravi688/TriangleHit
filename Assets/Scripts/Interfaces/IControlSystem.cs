using UnityEngine;
public interface IControlSystem : IManualUpdatable
{
    IController AxisController { get;}
    bool DashButton { get; }
    Vector2 TouchLocation { get;  }
}
