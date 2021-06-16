using UnityEngine;

public class LevelEditScenePauseHandler : MonoBehaviour, IPauseableScene {
	public void Pause(object sender) {
		LevelEditorUI uiHolder = GameObject.Find(nameof(LevelEditorUI)).GetComponent<LevelEditorUI>();
		Time.timeScale = 0;
		WindowManagement.Instance.AddWindow(new Window(uiHolder.menuPanel, uiHolder.menuPanel.GetComponent<Animator>(), true, sender));
		Control.isPaused = true;
	}

	public void Unpause(object sender) {
		Time.timeScale = 1;
		WindowManagement.Instance.CloseMostRecent();
		if (WindowManagement.Instance.realWindowCount == 0) {
			Control.isPaused = false;
		}
	}
}
