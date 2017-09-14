using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
	//private string savesPath;
	private SaveData saveInfo = null;
	//private bool isRefeshing = false;


	// Use this for initialization
	private async void Start() {

		List<string> contents = await server.GetContentsAsync();

		DirectoryInfo tempFolder = new DirectoryInfo(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves");
		FileInfo[] infos = tempFolder.GetFiles();
		print(infos.Length);
		FileInfo[] persistentInfos = new DirectoryInfo(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar).GetFiles();
		Task<SaveData>[] tasks = new Task<SaveData>[contents.Count];

		for (int j = 0; j < contents.Count; j++) {
			SaveFileInfo s = Instantiate(save, scrollViewContent).GetComponent<SaveFileInfo>();
			s.gameObject.SetActive(false);
			saves.Add(s);

			bool isFilePresentLocaly = false;
			int index = -1;
			for (int i = 0; i < infos.Length; i++) {
				print(infos[i].Name + "  " + contents[j]);
				if (infos[i].Name == contents[j]) {
					print("Exists");
					isFilePresentLocaly = true;
					index = i;
					break;
				}
			}
			if (isFilePresentLocaly) {
				tasks[j] = GetFileLocalyAsync(infos[index].Name, s);
			}
			else {
				print("Getting level info " + contents[j]);
				tasks[j] = server.GetLevelInfoAsync(contents[j]);
			}
		}
		await Task.WhenAll(tasks);
		print("INfos Lenght after " + infos.Length);
		for (int i = 0; i < tasks.Length; i++) {

			saveInfo = tasks[i].Result;

			try {
				saves[i].downloadButton = download;
				saves[i].gameObject.name = contents[i];
				saves[i].levelNameAndAuthorTM.text = saveInfo.levelInfo.levelName;
				saves[i].levelNameAndAuthorTM.text += " by <color=#2A8357FF>" + saveInfo.levelInfo.creator;
				saves[i].timeTM.text = saveInfo.levelInfo.creationTime.ToShortDateString();
			}
			catch (System.Exception e) {
				print("Something Failed " + e);
			}
			saves[i].gameObject.SetActive(true);
			saveInfo = null;
		}

		for (int i = 0; i < saves.Count; i++) {
			for (int j = 0; j < persistentInfos.Length; j++) {
				if (saves[i].gameObject.name == persistentInfos[j].Name) {
					saves[i].isSavedLocaly = true;
					saves[i].indicator.color = Color.green;
					break;
				}
			}
		}
	}


	//Unusable since you can't do it asynchronously
	public async Task<SaveData> GetFileLocalyAsync(string fileName, SaveFileInfo saveInfoScript) {

		string filePath = Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName;
		return await DeserializeObjectAsync<SaveData>(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true));
		//BinaryFormatter bf = new BinaryFormatter();
		//using (FileStream fs = File.Open(filePath, FileMode.Open)) {
		//	SaveData data = (SaveData)bf.Deserialize(fs);
		//	return data;
		//
		//
		//		fs.Close();

		//		saveInfoScript.gameObject.name = fileName;
		//		saveInfoScript.levelName.text = data.levelInfo.levelName;
		//		saveInfoScript.author.text = data.levelInfo.creator;

		//		saves.Add(saveInfoScript);
		//		saveInfo = null;
		//		try {
		//			saveInfoScript.time.text = data.levelInfo.creationTime.ToShortDateString();
		//		}
		//		catch (System.Exception e) {
		//			print(fileName + " " + e);
		//		}
		//		print("Got File Locally");
		//	}
	}

	public void RefreshLevels() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}


	public static Task<T> DeserializeObjectAsync<T>(FileStream stream) {
		return Task.Run(() => {
			BinaryFormatter bf = new BinaryFormatter();
			T type = (T)bf.Deserialize(stream);
			stream.Close();
			stream.Dispose();
			return type;
		});
	}
}
