using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

	public void SwitchScene(string sceneName) {
		if (SceneManager.GetActiveScene().name == Scenes.LEVEL_EDITOR) {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		if (sceneName == Scenes.LEVEL_EDITOR) {
			PlayerPrefs.SetString("LoadLevelFilePath", null);
		}
		Control.DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(sceneName);
	}
}
