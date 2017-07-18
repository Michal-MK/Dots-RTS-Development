using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectScript : MonoBehaviour {

	string saveDir;

	public GameObject levelObject;
	public Transform scrollViewContent;
	public InputField manualFileNameIF;
	public Text error;

	private void Start() {

#if !(UNITY_ANDROID || UNITY_IOS)
		saveDir = Application.streamingAssetsPath + "\\Saves";
#else
		saveDir = Application.persistentDataPath + "/Saves";
#endif

		DirectoryInfo d = new DirectoryInfo(saveDir);
		error.text = "";
		FileInfo[] saves = d.GetFiles("*.phage");
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
			}
			catch {
				error.text += "Error " + saves[i].Name;
			}
		}
	}

#if (UNITY_ANDROID || UNITY_IOS)
	public void loadButtonPress() {
		PlayerPrefs.SetString("LoadLevelFilePath", Application.persistentDataPath + "/Saves/" + manualFileNameIF.text + ".phage");
		SceneManager.LoadScene(3);
	}
#else
	public void loadButtonPress() {
		PlayerPrefs.SetString("LoadLevelFilePath", Application.streamingAssetsPath + "\\Saves\\" + manualFileNameIF.text + ".phage");
		SceneManager.LoadScene(3);
	}

#endif

}
