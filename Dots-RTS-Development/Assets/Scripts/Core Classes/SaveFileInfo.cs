using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveFileInfo : MonoBehaviour {
	public bool isSavedLocally;

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

	private readonly ServerAccess serverAccess = new ServerAccess();

	public LevelMarket Market { get; set; }


	public void SetAsTarget() {
		Market.SelectedSave = gameObject;
	}

	//TODO Replace Fixed Update with something more efficient
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

	public async void DownloadLevel(Transform filename) {
		await serverAccess.DownloadAsync(filename.name);
	}

	#endregion

	#region Local

	public void LoadLevel(Transform lvlName) {
		string s;
		if (GameEnvironment.IsAndroid) {
			s = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + lvlName.name;
		}
		else {
			s = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + lvlName.name;
		}
		PlaySceneSetupCarrier.Create().LoadPlayScene(PlaySceneState.Custom, s);
	}

	public void DeleteObject(Transform fileName) {
		if (GameEnvironment.IsAndroid) {
			File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + levelName.name);
		}
		else {
			File.Delete(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName.name);
		}

		LevelSelectScript.DISPLAYED_SAVES.Remove(this);
		Destroy(gameObject);
	}

	public async void UploadLevel(Transform fileName) {
		List<string> contents = await serverAccess.GetLevelsAsync();
		for (int i = 0; i < contents.Count; i++) {
			if (contents[i] == fileName.name) {
				print("File with this name already exists. Upload cancelled.");
				return;
			}
		}
		serverAccess.UploadLevel(fileName.name);
	}

	#endregion

	#region Editor Specific

	public void LoadToEditor() {
		string fileName;
		if (GameEnvironment.IsAndroid) {
			fileName = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name;
		}
		else {
			fileName = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name;
		}
		saveAndLoadEditor.Load(fileName);
		WindowManagement.Instance.CloseAll();
	}
	#endregion
}
