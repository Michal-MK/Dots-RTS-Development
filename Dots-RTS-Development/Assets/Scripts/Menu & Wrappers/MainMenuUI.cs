using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

	// Turns OFF the game
	public void ExitGame () {
		Application.Quit();
	}
	
	// Launches the level editor in which you can make levels, play them or save them.
	public void LaunchLevelEditor () {
		SceneManager.LoadScene("LevelEditor");
	}

	// Sends you to a screen with premade levels.
	public void LaunchCampaignScreen() {
		SceneManager.LoadScene("CampaignScreen");
	}
}
