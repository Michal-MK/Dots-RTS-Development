using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {
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
	public void SwitchScene(string sceneName) {
		if (SceneManager.GetActiveScene().buildIndex == 1) {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		Control.cells.Clear();
		SceneManager.LoadScene(sceneName);
	}

}
