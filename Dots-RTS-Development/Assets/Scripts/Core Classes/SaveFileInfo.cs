using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SaveFileInfo : MonoBehaviour {
	public bool isSavedLocaly = false;

	public Button downloadButton;
	public Text levelName;
	public Text time;
	public Text author;
	public Image indicator;
	public Image bg;
	public SaveAndLoadEditor saveAndLoadEditor;
	public string timeRaw;

	public static event Control.NewSelectionForDownload newTarget;

	public ServerAccess serverAccess = new ServerAccess();


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

	#region Server Specific

	public void DowloadLevel(Transform filename) {
		serverAccess.DownloadFileFTP(filename.name);
	}


	#endregion

	#region Local

	public void LoadLevel(Transform levelName) {
#if !(UNITY_ANDROID || UNITY_IOS)
		PlayerPrefs.SetString("LoadLevelFilePath", Application.streamingAssetsPath + "\\Saves\\" + levelName.name);
		SceneManager.LoadScene(3);
#else
		PlayerPrefs.SetString("LoadLevelFilePath", Application.persistentDataPath + "/Saves/" + levelName.name);
		SceneManager.LoadScene(3);
#endif
	}

	public void DeleteObject(Transform fileName) {
#if !(UNITY_ANDROID || UNITY_IOS)
		File.Delete(Application.streamingAssetsPath + "\\Saves\\" + fileName.name);
#else
		File.Delete(Application.persistentDataPath + "/Saves/" + fileName.name);
#endif
		LevelSelectScript.displayedSaves.Remove(this);
		Destroy(gameObject);

	}

	public void UploadLevel(Transform fileName) {
		List<string> contents = serverAccess.GetContents();
		for (int i = 0; i < contents.Count; i++) {
			if (contents[i] == fileName.name) {
				print("File with this name already exists. Upload cancelled.");
				return;
			}
		}
		serverAccess.UploadFileFTP(fileName.name);
	}

	#endregion

	#region Editor Specific

	public void LoadToEditor() {
		string fileName;
#if (UNITY_ANDROID || UNITY_IOS)
		fileName = Application.persistentDataPath + "/Saves/ " + transform.name;
#else
		fileName = Application.streamingAssetsPath + "\\Saves\\" + transform.name;
#endif
		saveAndLoadEditor.Load(fileName);
	}

	#endregion
}
