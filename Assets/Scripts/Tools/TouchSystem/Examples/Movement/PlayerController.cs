using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public JoystickSettings JoystickSettings;
	public float MaxForce; 

	private JoyStick joystick;
	private Rigidbody2D body;  
	private void Start()
	{
		joystick = JoyStick.CreateJoyStick(JoystickSettings, 0);
		body = GetComponent<Rigidbody2D>(); 
	}


	private void Update()
	{
		joystick.Update();

		body.AddForce(MaxForce * joystick.Axis); 
	}
}
