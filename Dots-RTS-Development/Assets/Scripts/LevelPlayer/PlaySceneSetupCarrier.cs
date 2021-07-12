using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneSetupCarrier : MonoBehaviour {

	public void LoadPlayScene(PlaySceneState state, string levelPath) {
		DontDestroyOnLoad(gameObject);

		SceneLoader.Instance.Load(Scenes.GAME, OnSceneLoaded);

		void OnSceneLoaded() {
			PlayManagerBehaviour manager = GameObject.Find(nameof(PlayManagerBehaviour)).GetComponent<PlayManagerBehaviour>();
			manager.LevelState = state;
			manager.FilePath = levelPath;
			Destroy(gameObject);
		}
	}

	public static PlaySceneSetupCarrier Create() {
		GameObject g = new GameObject(nameof(PlaySceneSetupCarrier));
		return g.AddComponent<PlaySceneSetupCarrier>();
	}
}
