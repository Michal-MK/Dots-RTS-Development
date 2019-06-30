using UnityEngine;

public class Window {

	public enum WindowType {
		ANIMATING,
		ACTIVATING,
	}

	public Window(GameObject window, object sender) {
		WindowObject = window;
		WinType = WindowType.ACTIVATING;
		WindowTrigger = sender;
	}

	public Window(GameObject window, Animator windowAnimator, bool disableAfterAnim, object sender) {
		WindowObject = window;
		Animator = windowAnimator;
		WinType = WindowType.ANIMATING;
		ShouldDisable = disableAfterAnim;
		WindowTrigger = sender;
	}

	public GameObject WindowObject { get; }

	public bool IsActive => WindowObject.activeInHierarchy;

	public WindowType WinType { get; }

	public Animator Animator { get; }

	public bool ShouldDisable { get; }

	public object WindowTrigger { get; }
}