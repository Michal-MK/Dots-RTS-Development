using UnityEngine;

public class SingleDiffIF : MonoBehaviour {
	int team;
	public LevelEditorCore core;

	public void OnMove(int newTeam) {
		team = newTeam;
	}

	public void IF_editEnd() {
		core.AiDiffHandler(team);
		gameObject.SetActive(false);
	}
}
