using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelMarket : MonoBehaviour {
	public GameObject save;
	public Transform scrollViewContent;
	public Button download;
	public static GameObject selectedSave = null;

	private ServerAccess server = new ServerAccess();
	private List<SaveFileInfo> saves = new List<SaveFileInfo>();
	private string savesPath;
	private string[] saveInfo = null;
	private bool isRefeshing = false;


	// Use this for initialization
	private IEnumerator Start() {
		isRefeshing = false;
		List<string> contents = server.GetContents();

		DirectoryInfo persistentDir = new DirectoryInfo(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves");

		for (int i = 0; i < contents.Count; i++) {
			if (isRefeshing) {
				isRefeshing = false;
				yield break;
			}

			SaveFileInfo s = Instantiate(save, scrollViewContent).GetComponent<SaveFileInfo>();
			s.gameObject.SetActive(false);

			//Unusable since it can't be done asynchronously
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
			//Uncomented Code Below
			// |
			// V
			//}

			//Old method of obtaining file information from server;
			//StartCoroutine(GetLevelInfo(contents[i]));
			//yield return new WaitUntil(() => saveInfo != null);

			//server.t.text += "Getting level info | ";
			IEnumerator e = server.GetLevelInfo(contents[i]);
			yield return e;
			saveInfo = e.Current as string[];

			//server.t.text += "Retrieved! | ";

			try {
				s.downloadButton = download;
				s.gameObject.name = contents[i];
				s.levelName.text = saveInfo[0];
				s.levelName.text += " by " + saveInfo[1];
				s.time.text = saveInfo[2];
			}
			catch {
				print("Something Failed");
			}

			saves.Add(s);
			s.gameObject.SetActive(true);
			saveInfo = null;
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

	public void RefreshLevels() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
