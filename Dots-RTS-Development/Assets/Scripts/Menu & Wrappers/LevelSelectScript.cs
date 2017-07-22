using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelSelectScript : MonoBehaviour {

	string saveDir;

	public GameObject levelObject;
	public Transform scrollViewContent;
	public Text error;
	public SaveAndLoadEditor saveAndLoadEditor;

	public static List<SaveFileInfo> displayedSaves = new List<SaveFileInfo>();
	private void Start() {

#if !(UNITY_ANDROID || UNITY_IOS)
		saveDir = Application.streamingAssetsPath + "\\Saves";
#else
		saveDir = Application.persistentDataPath + "/Saves";
#endif

		DirectoryInfo d = new DirectoryInfo(saveDir);
		error.text = "";
		FileInfo[] saves = d.GetFiles("*.phage");
		CheckCorruption(saves);
		for (int i = 0; i < saves.Length; i++) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = saves[i].Open(FileMode.Open);
			SaveData info = (SaveData)bf.Deserialize(file);
			file.Close();

			SaveFileInfo level = Instantiate(levelObject, scrollViewContent).GetComponent<SaveFileInfo>();
			level.name = saves[i].Name;

			level.time.text = string.Format("{0:dd/MM/yy H:mm:ss}", saves[i].CreationTime);
			try {
				level.levelName.text = info.levelInfo.levelName;
				level.author.text = info.levelInfo.creator;
				level.timeRaw = info.levelInfo.creationTime.ToString();
			}
			catch {
				error.text += "Error " + saves[i].Name;
			}
			if (SceneManager.GetActiveScene().buildIndex == 1) {
				level.saveAndLoadEditor = saveAndLoadEditor;
			}
			displayedSaves.Add(level);
		}
	}

	private void CheckCorruption(FileInfo[] saves) {
		foreach(FileInfo i in saves) {
			if(i.Length <= 100) {
				i.Delete();
			}
		}
	}
}
