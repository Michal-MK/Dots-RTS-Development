using UnityEngine;
using System.Windows.Input;

public class MovePanel : MonoBehaviour {
	public GameObject panel;
	public Animator anim;

	private bool isShown = true;
	// Use this for initialization
	void Start() {
		Control.RMBPressed += Control_RMBPressed;
		//print(isShown);
		anim.SetTrigger("Show");
	}

	private void Control_RMBPressed(Vector2 position) {
		//print("Pressed " + isShown);
		if (!isShown) {
			anim.SetTrigger("Show");
			isShown = true;
		}
		else {
			anim.SetTrigger("Hide");
			isShown = false;
		}
	}
}
