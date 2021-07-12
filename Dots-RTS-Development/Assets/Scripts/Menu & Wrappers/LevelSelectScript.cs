using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectScript : MonoBehaviour {
	private string saveDir;

	public GameObject levelObject;
	public GameObject campaignObject; // TODO

	public Transform scrollViewContent;
	public Transform campaignViewContent0;
	public Transform campaignViewContent1;

	public Text error;

	public SaveAndLoadEditor saveAndLoadEditor;

	public static readonly List<SaveFileInfo> DISPLAYED_SAVES = new List<SaveFileInfo>();

	private void Start() {
		if (SceneManager.GetActiveScene().name == Scenes.LEVEL_SELECT) {
			ListCustomSaves();
		}
		if (SceneManager.GetActiveScene().name == Scenes.LEVEL_EDITOR) {
			ListCustomSaves();
		}
	}

	private void ListCustomSaves() {
		saveDir = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves";

		DirectoryInfo d = new DirectoryInfo(saveDir);
		error.text = "";
		FileInfo[] saves = d.GetFiles("*" + Paths.SAVE_EXT);

		foreach (FileInfo fileInfo in saves) {
			SaveData info = JsonUtility.FromJson<SaveData>(File.ReadAllText(fileInfo.FullName));
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
			DISPLAYED_SAVES.Add(level);
			level.gameObject.SetActive(true);
		}
	}
}