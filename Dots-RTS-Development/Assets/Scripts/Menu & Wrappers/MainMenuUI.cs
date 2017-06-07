using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

	// Turns OFF the game
	public void ExitGame() {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}

	// Launches the level editor in which you can make levels, play them or save them.
	public void LaunchLevelEditor() {
		SceneManager.LoadScene("LevelEditor");
	}

	// Sends you to a screen with premade levels.
	public void LaunchCampaignScreen() {
		SceneManager.LoadScene("Game");
	}
}
