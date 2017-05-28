using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTheCamera : MonoBehaviour {
	//This script is used to move the camera with the mouse and WASD keys (Eventually using the accelerometer when mobile)
	
	//Scales the movement of the camera
	float SpeedOfMotion = 1f;
	public ControlsCore cC;
	// 
	void Start () {
		
	}
	
	// 
	void Update () {
		//MoveTheCamera
		Vector2 ApplyMovementAtTheEnd = Vector2.zero;

		if (Vector3.Magnitude(Input.acceleration - cC.DeviceOrientOrigin) > cC.accelerometerDeadzone) {
			
		}
	}
}
