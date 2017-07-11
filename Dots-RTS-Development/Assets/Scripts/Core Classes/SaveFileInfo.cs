using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveFileInfo : MonoBehaviour {
	public bool isSavedLocaly = false;

	public Button downloadButton;
	public Text levelName;
	public Text time;
	public Image indicator;
	public Image bg;

	public static event GameControll.NewSelectionForDownload newTarget;

	public ServerAccesss serverAccess = new ServerAccesss();

	public void DeleteObject(Transform fileName) {
		File.Delete(Application.streamingAssetsPath + "\\Saves\\" + fileName.name);
		Destroy(gameObject);
	}

	public void UploadLevel(Transform fileName) {
		List<string> contents = serverAccess.GetContents();
		for (int i = 0; i < contents.Count; i++) {
			if(contents[i] == fileName.name) {
				print("File with this name already exists. Upload cancelled.");
				return;
			}
		}
		serverAccess.UploadFileFTP(fileName.name);
	}

	public void DowloadLevel(Transform filename) {
		serverAccess.DownloadFileFTP(filename.name);
	}

	public void SetAsTarget() {
		LevelMarket.selectedSave = this.gameObject;
		newTarget(this);
	}

	//TO-DO Replace Fixed Update with someting more efficient
	private void FixedUpdate() {
		if (downloadButton != null) {
			if (downloadButton.name == this.gameObject.name) {
				bg.color = new Color32(100, 100, 255, 150);
			}
			else {
				bg.color = Color.white;
			}
		}
	}

	public void LoadLevel(Transform levelName) {
#if !UNITY_ANDROID
		PlayerPrefs.SetString("LoadLevelFilePath", Application.streamingAssetsPath + "\\Saves\\" + levelName.name);
		SceneManager.LoadScene("LevelPlayer");
#else
		PlayerPrefs.SetString("LoadLevelFilePath", Application.persistentDataPath + "/Saves/" + levelName.name);
		SceneManager.LoadScene("LevelPlayer");
#endif
	}
}
