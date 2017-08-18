using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {
	//Switch scene accroding to its build index
	[System.Obsolete("Use sceneName instead")]
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


	public void SwitchScene(string sceneName) {
		if (SceneManager.GetActiveScene().name == "Level_Editor") {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		if (sceneName == "Level_Editor") {
			PlayerPrefs.SetString("LoadLevelFilePath", null);
		}

		Control.cells.Clear();
		Control.DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(sceneName);
	}
}
