using UnityEngine;

public class LevelEditorLauncher : MonoBehaviour {

	public LevelEditScenePauseHandler pauseHandler;

	private void Awake() {
		Control.pauseHandlers = pauseHandler;
	}
}
