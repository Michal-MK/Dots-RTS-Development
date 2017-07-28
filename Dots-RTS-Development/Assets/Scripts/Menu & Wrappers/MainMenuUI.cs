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

	//Switch scene accroding to its build index
	public void SwitchScene(int sceneIndex) {

		if (SceneManager.GetActiveScene().buildIndex == 1) {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		if (sceneIndex == 1) {
			PlayerPrefs.SetString("LoadLevelFilePath", null);
		}
		Control.cells.Clear();
		SceneManager.LoadScene(sceneIndex);

	}
	public void SwitchScene(string Name) {
		if (SceneManager.GetActiveScene().buildIndex == 1) {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		Control.cells.Clear();
		SceneManager.LoadScene(Name);
	}
}