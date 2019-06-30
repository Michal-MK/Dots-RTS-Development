using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Control : MonoBehaviour {

	#region Delegates

	public delegate void NewSelectionForDownload(SaveFileInfo sender);

	#endregion

	public static bool isPaused = false;
	public static IPauseableScene pauseHandlers;
	public static event EventHandler RMBPressed;
	public static event EventHandler<bool> OnPauseStateChanged;

	public static int DebugSceneIndex = 0;

	#region Initializers

	public static Control Script { get; private set; }

	private void Awake() {
		if (Script == null) {
			Script = this;
			GetComponent<SceneLoader>().Init();
			print("Scene loader init");
			DontDestroyOnLoad(gameObject);
		}
		else if (Script != this) {
			Destroy(gameObject);
		}
	}

	private IEnumerator Start() {

		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

		string activeScene = SceneManager.GetActiveScene().name;

		if (activeScene == Scenes.PROFILES) {
			ProfileManager.Instance.ListProfiles();
		}
		if (ProfileManager.CurrentProfile == null) {
			yield return new WaitUntil(() => Global.baseLoaded);

			if (activeScene == Scenes.SPLASH) {
				SceneManager.LoadScene(Scenes.PROFILES);
			}
			if (activeScene == Scenes.MENU) {
				DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
				SceneManager.LoadScene(Scenes.PROFILES);
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
					DebugSceneIndex = 0;
					SceneManager.LoadScene(Scenes.PROFILES);
					break;
				}
				GameObject.Find("Profile_Name_Menu").GetComponent<TextMeshProUGUI>().SetText("Welcome: " + ProfileManager.CurrentProfile.Name);
				break;
			}
			case Scenes.LEVEL_SELECT: {

				LevelSelectScript.displayedSaves.Clear();
				break;
			}
		}
	}

	public void PauseAttempt(object sender) {
		pauseHandlers?.SetPaused(sender, !isPaused);
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

