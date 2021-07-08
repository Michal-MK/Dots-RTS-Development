using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine;

public class PlayManager {

	public PlayManagerBehaviour Behaviour { get; set; }

	public List<GameCell> AllCells { get; set; } = new List<GameCell>();
	public List<GameCell> NeutralCells { get; set; } = new List<GameCell>();

	public PlaySceneState LevelState { get; set; }
	public float Time { get; set; }
	public int CheckFrequency { get; set; }
	public string FilePath { get; set; }

	public Player Player { get; set; } = new Player();

	public InitializeActors InitializedActors { get; set; } = new InitializeActors();

	public GameResult Result { get; set; }

	private bool check;
	private DateTime startTime;

	public async Task Start() {
		LoadFromFile loader = GameObject.Find(nameof(LoadFromFile)).GetComponent<LoadFromFile>();
		loader.Load(FilePath, this);
		SetupCellSelection();
		Result = new GameResult();

		startTime = DateTime.Now;
		check = true;
		await GameState(CheckFrequency);
	}

	private void Stop() {
		check = false;
	}

	private async Task GameState(int checkFreq) {
		while (check) {
			await Task.Delay(checkFreq);

			int activeAIs = InitializedActors.AIs.Count(ai => ai.isActive);
			int activeAlliedAIs = InitializedActors.AIs.Count(ai => ai.isActive && ai.IsAllyOf(Player));

			if (activeAIs == 0 || activeAIs == activeAlliedAIs) {
				//TODO Delay ?
				Stop();
				if (LevelState == PlaySceneState.Preview) {
					Behaviour.EditLevel();
					return;
				}
				YouWon();
			}

			if (Player.MyCells.Count == 0) {
				//TODO Delay ?
				Stop();
				GameOver();
			}
		}
	}

	private void SetupCellSelection() {
		foreach (GameCell cell in AllCells.Where(cell => cell.Cell.team == Team.Allied)) {
			cell.OnSelectionAttempt += AttemptCellSelection;
		}
	}

	private void AttemptCellSelection(object sender, GameCell e) {
		if (e.Cell.team == Team.Allied) {
			Player.Selection.Add(e);
		}
	}

	private void GameOver() {
		Result.Winner = false;
		Result.GameplayTime = DateTime.Now - startTime;
		SceneLoader.Instance.Load(Scenes.POST_GAME, () => { });
	}

	private void YouWon() {
		Result.Winner = true;

		SceneLoader.Instance.Load(Scenes.POST_GAME, () => { });

		if (NeutralCells.Count == 0) {
			Result.IsDomination = true;
		}

		//if (CampaignLevel.current != null) {
		//	CampaignLevelCode c = ProfileManager.CurrentProfile.CurrentCampaignLevel;
		//	ProfileManager.CurrentProfile.CompletedCampaignLevels += 1;
		//	ProfileManager.CurrentProfile.ClearedCampaign[CampaignLevel.current.currentSaveData] = Time;
		//	ProfileManager.CurrentProfile.CurrentCampaignLevel = new CampaignLevelCode(c.Difficulty, c.LevelID + 1);
		//	CampaignLevel.current = null;
		//	Destroy(FindObjectOfType<CampaignLevel>().gameObject);
		//}
		//else {
		//	ProfileManager.CurrentProfile.CompletedCustomLevels += 1;
		//}

		//isInGame = false;
		//ProfileManager.CurrentProfile.Coins += totalCoinsAwarded;
		//ProfileManager.SerializeChanges();
	}
}
