using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class Control : MonoBehaviour {

	#region Delegates
	public delegate void TeamChanged(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current);
	public delegate void EnteredCellEditMode(EditCell sender);
	public delegate void PanelValueChanged(LevelEditorCore.PCPanelAttribute attribute);
	public delegate void EditModeChanged(LevelEditorCore.Mode mode);
	public delegate void NewSelectionForDownload(SaveFileInfo sender);

	public delegate void EnteredUpgradeMode(Upgrade_Manager sender);
	public delegate void QuitUpgradeMode(Upgrade_Manager sender);
	#endregion

	public static List<CellBehaviour> cells = new List<CellBehaviour>();

	public static bool isPaused = false;
	public static bool canPause = true;

	private float time;

	private bool isInGame = false;
	public static PlaySceneState levelState = PlaySceneState.NONE;
	public enum PlaySceneState {
		NONE,
		CAMPAIGN,
		CUSTOM,
		PREVIEW
	}
	public static ProfileManager pM;


	public static int DebugSceneIndex = 0;


	#region Prefab
	public GameObject profileVis;
	#endregion

	#region Post-Game data
	private static bool isWinner = true;
	private static bool domination = false;
	private static string gameTime;
	private static int totalCoinsAwarded;
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

	private IEnumerator Start() {

		SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

		string activeScene = SceneManager.GetActiveScene().name;

		if (activeScene == Scenes.PLAYER) {
			isInGame = true;
			if (levelState != PlaySceneState.PREVIEW) {
				StartCoroutine(GameState());
			}

		}
		if (activeScene == Scenes.PROFILES) {
			if (pM == null) {
				pM = new ProfileManager(profileVis, GameObject.Find("Content").transform);
				pM.ListProfiles();
			}
		}
		if (ProfileManager.getCurrentProfile == null) {
			if (activeScene == Scenes.SPLASH) {
				yield return new WaitUntil(() => Global.baseLoaded);
				SceneManager.LoadScene(Scenes.PROFILES);
			}
			if (activeScene == Scenes.MENU) {
				DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
				yield return new WaitUntil(() => Global.baseLoaded);
				SceneManager.LoadScene(Scenes.PROFILES);
			}
		}
	}


	private void OnDestroy() {
		SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
	}
	#endregion


	private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {

		if (newS.name == Scenes.MENU) {

			if (ProfileManager.getCurrentProfile == null) {
				DebugSceneIndex = 0;
				SceneManager.LoadScene(Scenes.PROFILES);
				return;
			}
			GameObject.Find("Profile").GetComponent<TextMeshProUGUI>().SetText("Welcome: " + ProfileManager.getCurrentProfile.profileName);

		}

		if (newS.name == Scenes.EDITOR) {

		}

		if (newS.name == Scenes.SELECT) {
			LevelSelectScript.displayedSaves.Clear();
		}

		if (newS.name == Scenes.PLAYER) {
			if (levelState != PlaySceneState.PREVIEW) {
				StartCoroutine(GameState());
			}
		}


		if (newS.name == Scenes.SHARING) {

		}

		if (newS.name == Scenes.DEBUG) {

		}


		if (newS.name == Scenes.POSTG) {
			isInGame = false;

			if (isWinner) {
				UI_ReferenceHolder.resultingJudgdement.text = "You Won!";
			}
			else {
				UI_ReferenceHolder.resultingJudgdement.text = "You Lost!";
			}

			if (domination) {
				UI_ReferenceHolder.didDominate.text = "Dominated all cells";
			}
			else {
				UI_ReferenceHolder.didDominate.text = "";
			}
			UI_ReferenceHolder.totalTimeToClear.text = "Fought for:\n" + gameTime;

			UI_ReferenceHolder.totalCoinsAwarded.text = totalCoinsAwarded + "<size=40>coins";
		}

		if (newS.name == Scenes.PROFILES) {
			pM = new ProfileManager(profileVis, GameObject.Find("Content").transform);
			pM.ListProfiles();
		}
		Time.timeScale = 1;
	}


	private void LateUpdate() {
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

		while (SceneManager.GetActiveScene().name == Scenes.PLAYER && isInGame) {
			yield return new WaitForSeconds(1);
			int activeAIs = 0;

			for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
				if (Initialize_AI.AIs[i] != null) {
					if (Initialize_AI.AIs[i].isActive) {
						activeAIs++;
					}
				}
			}
			if (activeAIs == 0) {
				yield return new WaitForSeconds(2);
				if (SceneManager.GetActiveScene().name == Scenes.PLAYER) {
					YouWon();
				}
			}

			int playerCells = 0;

			for (int i = 0; i < cells.Count; i++) {
				if (cells[i].cellTeam == Cell.enmTeam.ALLIED) {
					playerCells++;
				}
			}
			if (playerCells == 0) {
				yield return new WaitForSeconds(2);
				if (SceneManager.GetActiveScene().name == Scenes.PLAYER) {
					GameOver();
				}
			}
		}
	}

	public static void Pause() {
		if (UI_Manager.getWindowCount == 0) {
			isPaused = true;
			Time.timeScale = 0;
			UI_ReferenceHolder.menuPanel.SetActive(true);
		}
		else {
			UI_Manager.CloseMostRecent();
		}
	}

	public static void UnPause() {
		isPaused = false;
		Time.timeScale = 1;
		UI_ReferenceHolder.menuPanel.SetActive(false);
	}

	public void GameOver() {
		SceneManager.LoadScene(Scenes.POSTG);
		isWinner = false;
		gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));
		isInGame = false;
	}

	public void YouWon() {
		totalCoinsAwarded = 1;
		SceneManager.LoadScene(Scenes.POSTG);
		isWinner = true;
		int neutrals = 0;
		foreach (CellBehaviour c in cells) {
			if (c.cellTeam == Cell.enmTeam.NEUTRAL) {
				neutrals++;
			}
		}
		if (neutrals == 0) {
			totalCoinsAwarded += 1;
			domination = true;
		}
		else {
			domination = false;
		}
		gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));

		//Did we play a campaign level ?
		if (CampaignLevel.current != null) {
			ProfileManager.getCurrentProfile.completedCampaignLevels += 1;
			ProfileManager.getCurrentProfile.clearedCampaignLevels[CampaignLevel.current.currentSaveData] = time;
			CampaignLevel.current = null;
		}
		else {
			ProfileManager.getCurrentProfile.completedCustomLevels += 1;
			print("Played custom garbage");
		}

		isInGame = false;
		print(ProfileManager.getCurrentProfile.ownedCoins);
		ProfileManager.getCurrentProfile.ownedCoins += totalCoinsAwarded;
		ProfileManager.SerializeChanges();
	}
}

