using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectScript : MonoBehaviour {

	string saveDir;

	public GameObject levelObject;
	public GameObject campaignObject;

	public Transform scrollViewContent;
	public Transform campaignViewContent0;
	public Transform campaignViewContent1;

	public Text error;

	public SaveAndLoadEditor saveAndLoadEditor;

	public static List<SaveFileInfo> displayedSaves = new List<SaveFileInfo>();

	public int onPage = 0;
	public int totalPages = 1;


	private void Start() {
		if (SceneManager.GetActiveScene().name == Scenes.LEVEL_SELECT) {
			ListCustomSaves();
			ListCampaignLevels(1);
		}
		if (SceneManager.GetActiveScene().name == Scenes.LEVEL_EDITOR) {
			ListCustomSaves();
		}
	}
	//Display alll saves that you can find in the scroll view
	public void ListCustomSaves() {
#if UNITY_ANDROID
		saveDir = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves";
#else
        saveDir = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves";
#endif

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
				if (SceneManager.GetActiveScene().name == Scenes.LEVEL_EDITOR) {
					level.saveAndLoadEditor = saveAndLoadEditor;
				}
				displayedSaves.Add(level);
				level.gameObject.SetActive(true);

			}
		}
	}

	public void ListCampaignLevels(int diff) {
		saveDir = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + diff;
		DirectoryInfo d = new DirectoryInfo(saveDir);
		int totalLevels = d.GetFiles("*.pwl").Length;

		//int totalPages = totalLevels / 10;


		foreach (CampaignLevel g in campaignViewContent0.GetComponentsInChildren<CampaignLevel>()) {
			Destroy(g.gameObject);
		}
		foreach (CampaignLevel g in campaignViewContent1.GetComponentsInChildren<CampaignLevel>()) {
			Destroy(g.gameObject);
		}

		int i = onPage * 10;
		int j;
		if (totalLevels > 10) {
			j = 10;
		}
		else {
			j = totalLevels;
		}

		for (int q = i; q < j; q++) {
			if (q <= 4) {
				CampaignLevel c = Instantiate(campaignObject, campaignViewContent0).GetComponent<CampaignLevel>();
				c.gameObject.name = "Level_" + (q + 1);
				c.levelPath = saveDir + Path.DirectorySeparatorChar + c.gameObject.name + ".pwl";
			}
			else {
				CampaignLevel c = Instantiate(campaignObject, campaignViewContent1).GetComponent<CampaignLevel>();
				c.gameObject.name = "Level_" + (q + 1);
				c.levelPath = saveDir + Path.DirectorySeparatorChar + c.gameObject.name + ".pwl";
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
