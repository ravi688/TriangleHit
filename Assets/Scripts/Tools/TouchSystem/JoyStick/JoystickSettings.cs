
using UnityEngine;
using System;


[CreateAssetMenu(menuName = "TouchSystem/JoyStick Settings", fileName = "JoyStick Settings", order = 0)]
public class JoystickSettings : ScriptableObject
{
	public Vector2 position { get { Vector2 pos = new Vector2(); GetCalculatedPosition(ref pos); return pos; } }
    public Sprite HandleSprite;
    public Sprite BodySprite;
    public Vector2 BodySize = new Vector2(100, 100);
    public Vector2 HandleSize = new Vector2(80, 80);
    public bool IsInteractive = true;
    public float Sensitivity = 1.0f;
    public float HandleReturnSpeed = 2.0f;
    public int Threshold_distance = 10;
	public Vector2 offset = new Vector2(0, 0); 
	public HandConfiugartion UseHand = HandConfiugartion.RightHanded; 

	private static Vector2 ScreenSize = new Vector2(Screen.width, Screen.height); 

	public void GetCalculatedPosition(ref Vector2 position)
	{
		if (UseHand == HandConfiugartion.RightHanded) {
			position.x = ScreenSize.x * 0.5f - offset.x - BodySize.x * 0.5f; 
			position.y = -ScreenSize.y * 0.5f + offset.y + BodySize.y * 0.5f;
		} else {
			position.x = -(ScreenSize.x * 0.5f - offset.x - BodySize.x * 0.5f); 
			position.y = -ScreenSize.y * 0.5f + offset.y + BodySize.y * 0.5f;
		}
	}
}
