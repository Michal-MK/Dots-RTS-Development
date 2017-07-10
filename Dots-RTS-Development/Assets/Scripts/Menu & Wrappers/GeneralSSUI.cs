using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralSSUI : MonoBehaviour {

	// Loads Main Menu
	public void ToMainMenu () {

		//Shitty implementation of reseting the cursor to nothing when you leave the level editor;
		if (SceneManager.GetActiveScene().name == "LevelEditor") {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}

		SceneManager.LoadScene("Main Menu");
	}

	// Lods Level Select
	public void ToLevelSelect() {
		SceneManager.LoadScene("LevelSelect");
	}
}
