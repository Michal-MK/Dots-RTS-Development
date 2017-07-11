using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class DownloadButton : MonoBehaviour {
	public Button b;
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
		selected = sender.gameObject;

		DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "\\Saves\\");
		FileInfo[] files = dir.GetFiles();

		for (int i = 0; i < files.Length; i++) {
			if(files[i].Name == gameObject.name) {
				print("Got this one");
				b.interactable = false;
				return;
			}
		}
		b.interactable = true;
	}

	public void DownloadLevel() {
		if (LevelMarket.selectedSave != null) {
			ServerAccesss s = new ServerAccesss();
			s.DownloadFileFTP(gameObject.name);
			LevelMarket.selectedSave.GetComponent<SaveFileInfo>().indicator.color = Color.green;
			b.interactable = false;
		}
	}
}
