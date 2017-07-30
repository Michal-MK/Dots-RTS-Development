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
	public GameObject campaignObject;

	public Transform scrollViewContent;
	public Transform campaignViewContent;

	public Text error;

	public SaveAndLoadEditor saveAndLoadEditor;

	public static List<SaveFileInfo> displayedSaves = new List<SaveFileInfo>();

	private void Start() {
		if (SceneManager.GetActiveScene().buildIndex == 2) {
			ListCustomSaves();
			ListCampaignLevels();
		}
	}

	//Display alll saves that you can find in the scroll view
	public void ListCustomSaves() {

		saveDir = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves";
		DirectoryInfo d = new DirectoryInfo(saveDir);
		error.text = "";
		FileInfo[] saves = d.GetFiles("*.phage");
		BinaryFormatter bf = new BinaryFormatter();
		CheckCorruption(saves);
		for (int i = 0; i < saves.Length; i++) {
			using (FileStream file = saves[i].Open(FileMode.Open)) {
				SaveData info = (SaveData)bf.Deserialize(file);
				file.Close();
				SaveFileInfo level = Instantiate(levelObject, scrollViewContent).GetComponent<SaveFileInfo>();
				level.gameObject.SetActive(false);
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
				level.gameObject.SetActive(true);

			}
		}
	}

	public void ListCampaignLevels() {
		BinaryFormatter bf = new BinaryFormatter();
		CampaignLevel c;
		saveDir = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign";
		foreach (string dir in Directory.GetDirectories(saveDir)) {
			DirectoryInfo d = new DirectoryInfo(dir);
			for (int i = 0; i < d.GetFiles("*.pwl").Length; i++) {

				GameObject g = Instantiate(campaignObject, campaignViewContent);
				g.name = "Level_" + (i + 1);
			}
		}
	}

	//Looks for files that are smaller than 100 bytes, happens when download fails.
	private void CheckCorruption(FileInfo[] saves) {
		foreach (FileInfo i in saves) {
			if (i.Length <= 100) {
				i.Delete();
			}
		}
	}
}
