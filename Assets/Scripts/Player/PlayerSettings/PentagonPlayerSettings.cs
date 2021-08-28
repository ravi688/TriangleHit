
using UnityEngine;

[CreateAssetMenu(fileName = "PentagonPlayerSettings", menuName = "Custom/PlayerSettings/PentagonPlayerSettings")]
public class PentagonPlayerSettings : PlayerCoreSettings
{
    public override string name { get { return "Pentagon"; } }
}