using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveFileInfo : MonoBehaviour {
	public bool isSavedLocaly = false;
	public Text levelName;
	public Text creationTime;
	public Text author;
	public Image indicator;

	public static event GameControll.NewSelectionForDownload newTarget;

	public ServerAccesss serverAccess = new ServerAccesss();

	public void DeleteObject(Transform fileName) {
		File.Delete(Application.streamingAssetsPath + "\\Saves\\" + fileName.name);
		Destroy(gameObject);
	}

	public void UploadLevel(Transform fileName) {
		serverAccess.UploadFileFTP(fileName.name);
	}

	public void DowloadLevel(Transform filename) {
		serverAccess.DownloadFileFTP(filename.name);
	}

	public void SetAsTarget() {
		LevelMarket.selectedSave = this.gameObject;
		newTarget(this);
		print("OUOUOUOUOUO");
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
