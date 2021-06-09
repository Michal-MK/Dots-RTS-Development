using UnityEngine;

public class Window {

	public enum WindowType {
		Animating,
		Activating,
	}

	public Window(GameObject window, object sender) {
		WindowObject = window;
		WinType = WindowType.Activating;
		WindowTrigger = sender;
	}

	public Window(GameObject window, Animator windowAnimator, bool disableAfterAnim, object sender) {
		WindowObject = window;
		Animator = windowAnimator;
		WinType = WindowType.Animating;
		ShouldDisable = disableAfterAnim;
		WindowTrigger = sender;
	}

	public GameObject WindowObject { get; }

	public WindowType WinType { get; }

	public Animator Animator { get; }

	public bool ShouldDisable { get; }

	public object WindowTrigger { get; }
}