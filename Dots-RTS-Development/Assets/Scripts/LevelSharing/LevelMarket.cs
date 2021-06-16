using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMarket : MonoBehaviour {

	public GameObject save;
	public Transform scrollViewContent;
	public Button download;

	private GameObject selectedSave;
	public GameObject SelectedSave {
		get => selectedSave;
		set {
			selectedSave = value;
			download.interactable = value != null;
		}
	}

	private readonly ServerAccess server = new ServerAccess();
	private readonly List<SaveFileInfo> saves = new List<SaveFileInfo>();
	private SaveData saveInfo;


	private async void Start() {

		List<string> contents = await server.GetLevelsAsync();

		DirectoryInfo tempFolder = new DirectoryInfo(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves");
		FileInfo[] infos = tempFolder.GetFiles();
		FileInfo[] persistentInfos = new DirectoryInfo(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar).GetFiles();
		Task<SaveData>[] tasks = new Task<SaveData>[contents.Count];

		for (int j = 0; j < contents.Count; j++) {
			SaveFileInfo s = Instantiate(save, scrollViewContent).GetComponent<SaveFileInfo>();
			s.Market = this;
			s.gameObject.SetActive(false);
			saves.Add(s);

			bool isFilePresentLocally = false;
			int index = -1;
			for (int i = 0; i < infos.Length; i++) {
				print(infos[i].Name + "  " + contents[j]);
				if (infos[i].Name == contents[j]) {
					print("Exists");
					isFilePresentLocally = true;
					index = i;
					break;
				}
			}
			if (isFilePresentLocally) {
				// TODO
			}
			else {
				tasks[j] = server.DownloadAsync(contents[j]);
			}
		}
		await Task.WhenAll(tasks);
		//print("Infos Length after " + infos.Length);
		for (int i = 0; i < tasks.Length; i++) {

			saveInfo = tasks[i].Result;
			print(saveInfo);
			print(contents[i]);
			print(saveInfo.SaveMeta.LevelName);
			print(saveInfo.SaveMeta.CreatorName);
			print(saveInfo.SaveMeta.CreationTime.ToShortDateString());
			try {
				saves[i].downloadButton = download;
				saves[i].gameObject.name = contents[i];
				saves[i].levelNameAndAuthorTM.text = saveInfo.SaveMeta.LevelName;
				saves[i].levelNameAndAuthorTM.text += " by <color=#2A8357FF>" + saveInfo.SaveMeta.CreatorName;
				saves[i].timeTM.text = saveInfo.SaveMeta.CreationTime.ToShortDateString();
			}
			catch (System.Exception e) {
				print("Something Failed " + e);
			}
			saves[i].gameObject.SetActive(true);
			saveInfo = null;
		}

		foreach (SaveFileInfo sfi in saves) {
			if (persistentInfos.Any(persisted => sfi.gameObject.name == persisted.Name)) {
				sfi.isSavedLocally = true;
				sfi.indicator.color = Color.green;
			}
		}
	}

	public void DownloadLevel() {
		if (SelectedSave == null) return;
		
		download.interactable = false;
		File.Copy(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name,
				  Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + transform.name);
		SelectedSave.GetComponent<SaveFileInfo>().indicator.color = Color.green;
	}

	
	public void RefreshLevels() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
