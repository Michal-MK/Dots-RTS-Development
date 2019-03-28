using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class DownloadButton : MonoBehaviour {
	public readonly Button downloadButton;

	public GameObject selected;

	void Start() {
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
				downloadButton.interactable = false;
				return;
			}
		}
		downloadButton.interactable = true;
	}

	public void DownloadLevel() {
		if (LevelMarket.selectedSave != null) {
			downloadButton.interactable = false;
			File.Copy(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name,
						Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name);
			LevelMarket.selectedSave.GetComponent<SaveFileInfo>().indicator.color = Color.green;
		}
	}
}
