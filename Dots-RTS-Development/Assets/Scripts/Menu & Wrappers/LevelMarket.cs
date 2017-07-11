using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;


public class LevelMarket : MonoBehaviour {
	public GameObject save;
	public Transform scrollViewContent;

	public static GameObject selectedSave = null;

	public bool isGettingFileLocaly = false;

	private ServerAccesss server = new ServerAccesss();
	private List<SaveFileInfo> saves = new List<SaveFileInfo>();
	private string savesPath;
	private string[] saveInfo = null;

	// Use this for initialization
	private IEnumerator Start() {
		//RefreshLevels();
		//yield break;
		savesPath = Application.streamingAssetsPath + "\\Saves\\";

		List<string> contents = server.GetContents();

		//DirectoryInfo tempDir = new DirectoryInfo(Application.temporaryCachePath + "\\");
		DirectoryInfo persistentDir = new DirectoryInfo(savesPath);
		//FileInfo[] files = tempDir.GetFiles("*.phage");


		for (int i = 0; i < contents.Count; i++) {
			SaveFileInfo s = Instantiate(save, scrollViewContent).GetComponent<SaveFileInfo>();

			//bool isSavedLocally = false;
			//for (int j = 0; j < files.Length; j++) {
			//	//print(files[j].Name + " " + contents[i]);
			//	if (files[j].Name == contents[i]) {
			//		GetFileLocaly(files[j].Name, s);
			//		isSavedLocally = true;
			//		break;
			//	}
			//}
			//if (!isSavedLocally) {

				StartCoroutine(GetLevelInfo(contents[i]));
				yield return new WaitUntil(() => saveInfo != null);

				server.downloadedFile = "";

				s.gameObject.name = contents[i];

				try {
					s.levelName.text = saveInfo[0];
				}
				catch {
					//print("Name Fail");
				}
				try {
					s.author.text = saveInfo[1];
				}
				catch {
					//print("Author fail");
				}
				try {
					s.creationTime.text = saveInfo[2];
				}
				catch {
					//print("Time Falied");
				}

				saves.Add(s);
				saveInfo = null;
			//}
		}

		FileInfo[] localSaves = persistentDir.GetFiles();
		for (int i = 0; i < saves.Count; i++) {
			for (int j = 0; j < localSaves.Length; j++) {
				if (saves[i].gameObject.name == localSaves[j].Name) {
					saves[i].isSavedLocaly = true;
					saves[i].indicator.color = Color.green;
					break;
				}
			}
		}
	}

	//Unusable since you can't do it asynchronously
	//public void GetFileLocaly(string fileName, SaveFileInfo saveInfoScript) {
	//	string filePath = Application.temporaryCachePath + "\\" + fileName;

	//	isGettingFileLocaly = true;
	//	BinaryFormatter bf = new BinaryFormatter();
	//	using (FileStream fs = File.Open(filePath, FileMode.Open)) {
	//		SaveData data = (SaveData)bf.Deserialize(fs);

	//		fs.Close();

	//		saveInfoScript.gameObject.name = fileName;
	//		saveInfoScript.levelName.text = data.levelInfo.levelName;
	//		saveInfoScript.author.text = data.levelInfo.creator;

	//		saves.Add(saveInfoScript);
	//		saveInfo = null;
	//		try {
	//			saveInfoScript.creationTime.text = data.levelInfo.creationTime.ToShortDateString();
	//		}
	//		catch (System.Exception e) {
	//			print(fileName + e);
	//		}
	//		print("Got File Locally");
	//	}
	//}

	public IEnumerator GetLevelInfo(string path) {
		ServerAccesss s = new ServerAccesss();
		string filePath = s.GetFile(path);
		yield return new WaitUntil(() => !s.isDownloading);
		//print("Assigning save");
		saveInfo = s.GetLevelInfo();
	}

	public void RefreshLevels() {
		int[] ints = new int[5] { 0, 1, 2, 3, 4 };
		int[] moreints = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		for (int i = 0; i < ints.Length; i++) {
			for (int j = 0; j < moreints.Length; j++) {
				if (ints[i] == moreints[j]) {
					print("Mathced");
					break;
				}
			}
		}
	}


}
