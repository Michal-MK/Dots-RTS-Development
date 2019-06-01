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

	public static List<GameCell> cells = new List<GameCell>();
	public static List<GameCell> neutralCells = new List<GameCell>();

	public float checkFreq = 0.2f;

	private float time;
	private bool isInGame = true;
	private bool isWinner = false;
	private bool domination = false;
	private string gameTime = "";
	private int totalCoinsAwarded = 0;
	private Player playerScript;

	void Start() {
		playerScript = GameObject.Find("Player").GetComponent<Player>();

		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		if (levelState != PlaySceneState.PREVIEW) {
			StartCoroutine(GameState(checkFreq));
		}
		levelState = PlaySceneState.NONE;
	}

	private void OnDestroy() {
		SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

		StopAllCoroutines();
		cells.Clear();
		neutralCells.Clear();
	}

	public IEnumerator GameState(float updateCheckFreq) {

		while (SceneManager.GetActiveScene().name == Scenes.GAME && isInGame) {
			yield return new WaitForSeconds(updateCheckFreq);

			int activeAIs = 0;
			int alliedAIs = 0;

			foreach (Enemy_AI ai in Initialize_AI.AIs) {
				if (ai.isActive) {
					activeAIs++;
					if (ai.IsAllyOf(playerScript)) {
						alliedAIs++;
					}
				}
				
			}

			if (activeAIs == 0 || activeAIs == alliedAIs) {
				yield return new WaitForSeconds(1.5f);
				YouWon();
			}

			int playerCells = 0;

			for (int i = 0; i < cells.Count; i++) {
				if (cells[i].Cell.Team == Team.ALLIED) {
					playerCells++;
				}
			}
			if (playerCells == 0) {
				yield return new WaitForSeconds(1.5f);
				if (SceneManager.GetActiveScene().name == Scenes.GAME) {
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
		SceneManager.LoadScene(Scenes.POST_GAME);
		isWinner = false;
		gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));
		isInGame = false;
		totalCoinsAwarded = 0;
	}

	public void YouWon() {
		DontDestroyOnLoad(gameObject);
		SceneManager.LoadScene(Scenes.POST_GAME);
		totalCoinsAwarded = 1;
		isWinner = true;
		int neutrals = 0;
		foreach (GameCell c in cells) {
			if (c.Cell.Team == Team.NEUTRAL) {
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
			CampaignLevelCode c = ProfileManager.CurrentProfile.CurrentCampaignLevel;
			ProfileManager.CurrentProfile.CompletedCampaignLevels += 1;
			ProfileManager.CurrentProfile.ClearedCampaign[CampaignLevel.current.currentSaveData] = time;
			ProfileManager.CurrentProfile.CurrentCampaignLevel = new CampaignLevelCode(c.Difficulty, c.LevelID + 1);
			CampaignLevel.current = null;
			Destroy(FindObjectOfType<CampaignLevel>().gameObject);
		}
		else {
			ProfileManager.CurrentProfile.CompletedCustomLevels += 1;
			print("Played custom garbage");
		}

		isInGame = false;
		ProfileManager.CurrentProfile.Coins += totalCoinsAwarded;
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
