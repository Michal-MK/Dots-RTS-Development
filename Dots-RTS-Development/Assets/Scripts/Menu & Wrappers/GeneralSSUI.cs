using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralSSUI : MonoBehaviour {

	// Loads Main Menu
	public void ToMainMenu () {
		SceneManager.LoadScene("Main Menu");
	}

	// Lods Level Select
	public void ToLevelSelect() {
		SceneManager.LoadScene("LevelSelect");
	}
}
