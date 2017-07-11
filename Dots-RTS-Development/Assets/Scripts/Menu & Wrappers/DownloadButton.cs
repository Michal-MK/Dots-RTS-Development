using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadButton : MonoBehaviour {

	public GameObject selected;
	// Use this for initialization
	void Start() {
		SaveFileInfo.newTarget += SaveFileInfo_newTarget;
	}
	private void OnDestroy() {
		SaveFileInfo.newTarget -= SaveFileInfo_newTarget;

	}

	private void SaveFileInfo_newTarget(SaveFileInfo sender) {
		gameObject.name = sender.gameObject.name;
	}

	public void DownloadLevel() {
		if (LevelMarket.selectedSave != null) {
			ServerAccesss s = new ServerAccesss();
			s.DownloadFileFTP(gameObject.name);
		}
	}

	private void Update() {
		selected = LevelMarket.selectedSave;
	}
}
