using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public static SceneLoader Instance { get; private set; }
	public event EventHandler OnSceneChanged;

	private Action postLoad;

	public void Init() {
		Instance = this;
		SceneManager.sceneLoaded += SceneLoaded;
	}

	private void SceneLoaded(Scene scene, LoadSceneMode loadMode) {
		postLoad?.Invoke();
		OnSceneChanged?.Invoke(this, EventArgs.Empty);
	}

	public void Load(string sceneName, Action afterLoad, bool resetTimeScale = true) {
		postLoad = afterLoad;
		SceneManager.LoadScene(sceneName);
		Time.timeScale = resetTimeScale ? 1 : Time.timeScale;
	}
}
