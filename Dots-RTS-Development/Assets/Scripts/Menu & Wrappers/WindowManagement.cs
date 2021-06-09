using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Window;

public class WindowManagement : MonoBehaviour {

	public static WindowManagement Instance { get; private set; }

	private void Start() {
		Instance = this;
		SceneLoader.Instance.OnSceneChanged += SceneChanged;
	}

	private Dictionary<int, Window> activeWindows = new Dictionary<int, Window>();

	private readonly Stack<int> hashOrder = new Stack<int>();

	public event EventHandler<WindowChangeEventArgs> OnWindowChange;

	public int WindowCount => activeWindows.Count;

	public int realWindowCount = 0;

	public void AddWindow(Window win, bool show = true) {
		if (activeWindows.ContainsKey(win.GetHashCode())) {
			return;
		}
		realWindowCount++;
		activeWindows.Add(win.GetHashCode(), win);
		hashOrder.Push(win.GetHashCode());
		if (win.WinType == WindowType.Activating) {
			win.WindowObject.SetActive(show);
		}
		else {
			win.WindowObject.SetActive(show);
			win.Animator.SetTrigger(AnimatorStates.SHOW);
		}
	}

	public void CloseMostRecent() {
		if (activeWindows.Count == 0)
			return;

		int hash = hashOrder.Peek();
		Window win = activeWindows[hash];

		if (win.WinType == WindowType.Activating) {
			win.WindowObject.SetActive(false);
			hashOrder.Pop();
			activeWindows.Remove(hash);
		}
		else {
			CloseAnimatingWindow(win);
		}
		realWindowCount--;
		OnWindowChange?.Invoke(null, new WindowChangeEventArgs { Changed = win, IsOpening = false });
	}

	private IEnumerator DisableAfterAnimation(Window window) {
		Button trigger = window.WindowTrigger as Button;

		window.Animator.SetTrigger(AnimatorStates.HIDE);
		if (trigger != null) {
			trigger.enabled = false;
		}
		yield return new WaitUntil(() => window.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
		window.WindowObject.SetActive(false);
		int hash = hashOrder.Pop();
		activeWindows.Remove(hash);

		if (trigger != null) {
			trigger.enabled = true;
		}
	}

	private void CloseAnimatingWindow(Window win) {
		win.Animator.SetTrigger(AnimatorStates.HIDE);
		if (win.ShouldDisable) {
			StartCoroutine(DisableAfterAnimation(win));
		}
	}


	private void SceneChanged(object sender, EventArgs e) {
		activeWindows.Clear();
	}

	public void CloseAll() {
		foreach (int winHash in activeWindows.Keys) {
			Window win = activeWindows[winHash];

			if (win.WinType == WindowType.Activating) {
				win.WindowObject.SetActive(false);
				int hash = hashOrder.Pop();
			}
			else {
				CloseAnimatingWindow(win);
			}
			realWindowCount--;
			OnWindowChange?.Invoke(null, new WindowChangeEventArgs { Changed = win, IsOpening = false });
		}
		activeWindows.Clear();
		realWindowCount = 0;
		Time.timeScale = 1;
		Control.Script.PauseAttempt(null);
	}

	public void CloseWindow(GameObject window) {
		foreach (int winHash in activeWindows.Keys) {
			Window win = activeWindows[winHash];
			if (win.WindowObject == window) {
				if (win.WinType == WindowType.Activating) {
					win.WindowObject.SetActive(false);
				}
				else {
					CloseAnimatingWindow(win);
				}
				activeWindows = new Dictionary<int, Window>(activeWindows.Where(w => w.Value.WindowObject != window).ToDictionary(w => w.Key, w => w.Value));
				realWindowCount--;
				OnWindowChange?.Invoke(null, new WindowChangeEventArgs { Changed = win, IsOpening = false });
			}
		}
	}
}

