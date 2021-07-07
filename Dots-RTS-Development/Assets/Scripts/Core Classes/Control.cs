using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Control : MonoBehaviour {

	public static bool isPaused;
	public static IPauseableScene pauseHandlers;
	public static event EventHandler RMBPressed;
	public static event EventHandler<bool> OnPauseStateChanged;

	#region Initializers

	public static Control Script { get; private set; }

	private void Awake() {
		if (Script == null) {
			Script = this;
			GetComponent<SceneLoader>().Init();
			print("Control script initialized.");
			DontDestroyOnLoad(gameObject);
		}
		else if (Script != this) {
			Destroy(gameObject);
		}
	}

	private IEnumerator Start() {
		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

		string activeScene = SceneManager.GetActiveScene().name;

		if (activeScene == Scenes.SPLASH) {
			yield return new WaitUntil(() => Global.baseLoaded);
			SceneManager.LoadScene(Scenes.PROFILES);
			yield break;
		}

		// DEBUG
		if (Application.isEditor) {
			if (ProfileManager.CurrentProfile == null) {
				if (activeScene == Scenes.MENU) {
					SceneManager.LoadScene(Scenes.PROFILES);
				}
			}
		}
	}

	private void OnDestroy() {
		SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
	}

	#endregion

	private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {
		switch (newS.name) {
			case Scenes.PROFILES: {
				ProfileManager.Instance.ListProfiles();
				break;
			}
			case Scenes.MENU: {

				if (ProfileManager.CurrentProfile == null) {
					SceneManager.LoadScene(Scenes.PROFILES);
					break;
				}
				GameObject.Find("Profile_Name_Menu").GetComponent<TextMeshProUGUI>().SetText("Welcome: " + ProfileManager.CurrentProfile.Name);
				break;
			}
			case Scenes.LEVEL_SELECT: {

				LevelSelectScript.DISPLAYED_SAVES.Clear();
				break;
			}
		}
	}

	public void PauseAttempt(object sender) {
		if (isPaused) {
			pauseHandlers?.Unpause(sender);
		}
		else {
			pauseHandlers?.Pause(sender);
		}
		isPaused ^= true;
		OnPauseStateChanged?.Invoke(this, isPaused);
	}

	private void Update() {
		if (Input.GetMouseButtonDown(1)) {
			RMBPressed?.Invoke(Input.mousePosition, null);
		}
	}

	private void LateUpdate() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			PauseAttempt(KeyCode.Escape);
		}
	}
}
