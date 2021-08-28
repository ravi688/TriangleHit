
using UnityEngine;

[CreateAssetMenu(fileName = "SquarePlayerSettings", menuName = "Custom/PlayerSettings/SquarePlayerSettings")]
public class SquarePlayerSettings : PlayerCoreSettings
{
    public override string name { get { return "Square"; } }
}