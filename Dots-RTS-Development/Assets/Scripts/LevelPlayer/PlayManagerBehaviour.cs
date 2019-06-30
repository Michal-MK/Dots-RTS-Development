using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManagerBehaviour : MonoBehaviour {

	public PlayScenePauseHandler pauseHandler;
	public PlayManager Instance { get; set; }

	public int checkFreq;

	private void Awake() {
		Control.pauseHandlers = pauseHandler;
	}

	public void Setup() {
		Instance.CheckFrequency = checkFreq;
		Instance.Behaviour = this;
		_ = Instance.Start();
	}

	public void EditLevel() {
		SceneLoader.Instance.Load(Scenes.LEVEL_EDITOR, () => { Extensions.Find<LevelEditorCore>().saveAndLoad.Load(Instance.FilePath); });
	}

	private void FixedUpdate() {
		if (Instance != null) {
			Instance.Time += Time.fixedDeltaTime;
		}
	}
}

