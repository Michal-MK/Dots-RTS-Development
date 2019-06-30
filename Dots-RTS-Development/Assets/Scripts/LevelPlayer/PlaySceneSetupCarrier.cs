using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneSetupCarrier : MonoBehaviour {

	public void LoadPlayScene(PlaySceneState state, string levelPath) {
		DontDestroyOnLoad(gameObject);

		void OnSceneLoaded() {
			PlayManagerBehaviour manager = GameObject.Find(nameof(PlayManagerBehaviour)).GetComponent<PlayManagerBehaviour>();
			manager.Instance = new PlayManager {
				LevelState = state,
				FilePath = levelPath
			};
			manager.Setup();
			Destroy(gameObject);
		};

		SceneLoader.Instance.Load(Scenes.GAME, OnSceneLoaded);
	}

	public static PlaySceneSetupCarrier Create() {
		GameObject g = new GameObject(nameof(PlaySceneSetupCarrier));
		return g.AddComponent<PlaySceneSetupCarrier>();
	}
}
