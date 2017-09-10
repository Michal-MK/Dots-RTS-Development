using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using System;
using UnityEngine.UI;

public class Control : MonoBehaviour {

	#region Delegates
	public delegate void TeamChanged(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current);
	public delegate void CellSelected(EditCell sender);
	public delegate void PanelValueChanged(LevelEditorCore.PCPanelAttribute attribute);
	public delegate void EditModeChanged(LevelEditorCore.Mode mode);
	public delegate void NewSelectionForDownload(SaveFileInfo sender);

	public delegate void EnteredUpgradeMode(Upgrade_Manager sender);
	public delegate void QuitUpgradeMode(Upgrade_Manager sender);
	public delegate void InstallUpgradeHandler(Upgrade.Upgrades type, Upgrade_Manager cell);

	public delegate void OnMouseButtonPressed(Vector2 position);


	#endregion

	//public static List<CellBehaviour> cells = new List<CellBehaviour>();

	public static bool isPaused = false;
	public static bool canPause = true;
	public static event OnMouseButtonPressed RMBPressed;

	private float time;

	private bool isInGame = false;

	public static ProfileManager pM;


	public static int DebugSceneIndex = 0;


	#region Prefab
	public GameObject profileVis;
	#endregion

	#region Initializers
	public static Control script;
	private void Awake() {
		if (script == null) {
			script = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (script != this) {
			Destroy(gameObject);
		}
	}

	private IEnumerator Start() {

		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

		string activeScene = SceneManager.GetActiveScene().name;

		if (activeScene == Scenes.PROFILES) {
			if (pM == null) {
				pM = new ProfileManager(profileVis, GameObject.Find("Content").transform);
				pM.ListProfiles();
			}
		}
		if (ProfileManager.getCurrentProfile == null) {
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

			case Scenes.SPLASH: {

				break;
			}

			case Scenes.PROFILES: {

				pM = new ProfileManager(profileVis, GameObject.Find("Content").transform);
				pM.ListProfiles();
				break;
			}

			case Scenes.MENU: {

				if (ProfileManager.getCurrentProfile == null) {
					DebugSceneIndex = 0;
					SceneManager.LoadScene(Scenes.PROFILES);
					break;
				}
				GameObject.Find("Profile_Name_Menu").GetComponent<TextMeshProUGUI>().SetText("Welcome: " + ProfileManager.getCurrentProfile.profileName);

				break;
			}

			case Scenes.EDITOR: {

				break;
			}

			case Scenes.SELECT: {

				LevelSelectScript.displayedSaves.Clear();

				break;
			}

			case Scenes.PLAYER: {

				break;
			}

			case Scenes.SHARING: {

				break;
			}

			case Scenes.DEBUG: {

				break;
			}

			case Scenes.POSTG: {

				break;
			}

			case Scenes.SHOP: {

				break;
			}
		}

		Time.timeScale = 1;
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
	private IEnumerator DisableAfterAnimation() {
		Animator anim = UI_ReferenceHolder.MULTI_menuPanel.GetComponent<Animator>();
		anim.SetTrigger("Hide");
		yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length - 0.1f);
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hide")) {
			anim.gameObject.SetActive(false);
			UI_ReferenceHolder.LE_cellPanel.StartCoroutine(UI_ReferenceHolder.LE_cellPanel.MoveToAnchor(0));
		}
	}

	public void Pause() {
		if (UI_Manager.getWindowCount == 0) {
			isPaused = true;
			Time.timeScale = 0;
			UI_Manager.AddWindow(UI_ReferenceHolder.MULTI_menuPanel);

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

			UI_ReferenceHolder.LE_cellPanel.StartCoroutine(UI_ReferenceHolder.LE_cellPanel.MoveToAnchor(0));
		}
		else {
			UI_Manager.CloseMostRecent();
		}
	}

	public void UnPause() {
		isPaused = false;
		Time.timeScale = 1;
		StartCoroutine(DisableAfterAnimation());
	}

	IEnumerator TakePicture() {
		if (!File.Exists(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "EnterDirToStoreImages.txt")) {
			print("NOPE");
			yield break;
		}
		else {
			if (SceneManager.GetActiveScene().name == Scenes.PLAYER) {
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
		if (s == Scenes.EDITOR || s == Scenes.PLAYER) {
			return true;
		}
		else {
			return false;
		}
	}
}

