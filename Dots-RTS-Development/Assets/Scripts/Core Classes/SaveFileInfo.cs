using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class SaveFileInfo : MonoBehaviour {
	public bool isSavedLocaly = false;

	public Button downloadButton;
	public Text levelName;
	public TextMeshProUGUI levelNameAndAuthorTM;

	public Text time;
	public TextMeshProUGUI timeTM;

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

	public async void DowloadLevel(Transform filename) {
		await serverAccess.DownloadFTPAsync(filename.name);
	}

	#endregion

	#region Local

	public void LoadLevel(Transform levelName) {
#if UNITY_ANDROID
		string s = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + levelName.name;
#else
		string s = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + levelName.name;
#endif
		PlayManager.levelState = PlayManager.PlaySceneState.CUSTOM;
		PlayerPrefs.SetString("LoadLevelFilePath", s);
		SceneManager.LoadScene(Scenes.PLAYER);
	}

	public void DeleteObject(Transform fileName) {
#if UNITY_ANDROID
		File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + levelName.name);
#else
		File.Delete(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName.name);
#endif
		LevelSelectScript.displayedSaves.Remove(this);
		Destroy(gameObject);
	}

	//public void UploadLevel(Transform fileName) {
	//	List<string> contents = serverAccess.GetContents();
	//	for (int i = 0; i < contents.Count; i++) {
	//		if (contents[i] == fileName.name) {
	//			print("File with this name already exists. Upload cancelled.");
	//			return;
	//		}
	//	}
	//	serverAccess.UploadFileFTP(fileName.name);
	//}
	public async void UploadLevel(Transform fileName) {
		List<string> contents = await serverAccess.GetContentsAsync();
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
		//print(levelName.name);
#if UNITY_ANDROID
		string fileName = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name;
#else
		string fileName = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name;
#endif
		saveAndLoadEditor.Load(fileName);
		UI_Manager.CloseMostRecent(2);
	}
	#endregion
}
