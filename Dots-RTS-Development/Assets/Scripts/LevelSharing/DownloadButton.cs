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
#if !(UNITY_ANDROID || UNITY_IOS)
		DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "\\Saves\\");
#else
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/Saves/");
#endif
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
			//s.t.text = "Download Initiated | ";
			s.DownloadFileFTP(gameObject.name);

			LevelMarket.selectedSave.GetComponent<SaveFileInfo>().indicator.color = Color.green;
			b.interactable = false;
		}
	}
}
