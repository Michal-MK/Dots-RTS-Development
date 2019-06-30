using UnityEngine;

public class PlayScenePauseHandler : MonoBehaviour, IPauseableScene {

	public void SetPaused(object sender, bool state) {
		LevelPlayerUI UI = GameObject.Find(nameof(LevelPlayerUI)).GetComponent<LevelPlayerUI>();
		if (state) {
			Time.timeScale = 0;
			WindowManagement.Instance.AddWindow(new Window(UI.menuPanel, UI.menuPanel.GetComponent<Animator>(), true, sender));
			Control.isPaused = state;
		}
		else {
			Time.timeScale = 1;
			WindowManagement.Instance.CloseMostRecent();
			if (WindowManagement.Instance.RealWindowCount == 0) {
				Control.isPaused = state;
			}
		}
	}
}
