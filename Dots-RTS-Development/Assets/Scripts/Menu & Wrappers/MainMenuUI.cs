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

		if (SceneManager.GetActiveScene().name == "LevelEditor") {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}

		SceneManager.LoadScene(sceneIndex);

	}
}