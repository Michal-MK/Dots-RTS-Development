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

	// Use this for initialization
	void Start() {
		transform.position = Vector3.zero;

		Camera c = Camera.main;

		top.transform.position = new Vector3(0, c.orthographicSize);
		bottom.transform.position = new Vector3(0, -c.orthographicSize);
		right.transform.position = new Vector3(c.orthographicSize * c.aspect, 0);
		left.transform.position = new Vector3(-c.orthographicSize * c.aspect, 0);

		topC.size = bottomC.size = new Vector2(c.orthographicSize * c.aspect, 1);
		rightC.size = leftC.size = new Vector2(c.orthographicSize, 1);
	}


}
