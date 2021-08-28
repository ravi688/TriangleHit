
using UnityEngine; 

[CreateAssetMenu(fileName = "HexagonPlayerSettings", menuName = "Custom/PlayerSettings/HexagonPlayerSettings")]
public class HexagonPlayerSettings : PlayerCoreSettings
{
    public override string name {  get { return "Hexagon";  } }
}