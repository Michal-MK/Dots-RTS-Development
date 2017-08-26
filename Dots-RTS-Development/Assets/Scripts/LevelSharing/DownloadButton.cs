using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class DownloadButton : MonoBehaviour {
	public Button b;
	public GameObject selected;


	public Text debug;
	public ServerAccess s = new ServerAccess();

	// Use this for initialization
	void Start() {
		//s.t = debug;
		SaveFileInfo.newTarget += SaveFileInfo_newTarget;
	}
	private void OnDestroy() {
		SaveFileInfo.newTarget -= SaveFileInfo_newTarget;

	}

	private void SaveFileInfo_newTarget(SaveFileInfo sender) {
		gameObject.name = sender.gameObject.name;
		selected = sender.gameObject;

		DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + Path.DirectorySeparatorChar +  "Saves");

		FileInfo[] files = dir.GetFiles();

		for (int i = 0; i < files.Length; i++) {
			if(files[i].Name == gameObject.name) {
				b.interactable = false;
				return;
			}
		}
		b.interactable = true;
	}

	public void DownloadLevel() {
		if (LevelMarket.selectedSave != null) {
			b.interactable = false;
			File.Copy(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name,
						Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name);
			LevelMarket.selectedSave.GetComponent<SaveFileInfo>().indicator.color = Color.green;
		}
	}
}
