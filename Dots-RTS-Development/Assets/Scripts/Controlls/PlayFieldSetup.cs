using System.Collections;
using UnityEngine;

public class PlayFieldSetup : MonoBehaviour {

	public RectTransform background;
	public GameObject top;
	public GameObject right;
	public GameObject bottom;
	public GameObject left;
	public BoxCollider2D topC;
	public BoxCollider2D rightC;
	public BoxCollider2D bottomC;
	public BoxCollider2D leftC;


	// Use this for initialization
	IEnumerator Start() {
		Camera c = Camera.main;
		if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level_Editor") {
			yield return new WaitForEndOfFrame();
			ResizeBackground(c.aspect);
		}

		yield return new WaitForSeconds(0.5f);

		top.transform.position = new Vector3(c.transform.position.x, c.transform.position.y + c.orthographicSize);
		bottom.transform.position = new Vector3(c.transform.position.x, c.transform.position.y - c.orthographicSize);
		right.transform.position = new Vector3(c.transform.position.x + c.orthographicSize * c.aspect, c.transform.position.y);
		left.transform.position = new Vector3(c.transform.position.x - c.orthographicSize * c.aspect, c.transform.position.y);

		topC.size = bottomC.size = new Vector2(c.orthographicSize * c.aspect * 2, 1);
		rightC.size = leftC.size = new Vector2(1, c.orthographicSize * 2);


	}

	public void ResizeBackground(float refAspect) {
		Camera c = Camera.main;

		if (refAspect == 0) {
			refAspect = c.aspect;
		}
		background.position = c.transform.position + Vector3.forward * 10;
		transform.position = c.transform.position;

		SpriteRenderer bg = background.GetComponent<SpriteRenderer>();
		BoxCollider2D col = background.GetComponent<BoxCollider2D>();
		Vector2 newSize = new Vector2(c.transform.position.x + c.orthographicSize * 2 * refAspect, c.orthographicSize * 2);

		bg.size = newSize;
		background.sizeDelta = newSize;
		col.size = newSize;		
	}
}
