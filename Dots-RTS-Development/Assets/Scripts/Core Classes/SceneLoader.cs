using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public static SceneLoader Instance { get; private set; }
	public event EventHandler OnSceneChanged;

	private Action _postLoadExec;

	public void Init() {
		Instance = this;
		SceneManager.sceneLoaded += SceneLoaded;
	}

	private void SceneLoaded(Scene scene, LoadSceneMode loadMode) {
		_postLoadExec?.Invoke();
		OnSceneChanged?.Invoke(this, EventArgs.Empty);
	}

	public void Load(string sceneName, Action postLoad, bool resetTimeScale = true) {
		_postLoadExec = postLoad;
		SceneManager.LoadScene(sceneName);
		Time.timeScale = resetTimeScale ? 1 : Time.timeScale;
	}
}
