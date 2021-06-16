using UnityEngine;

public class MoveTheCamera : MonoBehaviour {
	//This script is used to move the camera with the mouse and WASD keys (Eventually using the accelerometer when on mobile)
	
	//Scales the movement of the camera
	private const float SPEED_OF_MOTION = 1000f;
	
	public ControlsCore cc;
	
	private void Update () {
		//MoveTheCamera
		Vector2 applyMovementAtTheEnd = new Vector2(cc.accelerometerDelta.x, cc.accelerometerDelta.y);
		transform.position += (Vector3)applyMovementAtTheEnd * (Time.deltaTime * SPEED_OF_MOTION);
	}
}
