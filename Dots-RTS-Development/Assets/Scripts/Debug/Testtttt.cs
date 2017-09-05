using UnityEngine;
using System.Collections;

public class Testtttt : MonoBehaviour {
	public GameObject panel;
	public Animator anim;
	public float yPos;

	private bool isShown = true;
	// Use this for initialization
	new RectTransform transform;
	void Start() {
		Control.RMBPressed += Control_RMBPressed;
		transform = GetComponent<RectTransform>();

	}

	private void Control_RMBPressed(Vector2 position) {
		if (!isShown) {
			anim.SetTrigger("Show");
			isShown = true;
		}
		else {
			anim.SetTrigger("Hide");
			isShown = false;
		}
	}

	private void LateUpdate() {
		if (yPos != 0 && yPos != 200) {
			//print(transform.anchoredPosition - new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y + yPos));
			transform.anchoredPosition = (transform.anchoredPosition - new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y - yPos));
		}
	}


}
