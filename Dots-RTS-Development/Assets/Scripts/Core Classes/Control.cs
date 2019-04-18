using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control : MonoBehaviour {

	#region Delegates
	public delegate void TeamChanged(GameCell sender, Team previous, Team current);
	public delegate void CellSelected(EditCell sender);
	public delegate void PanelValueChanged(LevelEditorCore.PCPanelAttribute attribute);
	public delegate void EditModeChanged(LevelEditorCore.Mode mode);
	public delegate void NewSelectionForDownload(SaveFileInfo sender);

	public delegate void EnteredUpgradeMode(UM_InGame sender);
	public delegate void QuitUpgradeMode(UM_InGame sender);
	public delegate void InstallUpgradeHandler(Upgrades type, UM_InGame cell);

	public delegate void OnMouseButtonPressed(Vector2 position);

	#endregion

	public static bool isPaused = false;
	public static bool canPause = true;
	public static event OnMouseButtonPressed RMBPressed;

	public static int DebugSceneIndex = 0;


	#region Initializers
	public static Control Script { get; set; }
	private void Awake() {
		if (Script == null) {
			Script = this;
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

			case Scenes.LEVEL_EDITOR: {

				break;
			}

			case Scenes.LEVEL_SELECT: {

				LevelSelectScript.displayedSaves.Clear();

				break;
			}
		}
	}

	private void Update() {
		if (Input.GetKey(KeyCode.LeftShift)) {
			if (Input.GetKeyDown(KeyCode.C)) {
				StartCoroutine(TakePicture());
			}
		}
		if (Input.GetMouseButtonDown(1)) {
			if (RMBPressed != null) {
				RMBPressed(Input.mousePosition);
				print("Pressed RMB");
			}
		}
	}

	private void LateUpdate() {
		if (Input.GetKeyDown(KeyCode.Escape) && IsInPausebleScene()) {
			if (isPaused) {
				UnPause();
			}
			else {
				Pause();
			}
		}
	}

	public void Pause() {
		if (UI_Manager.getWindowCount == 0) {
			isPaused = true;
			Time.timeScale = 0;
			UI_Manager.AddWindow(new Window(UI_ReferenceHolder.MULTI_menuPanel, Window.WindowType.MOVING));
			if (SceneManager.GetActiveScene().name == Scenes.LEVEL_EDITOR) {

				foreach (Image img in UI_ReferenceHolder.MULTI_menuPanel.GetComponentsInChildren<Image>()) {
					img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
				}
				foreach (TextMeshProUGUI text in UI_ReferenceHolder.MULTI_menuPanel.GetComponentsInChildren<TextMeshProUGUI>()) {
					text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
				}
				UI_ReferenceHolder.MULTI_menuPanel.GetComponent<Animator>().SetTrigger("Show");

				AnimatorStateInfo s = UI_ReferenceHolder.LE_editorSliderPanel.GetCurrentAnimatorStateInfo(0);

				if (!s.IsName("Hide") && !s.IsName("Def")) {
					UI_ReferenceHolder.LE_editorSliderPanel.SetTrigger("Hide");
				}
				UI_ReferenceHolder.LE_cellPanel.ToggleControlsPanel();
			}
		}
		else {
			UI_Manager.CloseMostRecent();
		}
	}

	public void UnPause() {
		isPaused = false;
		Time.timeScale = 1;
		UI_Manager.CloseMostRecent();
	}

	IEnumerator TakePicture() {
		if (!File.Exists(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "EnterDirToStoreImages.txt")) {
			print("NOPE");
			yield break;
		}
		else {
			if (SceneManager.GetActiveScene().name == Scenes.GAME) {
				string path = File.ReadAllText(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "EnterDirToStoreImages.txt");
				GameObject canvas = GameObject.Find("Canvas");
				canvas.SetActive(false);
				yield return new WaitForSeconds(0.1f);
				ScreenCapture.CaptureScreenshot(path + DateTime.Now.ToFileTime() + ".png");
				yield return new WaitForSeconds(0.1f);
				canvas.SetActive(true);
			}
		}
	}

	private bool IsInPausebleScene() {
		string s = SceneManager.GetActiveScene().name;
		if (s == Scenes.LEVEL_EDITOR || s == Scenes.GAME || s == Scenes.DEBUG) {
			return true;
		}
		else {
			return false;
		}
	}
}

