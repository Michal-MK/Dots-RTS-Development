using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	public Text errorText;
	public GameObject[] thingsToDisable = new GameObject[2];

	/// <summary>
	/// Stack of active windows, wince windows tend to "Stack up" may be switched for a list  in the future to allow for removing from the middle.
	/// </summary>
	private static Stack<Window> activeWindows = new Stack<Window>();

	public delegate void WindowChangedHandler(Window changed);
	public static event WindowChangedHandler OnWindowClose;

	private void Awake() {
		if (SceneManager.GetActiveScene().name == Scenes.PLAYER) {
			UM_InGame.OnUpgradeBegin += UM_InGame_OnUpgradeBegin;
			UM_InGame.OnUpgradeQuit += UM_InGame_OnUpgradeQuit;
		}
	}
	private void OnDisable() {
		UM_InGame.OnUpgradeBegin -= UM_InGame_OnUpgradeBegin;
		UM_InGame.OnUpgradeQuit -= UM_InGame_OnUpgradeQuit;
		activeWindows.Clear();
	}

	/// <summary>
	/// Called in play scene to show upgrade panel
	/// </summary>
	private void UM_InGame_OnUpgradeBegin(Upgrade_Manager sender) {
		AddWindow(new Window(UI_ReferenceHolder.MULTI_upgradePanel.gameObject, Window.WindowType.ACTIVATING));
		UI_ReferenceHolder.MULTI_upgradePanel.anchoredPosition = Vector2.zero;
	}

	/// <summary>
	/// Called in play scene to hide upgrade panel
	/// </summary>
	private void UM_InGame_OnUpgradeQuit(Upgrade_Manager sender) {
		UI_ReferenceHolder.MULTI_upgradePanel.anchoredPosition = new Vector2(0, -360);
	}


	// Save Error checking
	public void NoFileFound() {
		StartCoroutine(ReturnToLevelSelectIn(5));
		errorText.gameObject.SetActive(true);
		errorText.text = "No file found, returing to level select";
	}
	IEnumerator ReturnToLevelSelectIn(float seconds) {
		yield return new WaitForSeconds(seconds);
		SceneManager.LoadScene(Scenes.SELECT);
	}


	public void ChangeLayoutToPreview() {
		for (int i = 0; i < thingsToDisable.Length; i++) {
			thingsToDisable[i].SetActive(false);
		}
	}



	public static void AddWindow(Window win) {
		activeWindows.Push(win);
		if (win.type == Window.WindowType.ACTIVATING) {
			if (!win.window.activeInHierarchy) {
				win.window.SetActive(true);
			}
		}
		else {
			win.window.SetActive(true);
			if (!win.animator.GetCurrentAnimatorStateInfo(0).IsName("Show")) {
				win.animator.SetTrigger("Show");
			}
		}
	}

	[Obsolete("Use the more specific version of \"AddWindow\" whenever possible.")]
	public static void AddWindow(GameObject win) {
		Window.WindowType t;
		switch (win.name) {
			case "MenuPanel": {
				t = Window.WindowType.MOVING;
				break;
			}
			case "GameSettingsPanel": {
				t = Window.WindowType.ACTIVATING;
				break;
			}
			case "EditorSettingsPanel": {
				t = Window.WindowType.ACTIVATING;
				break;
			}
			case "SaveOrLoad": {
				t = Window.WindowType.ACTIVATING;
				break;
			}
			case "SavePanel": {
				t = Window.WindowType.ACTIVATING;
				break;
			}
			case "LoadPanel": {
				t = Window.WindowType.ACTIVATING;
				break;
			}
			case "UPGRADE_Selection_To_UI": {
				t = Window.WindowType.ACTIVATING;
				break;
			}
			default: {
				t = Window.WindowType.MOVING;
				break;
			}
		}
		Window w = new Window(win, t);
		AddWindow(w);
	}

	public static void CloseMostRecent() {
		if (activeWindows.Count > 0) {
			Window win = activeWindows.Pop();
			if (win != null) {
				if (win.type == Window.WindowType.ACTIVATING) {
					win.window.SetActive(false);
				}
				else {
					win.animator.SetTrigger("Hide");
					if (win.isFlagedForSwithOff) {
						Control.script.StartCoroutine(DisableAfterAnimation(win.animator));
					}
				}
				OnWindowClose(win);
			}
			else {
				Control.script.Pause();
			}
		}
		else {
			Control.script.Pause();
		}
	}

	public static void CloseMostRecent(int count) {
		if (activeWindows.Count >= count) {
			for (int i = 0; i < count; i++) {
				Window win = activeWindows.Pop();
				if (win != null) {
					if (win.type == Window.WindowType.ACTIVATING) {
						win.window.SetActive(false);
					}
					else {
						win.animator.SetTrigger("Hide");
						if (win.isFlagedForSwithOff) {
							Control.script.StartCoroutine(DisableAfterAnimation(win.animator));
						}
					}
					OnWindowClose(win);
				}
				else {
					Control.script.Pause();
				}
			}
		}
		else {
			print("Invalid count " + count + " is bigger than all active windows " + activeWindows.Count);
			Control.script.Pause();
		}
	}

	private static IEnumerator DisableAfterAnimation(Animator win) {
		win.SetTrigger("Hide");
		yield return new WaitForSecondsRealtime(win.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.05f);

		if (win.GetCurrentAnimatorStateInfo(0).IsName("Hide")) {
			win.gameObject.SetActive(false);
			UI_ReferenceHolder.LE_cellPanel.StartCoroutine(UI_ReferenceHolder.LE_cellPanel.MoveToAnchor(0.25f, 2));
		}
	}

	private static bool isShown = false;

	public static void ToggleWindow(GameObject window) {
		Animator anim;
		try {
			anim = window.GetComponent<Animator>();
			if (isShown) {
				anim.SetTrigger("Hide");
				isShown = false;
			}
			else {
				anim.SetTrigger("Show");
				isShown = true;
			}
		}
		catch (System.NullReferenceException e) {
			print(e.Source + " No Animator Present");

		}
	}

	public static void ToggleWindow(Window win) {
		if (win.type == Window.WindowType.ACTIVATING) {
			win.window.SetActive(!win.window.activeInHierarchy);
		}
		else {
			AnimatorStateInfo s = win.animator.GetCurrentAnimatorStateInfo(0);
			if (s.IsName("Show")) {
				win.animator.SetTrigger("Hide");
			}
			else if (s.IsName("Hide")) {
				win.animator.SetTrigger("Show");
			}
		}
	}

	public void ToggleWindowWrapper(GameObject g) {
		ToggleWindow(g);
	}

	public static void CloseAllActive() {
		foreach (Window win in activeWindows) {
			if (win.type == Window.WindowType.ACTIVATING) {
				win.window.SetActive(false);
			}
			else {
				print(win.window.name);
			}
		}
		activeWindows = new Stack<Window>();
	}
	public static int getWindowCount {
		get {
			return activeWindows.Count;
		}
	}

	public static void CloseWindow(GameObject window) {
		foreach (Window w in activeWindows) {
			if (w.window == window) {

				if (w.type == Window.WindowType.ACTIVATING) {
					w.window.SetActive(false);
				}
				else {
					w.animator.SetTrigger("Hide");
				}
				activeWindows.Pop();
				break;
			}
		}
	}
}

public class Window {

	public enum WindowType {
		MOVING,
		ACTIVATING,
	}

	public Window(GameObject window, WindowType type) {
		_window = window;
		_type = type;
		if (type == WindowType.MOVING) {
			_anim = window.GetComponent<Animator>();
			switch (window.name) {
				case "MenuPanel": {
					_disableAfterMoving = true;
					return;
				}
				default: {
					_disableAfterMoving = false;
					return;
				}
			}
		}
	}

	private GameObject _window;
	private WindowType _type;
	private Animator _anim;
	private bool _disableAfterMoving;

	public GameObject window {
		get { return _window; }
	}

	public WindowType type {
		get { return _type; }
	}

	public Animator animator {
		get { return _anim; }
	}

	public bool isFlagedForSwithOff {
		get { return _disableAfterMoving; }
	}
}
