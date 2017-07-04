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

	private void Start() {

		saveDir = Application.streamingAssetsPath + "\\Saves";


		DirectoryInfo d = new DirectoryInfo(saveDir);
		FileInfo[] saves = d.GetFiles("*.phage");
		for (int i = 0; i < saves.Length; i++) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = saves[i].Open(FileMode.Open);
			SaveData info = (SaveData)bf.Deserialize(file);
			file.Close();

			SaveFileInfo level = Instantiate(levelObject, scrollViewContent).GetComponent<SaveFileInfo>();
			level.name = saves[i].Name;

			level.creationTime.text = string.Format("{0:dd/MM/yy H:mm:ss}", saves[i].CreationTime);
			level.levelName.text = info.levelInfo.levelName;

		}
	}

#if UNITY_ANDROID
	public void loadButtonPress() {
		PlayerPrefs.SetString("LoadLevelFilePath", Application.persistentDataPath + "\\Saves\\" + manualFileNameIF.text + ".phage");
		SceneManager.LoadScene("LevelPlayer");
	}
#else
	public void loadButtonPress() {
		PlayerPrefs.SetString("LoadLevelFilePath", Application.streamingAssetsPath + "\\Saves\\" + manualFileNameIF.text + ".phage");
		SceneManager.LoadScene("LevelPlayer");
	}

#endif

}
