using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionColiders : MonoBehaviour {

	public RectTransform background;
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
		Camera c = Camera.main;

		yield return new WaitForEndOfFrame();

		ResizeBackground();
		yield return new WaitForSeconds(0.5f);

		top.transform.position = new Vector3(c.transform.position.x, c.transform.position.y + c.orthographicSize);
		bottom.transform.position = new Vector3(c.transform.position.x, c.transform.position.y - c.orthographicSize);
		right.transform.position = new Vector3(c.transform.position.x + c.orthographicSize * c.aspect, c.transform.position.y);
		left.transform.position = new Vector3(c.transform.position.x - c.orthographicSize * c.aspect, c.transform.position.y);

		topC.size = bottomC.size = new Vector2(c.orthographicSize * c.aspect * 2, 1);
		rightC.size = leftC.size = new Vector2(1, c.orthographicSize * 2);


	}

	public void ResizeBackground() {
		print("Hello");
		Camera c = Camera.main;
		background.position = c.transform.position + Vector3.forward * 10;
		transform.position = c.transform.position;

		SpriteRenderer bg = background.GetComponent<SpriteRenderer>();
		bg.size = new Vector2(c.transform.position.x + c.orthographicSize * 2 * c.aspect, c.orthographicSize * 2);

	}

	public static void UpdateTopPoint() {
		Camera c = Camera.main;
		TopRightCameraPointWorld = new Vector2(c.orthographicSize * c.aspect, c.orthographicSize);
	}
}
