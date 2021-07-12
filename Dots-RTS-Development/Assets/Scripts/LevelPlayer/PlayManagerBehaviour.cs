using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManagerBehaviour : MonoBehaviour {

	public PlayScenePauseHandler pauseHandler;

	public int checkFreq;

	private void Awake() {
		Control.pauseHandlers = pauseHandler;
	}
	
	public void EditLevel() {
		SceneLoader.Instance.Load(Scenes.LEVEL_EDITOR, () => { Extensions.Find<LevelEditorCore>().saveAndLoad.Load(FilePath); });
	}

	private void FixedUpdate() {
		Time += UnityEngine.Time.fixedDeltaTime;
	}

	public List<GameCell> AllCells { get; set; } = new List<GameCell>();
	public List<GameCell> NeutralCells { get; set; } = new List<GameCell>();

	public PlaySceneState LevelState { get; set; }
	public float Time { get; set; }
	public int CheckFrequency { get; set; }
	public string FilePath { get; set; }

	public Player Player { get; } = new Player();

	public InitializeActors InitializedActors { get; } = new InitializeActors();

	public GameResult Result { get; set; }

	private bool check;
	private DateTime startTime;

	public IEnumerator Start() {
		LoadFromFile loader = GameObject.Find(nameof(LoadFromFile)).GetComponent<LoadFromFile>();
		PlaySceneConfig config = loader.Load(FilePath, this);

		foreach (GameCell gameCell in config.Cells) {
			foreach (EnemyAI ai in InitializedActors.AIs) {
				gameCell.TeamChanged += ai.CellBehaviour_TeamChanged;
			}
		}
		
		Result = new GameResult();

		startTime = DateTime.Now;
		check = true;
		yield return StartCoroutine(GameState(CheckFrequency));
	}

	private void Stop() {
		check = false;
	}

	private IEnumerator GameState(int checkFreq) {
		while (check) {
			yield return new WaitForSeconds(checkFreq / 1000f);

			int activeAIs = InitializedActors.AIs.Count(ai => ai.isActive);
			int activeAlliedAIs = InitializedActors.AIs.Count(ai => ai.isActive && ai.IsAllyOf(Player));

			if (activeAIs == 0 || activeAIs == activeAlliedAIs) {
				//TODO Delay ?
				Stop();
				if (LevelState == PlaySceneState.Preview) {
					EditLevel();
					yield break;
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
	}
}
