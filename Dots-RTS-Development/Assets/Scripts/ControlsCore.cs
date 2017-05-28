using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsCore : MonoBehaviour {
	// this contains the reference orientation
	public Vector3 DeviceOrientOrigin;
	// used to make a deadzone
	public float accelerometerDeadzone;

	// At this time this script is used to recognize the orientation of a mobile device and then move the camera based on the delta of the orientation.
	private void Start() {
		RefreshOrientation();
	}


	//Call this to refresh the defalut point of orientation
	public void RefreshOrientation () {
		DeviceOrientOrigin = Input.acceleration;
	}
}
