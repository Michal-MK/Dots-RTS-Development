using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTheCamera : MonoBehaviour {
	//This script is used to move the camera with the mouse and WASD keys (Eventually using the accelerometer when on mobile)
	
	//Scales the movement of the camera
	float SpeedOfMotion = 1000f;
	// 
	public ControlsCore Cc;
	void Start () {
		
	}
	
	// 
	void Update () {
		//MoveTheCamera
		Vector2 ApplyMovementAtTheEnd = new Vector2(Cc.accelerometerDelta.x, Cc.accelerometerDelta.y);
		transform.position = transform.position + (Vector3)ApplyMovementAtTheEnd * Time.deltaTime * SpeedOfMotion; 
		
	}
}
