using UnityEngine;
using System.Collections;
using System;

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

	private void Control_RMBPressed(object _, EventArgs __) {
		if (!isShown) {
			anim.SetTrigger(AnimatorStates.SHOW);
			isShown = true;
		}
		else {
			anim.SetTrigger(AnimatorStates.HIDE);
			isShown = false;
		}
	}

	private void LateUpdate() {
		if (yPos != 0 && yPos != 200) {
			transform.anchoredPosition = (transform.anchoredPosition - new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y - yPos));
		}
	}
}
