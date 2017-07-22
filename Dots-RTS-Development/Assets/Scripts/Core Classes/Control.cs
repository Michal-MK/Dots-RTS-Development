using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control : MonoBehaviour {
	public delegate void TeamChangeEventHandler(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current);

	public delegate void EnteredCellEditMode(EditCell sender);

	public delegate void PanelValueChanged();
	public delegate void EditModeChanged(LevelEditorCore.Mode mode);
	public delegate void NewSelectionForDownload(SaveFileInfo sender);

	public static List<CellBehaviour> cells = new List<CellBehaviour>();

	public static bool isPaused = false;

	private static Control script;

	public static GameObject menuPanel;

	private static float time;
	private bool isInGame = false;

	#region Post-Game data
	private static bool isWinner = true;
	private static bool domination = false;
	private static string gameTime;
	#endregion

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
		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
		if (SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 3) {
			menuPanel = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
			time = 0;
			isInGame = true;
			StartCoroutine(GameState());
		}

	}
	private void OnDestroy() {
		SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
	}
	#endregion


	private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {
		if (newS.buildIndex == 3 || newS.buildIndex == 5) {
			menuPanel = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
			StartCoroutine(GameState());
		}
		Time.timeScale = 1;
		time = 0;
		if (newS.buildIndex == 6) {
			Text res = GameObject.Find("Result").GetComponent<Text>();
			Text dom = GameObject.Find("Domination").GetComponent<Text>();
			Text time = GameObject.Find("Time").GetComponent<Text>();

			if (isWinner) {
				res.text = "You Won!";
			}
			else {
				res.text = "You Lost!";
			}

			if (domination) {
				dom.text = "Dominated all cells";
			}
			time.text = gameTime;
		}
	}


	private void Update() {
		if (isInGame) {
			time += Time.deltaTime;
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (isPaused) {
				UnPause();
			}
			else {
				Pause();
			}

		}
	}

	public IEnumerator GameState() {
		isInGame = true;

		while ((SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 3) && isInGame) {
			yield return new WaitForSeconds(1);
			int activeAIs = 0;

			for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
				if (Initialize_AI.AIs[i] != null) {
					if (Initialize_AI.AIs[i].isActive) {
						activeAIs++;
					}
				}
			}
			print(activeAIs);
			if (activeAIs == 0) {
				yield return new WaitForSeconds(2);
				YouWon();
				isInGame = false;
			}

			int playerCells = 0;

			for (int i = 0; i < cells.Count; i++) {
				if (cells[i].cellTeam == Cell.enmTeam.ALLIED) {
					playerCells++;
				}
			}
			print(playerCells);
			if (playerCells == 0) {
				yield return new WaitForSeconds(2);
				GameOver();
				isInGame = false;
			}
		}
	}

	public static void Pause() {
		isPaused = true;
		Time.timeScale = 0;
		menuPanel.SetActive(true);
	}

	public static void UnPause() {
		isPaused = false;
		Time.timeScale = 1;
		menuPanel.SetActive(false);
	}


	public static void GameOver() {
		SceneManager.LoadScene(6);
		isWinner = false;
		gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));
	}

	public static void YouWon() {
		SceneManager.LoadScene(6);
		isWinner = true;
		int neutrals = 0;
		foreach (CellBehaviour c in cells) {
			if (c.cellTeam == Cell.enmTeam.NEUTRAL) {
				neutrals++;
			}
		}
		if (neutrals == 0) {
			domination = true;
		}
		else { domination = false;
		}
		gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));
	}
}

