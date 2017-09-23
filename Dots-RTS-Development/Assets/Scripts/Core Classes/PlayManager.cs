using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayManager : MonoBehaviour {

	public static PlaySceneState levelState;
	public enum PlaySceneState {
		NONE,
		CAMPAIGN,
		CUSTOM,
		PREVIEW
	}

	public static List<CellBehaviour> cells = new List<CellBehaviour>();

	private float time;
	private bool isInGame = true;
	private bool isWinner = false;
	private bool domination = false;
	private string gameTime = "";
	private int totalCoinsAwarded = 0;

	// Use this for initialization
	void Start() {
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		if (levelState != PlaySceneState.PREVIEW) {
			StartCoroutine(GameState());
		}
		levelState = PlaySceneState.NONE;
	}

	private void OnDestroy() {
		SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

		StopAllCoroutines();
		cells.Clear();
	}

	public IEnumerator GameState() {

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
				yield return new WaitForSeconds(1.5f);
				YouWon();
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

	private void FixedUpdate() {
		time += Time.fixedDeltaTime;
	}

	public void GameOver() {
		DontDestroyOnLoad(gameObject);
		SceneManager.LoadScene(Scenes.POSTG);
		isWinner = false;
		gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));
		isInGame = false;
		totalCoinsAwarded = 0;
	}

	public void YouWon() {
		DontDestroyOnLoad(gameObject);
		SceneManager.LoadScene(Scenes.POSTG);
		totalCoinsAwarded = 1;
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
		gameTime = string.Format("{0:00}:{1:00}.{2:00} ", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));
		if((int)time / 60 == 0) {
			gameTime += "seconds";
		}
		else {
			gameTime += "minutes";
		}

		//Did we play a campaign level ?
		if (CampaignLevel.current != null) {
			CampaignLevelCode c = ProfileManager.getCurrentProfile.onLevelBaseGame;
			ProfileManager.getCurrentProfile.completedCampaignLevels += 1;
			ProfileManager.getCurrentProfile.clearedCampaignLevels[CampaignLevel.current.currentSaveData] = time;
			ProfileManager.getCurrentProfile.onLevelBaseGame = new CampaignLevelCode(c.difficulty, c.level + 1);
			CampaignLevel.current = null;
			Destroy(FindObjectOfType<CampaignLevel>().gameObject);
		}
		else {
			ProfileManager.getCurrentProfile.completedCustomLevels += 1;
			print("Played custom garbage");
		}

		isInGame = false;
		ProfileManager.getCurrentProfile.ownedCoins += totalCoinsAwarded;
		ProfileManager.SerializeChanges();
	}

	private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode) {
		SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

		isInGame = false;

		if (isWinner) {
			UI_ReferenceHolder.PG_resultingJudgdement.text = "You Won!";
		}
		else {
			UI_ReferenceHolder.PG_resultingJudgdement.text = "You Lost!";
		}

		if (domination) {
			UI_ReferenceHolder.PG_didDominate.text = "Dominated all cells";
		}
		else {
			UI_ReferenceHolder.PG_didDominate.text = "";
		}

		UI_ReferenceHolder.PG_totalTimeToClear.text = "Fought for:\t" + gameTime;

		UI_ReferenceHolder.PG_totalCoinsAwarded.text = totalCoinsAwarded + " <size=40>coins";

		Destroy(gameObject);
	}
}
