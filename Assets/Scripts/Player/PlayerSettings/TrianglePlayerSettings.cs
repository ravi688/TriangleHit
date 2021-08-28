
using UnityEngine;

[CreateAssetMenu(fileName = "TrianglePlayerSettings", menuName = "Custom/PlayerSettings/TrianglePlayerSettings")]
public class TrianglePlayerSettings : PlayerCoreSettings
{
    public override string name { get { return "Triangle"; } }
}