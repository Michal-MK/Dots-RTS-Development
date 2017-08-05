using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionColiders : MonoBehaviour {

	public GameObject top;
	public GameObject right;
	public GameObject bottom;
	public GameObject left;
	public BoxCollider2D topC;
	public BoxCollider2D rightC;
	public BoxCollider2D bottomC;
	public BoxCollider2D leftC;

	public static Vector2 TopRightCameraPointWorld = Vector2.zero;
	public static Vector2 TopRightCanvasPoint = Vector2.zero;

	// Use this for initialization
	IEnumerator Start() {
		transform.position = Vector3.zero;

		Camera c = Camera.main;

		yield return new WaitForSeconds(0.5f);
		top.transform.position = new Vector3(0, c.orthographicSize);
		bottom.transform.position = new Vector3(0, -c.orthographicSize);
		right.transform.position = new Vector3(c.orthographicSize * c.aspect, 0);
		left.transform.position = new Vector3(-c.orthographicSize * c.aspect, 0);

		topC.size = bottomC.size = new Vector2(c.orthographicSize * c.aspect * 2, 1);
		rightC.size = leftC.size = new Vector2(c.orthographicSize * 2, 1);

	}
	public static void UpdateTopPoint() {
		Camera c = Camera.main;
		TopRightCameraPointWorld = new Vector2(c.orthographicSize * c.aspect, c.orthographicSize);
	}
}
