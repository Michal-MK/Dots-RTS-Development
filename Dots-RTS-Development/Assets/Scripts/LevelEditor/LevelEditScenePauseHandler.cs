using UnityEngine;


public class LevelEditScenePauseHandler : MonoBehaviour, IPauseableScene {

	public void SetPaused(object sender, bool state) {
		LevelEditorUI UI = GameObject.Find(nameof(LevelEditorUI)).GetComponent<LevelEditorUI>();
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

