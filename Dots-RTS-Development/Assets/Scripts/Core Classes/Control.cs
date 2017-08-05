using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Events;

public class Control : MonoBehaviour {

	#region Delegates
	public delegate void TeamChangeEventHandler(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current);
	public delegate void EnteredCellEditMode(EditCell sender);
	public delegate void PanelValueChanged();
	public delegate void EditModeChanged(LevelEditorCore.Mode mode);
	public delegate void NewSelectionForDownload(SaveFileInfo sender);

	public delegate void EnteredUpgradeMode(Upgrade_Manager sender);
	public delegate void QuitUpgradeMode(Upgrade_Manager sender);
	#endregion

	public static List<CellBehaviour> cells = new List<CellBehaviour>();

	public static bool isPaused = false;
	private static float time;

	public static GameObject menuPanel;

	public static int DebugSceneIndex = 0;

	public static CampaignLevel currentCampaignLevel;

	private bool isInGame = false;

	public GameObject profileVis;
	public static ProfileManager pM;


	#region Post-Game data
	private static bool isWinner = true;
	private static bool domination = false;
	private static string gameTime;
	#endregion

	#region Initializers
	private static Control script;
	private void Awake() {
		if (script == null) {
			script = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (script != this) {
			Destroy(gameObject);
		}

		if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves");
		}
		if (!Directory.Exists(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves")) {
			Directory.CreateDirectory(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves");
		}
		if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles")) {
			Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles");
		}
	}

	private void Start() {
		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

		int activeScene = SceneManager.GetActiveScene().buildIndex;

		if (activeScene == 5 || activeScene == 3) {
			menuPanel = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
			time = 0;
			isInGame = true;
			StartCoroutine(GameState());
		}
		if (SceneManager.GetActiveScene().name == "Profiles") {
			if (pM == null) {
				pM = new ProfileManager(profileVis, GameObject.Find("Content").transform);
				pM.ListProfiles();
			}
		}
		if (ProfileManager.currentProfile == null && activeScene != 5) {
			if (SceneManager.GetActiveScene().name == "Profiles") {
				DebugSceneIndex = 0;
			}
			else {
				DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
			}
			if(SceneManager.GetActiveScene().name != "Profiles"){
				SceneManager.LoadScene("Profiles");
			}
		}
	}

	private void OnDestroy() {
		SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
	}
	#endregion


	private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {
		if (ProfileManager.currentProfile == null && newS.name != "Profiles") {
			DebugSceneIndex = newS.buildIndex;
			SceneManager.LoadScene("Profiles");

		}

		if (newS.buildIndex == 0) {
			GameObject.Find("Profile").GetComponent<TextMeshProUGUI>().SetText("Welcome: " + ProfileManager.currentProfile.profileName);
			currentCampaignLevel = null;
			isInGame = false;

		}

		if (newS.buildIndex == 1) {
			currentCampaignLevel = null;
			isInGame = false;

		}

		if (newS.buildIndex == 2) {
			LevelSelectScript.displayedSaves.Clear();
			currentCampaignLevel = null;
			isInGame = false;

		}

		if (newS.buildIndex == 3) {
			menuPanel = GameObject.Find("Canvas").transform.Find("MenuPanel").gameObject;
			StartCoroutine(GameState());
		}


		if(newS.buildIndex == 4) {
			isInGame = false;

		}

		if (newS.buildIndex == 5) {
			isInGame = true;

		}


		if (newS.buildIndex == 6) {
			isInGame = false;

			TextMeshProUGUI res = GameObject.Find("Result").GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI dom = GameObject.Find("Domination").GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI time = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();

			if (isWinner) {
				res.text = "You Won!";
			}
			else {
				res.text = "You Lost!";
			}

			if (domination) {
				dom.text = "Dominated all cells";
			}
			else {
				dom.text = "";
			}
			time.text = "Fought for: " + gameTime;
		}

		if (SceneManager.GetActiveScene().name == "Profiles") {

			if (pM == null) {
				print("Azzz");
				pM = new ProfileManager(profileVis, GameObject.Find("Content").transform);
				pM.ListProfiles();
			}
		}

		Time.timeScale = 1;
		time = 0;
	}

	private void Update() {
		if (isInGame) {
			time += Time.deltaTime;
		}

		if (SceneManager.GetActiveScene().buildIndex != 1) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (isPaused) {
					UnPause();
				}
				else {
					if (Upgrade_Manager.isUpgrading) {
						Upgrade_Manager.isUpgrading = false;
					}
					else {
						Pause();
					}
				}
			}
		}
	}

	public IEnumerator GameState() {
		isInGame = true;

		print("Start");
		print((SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 3) && isInGame);

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
				if (SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 3) {
					YouWon();
				}
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
				if (SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 3) {
					GameOver();
				}
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
		else {
			domination = false;
		}
		gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));

		if(currentCampaignLevel != null) {
			currentCampaignLevel.MarkLevelAsPassed(time);
		}
	}
}

