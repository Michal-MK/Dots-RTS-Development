using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectScript : MonoBehaviour {

	private string saveDir;

	public GameObject levelObject;
	public GameObject campaignObject;

	public Transform scrollViewContent;
	public Transform campaignViewContent0;
	public Transform campaignViewContent1;

	public Text error;

	public SaveAndLoadEditor saveAndLoadEditor;

	public static readonly List<SaveFileInfo> displayedSaves = new List<SaveFileInfo>();

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
	
	//Display all saves that you can find in the scroll view
	private void ListCustomSaves() {

		if (GameEnvironment.IsAndroid) {
			saveDir = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves";
		}
		else {
			saveDir = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves";
		}
		DirectoryInfo d = new DirectoryInfo(saveDir);
		error.text = "";
		FileInfo[] saves = d.GetFiles("*.phage");
		BinaryFormatter bf = new BinaryFormatter();
		foreach (FileInfo fileInfo in saves) {
			using FileStream file = fileInfo.Open(FileMode.Open);
			SaveData info = (SaveData)bf.Deserialize(file);
			file.Close();
			SaveFileInfo level = Instantiate(levelObject, scrollViewContent).GetComponent<SaveFileInfo>();
			level.gameObject.SetActive(false);
			level.name = fileInfo.Name;

			level.time.text = $"{fileInfo.CreationTime:dd/MM/yy H:mm:ss}";
			try {
				level.levelName.text = info.SaveMeta.LevelName;
				level.author.text = info.SaveMeta.CreatorName;
				level.timeRaw = info.SaveMeta.CreationTime.ToString();
			}
			catch {
				error.text += "Error " + fileInfo.Name;
			}
			if (SceneManager.GetActiveScene().name == Scenes.LEVEL_EDITOR) {
				level.saveAndLoadEditor = saveAndLoadEditor;
			}
			displayedSaves.Add(level);
			level.gameObject.SetActive(true);
		}
	}

	public void ListCampaignLevels(int diff) {
		saveDir = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + diff;
		DirectoryInfo d = new DirectoryInfo(saveDir);
		int totalLevels = d.GetFiles("*.pwl").Length;

		foreach (CampaignLevel g in campaignViewContent0.GetComponentsInChildren<CampaignLevel>()) {
			Destroy(g.gameObject);
		}
		foreach (CampaignLevel g in campaignViewContent1.GetComponentsInChildren<CampaignLevel>()) {
			Destroy(g.gameObject);
		}

		// int i = onPage * 10;
		// int j;
		// if (totalLevels > 10) {
		// 	j = 10;
		// }
		// else {
		// 	j = totalLevels;
		// }
		// for (int q = i; q < j; q++) {
		// 	if (q <= 4) {
		// 		CampaignLevel c = Instantiate(campaignObject, campaignViewContent0).GetComponent<CampaignLevel>();
		// 		c.gameObject.name = "Level_" + (q + 1);
		// 		c.levelPath = saveDir + Path.DirectorySeparatorChar + c.gameObject.name + ".pwl";
		// 	}
		// 	else {
		// 		CampaignLevel c = Instantiate(campaignObject, campaignViewContent1).GetComponent<CampaignLevel>();
		// 		c.gameObject.name = "Level_" + (q + 1);
		// 		c.levelPath = saveDir + Path.DirectorySeparatorChar + c.gameObject.name + ".pwl";
		// 	}
		// }
	}
}
