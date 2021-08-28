using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour {

	public float GrabRadius = 1.0f; 

	private Camera view_camera; 

	private TouchEvent grab_event;
	private Vector2 offset; 

	private static int layer_id = 0;

	// Use this for initialization
	void Start () {
		
		grab_event = new TouchEvent(); 
		grab_event.LayerID = (layer_id++);
		grab_event.Condition = IsInsideCircle; 
		grab_event.OnMoved = Move;
		grab_event.OnBegan = UpdateOffset;
		GameManager.GetTouchManager().RegisterEvent(grab_event); 
		view_camera = Camera.main; 

	}

	void UpdateOffset()
	{
		offset = transform.position - view_camera.ScreenToWorldPoint(grab_event.touch.position); 
	}
	void Move()
	{
		transform.position = (Vector2)view_camera.ScreenToWorldPoint(grab_event.touch.position) + offset;  
	}

	bool IsInsideCircle(Vector2 inputPos)
	{
		return (ScreenToWorldPoint_Center_Origin(inputPos) - (Vector2)transform.position).sqrMagnitude <= GrabRadius * GrabRadius;
	}

	Vector2 ScreenToWorldPoint_Center_Origin(Vector2 screenPoint_center_orgin)
	{
		return view_camera.ScreenToWorldPoint(screenPoint_center_orgin + new Vector2(Screen.width, Screen.height) * 0.5f); 
	}		
}
