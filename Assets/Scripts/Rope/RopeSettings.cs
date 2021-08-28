using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName ="RopeSettings",menuName = "Custom/RopeSettings")]
public class RopeSettings : ScriptableObject
{
    public Color Color = Color.green;
    public Sprite HookSprite;
    public float RopeWidth = 0.5f;
    public float HookScale = 1.0f; 
    public float BreakReactionForce = 50.0f ;
    public float FadeDuration = 0.5f;
    public float Frequency = 1;
    public float DampingRatio = 1; 
}
