using UnityEngine;

public class PlayScenePauseHandler : MonoBehaviour, IPauseableScene {
	public void Unpause(object sender) {
		LevelPlayerUI uiHolder = GameObject.Find(nameof(LevelPlayerUI)).GetComponent<LevelPlayerUI>();
		Time.timeScale = 0;
		WindowManagement.Instance.AddWindow(new Window(uiHolder.menuPanel, uiHolder.menuPanel.GetComponent<Animator>(), true, sender));
		Control.isPaused = false;
	}

	public void Pause(object sender) {
		Time.timeScale = 1;
		WindowManagement.Instance.CloseMostRecent();
		if (WindowManagement.Instance.realWindowCount == 0) {
			Control.isPaused = true;
		}
	}
}
