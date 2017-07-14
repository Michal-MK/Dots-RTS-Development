using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Control : MonoBehaviour {
	public delegate void TeamChangeEventHandler(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current);

	public delegate void EnteredCellEditMode(EditCell sender);

	public delegate void PanelValueChanged();
	public delegate void EditModeChanged(LevelEditorCore.Mode mode);
	public delegate void NewSelectionForDownload(SaveFileInfo sender);

	public static List<CellBehaviour> cells = new List<CellBehaviour>();

	private static bool isPaused = false;

	private static Control script;

	public GameObject menuPanel;

	#region Initializers
	private void Awake() {
		if (script == null) {
			script = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (script != this) {
			Destroy(gameObject);
		}
	}
	private void Start() {
		DontDestroyOnLoad(this);
		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
	}
	private void OnDestroy() {
		SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
	}
	#endregion


	private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {
		if (newS.buildIndex == 3 || newS.buildIndex == 5) {
			menuPanel = GameObject.Find("MenuPanel");
			menuPanel.SetActive(false);
		}
	}

	private void Update() {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isPaused) {
				UnPause();
			}
			else {
				Pause();
			}

		}
	}


	public static void Pause() {
		isPaused = true;
		Time.timeScale = 0;
	}

	public static void UnPause() {
		isPaused = false;
		Time.timeScale = 1;
	}


	public static void GameOver() {
		print("You Lost");
	}

	public static void YouWon() {
		print("You Won");
	}
}

