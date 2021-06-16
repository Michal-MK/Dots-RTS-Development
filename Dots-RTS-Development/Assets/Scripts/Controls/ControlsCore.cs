using UnityEngine;
using UnityEngine.Serialization;

public class ControlsCore : MonoBehaviour {
	// this contains the reference orientation
	public Vector3 deviceOrientOrigin;
	// used to make a dead-zone
	public float accelerometerDeadzone;
	public Vector3 accelerometerDelta;

	// At this time this script is used to recognize the orientation of a mobile device and then move the camera based on the delta of the orientation.
	private void Start() {
		RefreshOrientation();
	}

	private void Update() {
		if (Vector3.Magnitude(Input.acceleration - deviceOrientOrigin) > accelerometerDeadzone) {
			accelerometerDelta = Input.acceleration - deviceOrientOrigin;
		}
		else {
			accelerometerDelta = Vector2.zero;
		}
		if (Input.GetAxis("Horizontal") != 0) {
			accelerometerDelta.x = Input.GetAxis("Horizontal");
		}
		if (Input.GetAxis("Vertical") != 0) {
			accelerometerDelta.y = Input.GetAxis("Vertical");
		}
	}

	//Call this to refresh the default point of orientation
	public void RefreshOrientation() {
		deviceOrientOrigin = Input.acceleration;
	}
}
