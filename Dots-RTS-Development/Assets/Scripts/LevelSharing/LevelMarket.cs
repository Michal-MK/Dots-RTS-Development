using System;
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

		FileInfo[] persistentInfos = new DirectoryInfo(Paths.SAVES).GetFiles();
		Task<SaveData>[] tasks = new Task<SaveData>[contents.Count];

		for (int j = 0; j < contents.Count; j++) {
			SaveFileInfo s = Instantiate(save, scrollViewContent).GetComponent<SaveFileInfo>();
			s.Market = this;
			s.gameObject.SetActive(false);
			saves.Add(s);
			tasks[j] = server.DownloadAsync(contents[j]);
		}
		await Task.WhenAll(tasks);

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
			catch (Exception e) {
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

	public async void DownloadLevel() {
		if (SelectedSave == null) return;

		download.interactable = false;
		await server.DownloadAsync(SelectedSave.name);
		SelectedSave.GetComponent<SaveFileInfo>().indicator.color = Color.green;
	}

	public void RefreshLevels() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}