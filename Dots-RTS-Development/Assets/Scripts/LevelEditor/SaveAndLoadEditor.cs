using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SaveAndLoadEditor : MonoBehaviour {

	public Text ErrorMessages;
	public LevelEditorCore core;
	public TeamSetup teams;
	public GameObject prefab;
	private string fileName;


	public void TryLevel() {
		string path = Save(true);
		PlayerPrefs.SetString("LoadLevelFilePath", path);
		PlayManager.levelState = PlayManager.PlaySceneState.PREVIEW;
		print(PlayManager.levelState);

		SceneManager.LoadScene(Scenes.PLAYER);
	}

	public void SaveButton() {
		string s = Save();
		print(s);
	}

	public string Save(bool temp = false) {
		fileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
		if (temp) {
			fileName = "testLevel";
		}
		string fullpath;
#if UNITY_ANDROID
			fullpath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".phage";
#else
			fullpath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".phage";
#endif

		#region Pre-Save Error checking

		int numAllies = 0;
		int numEnemies = 0;
		for (int i = 0; i < core.cellList.Count; i++) {
			if (core.cellList[i].cellTeam == Cell.enmTeam.ALLIED) {
				numAllies++;
			}
			if ((int)core.cellList[i].cellTeam >= (int)Cell.enmTeam.ENEMY1) {
				numEnemies++;
			}
		}
		if (numAllies == 0 || numEnemies == 0) {
			ErrorMessages.text = "Your level is missing an enemy, or you didn't create player's cell!";
			if (temp == false) {
				return fullpath;
			}
		}

		ErrorMessages.text += "You picked the fileName: " + fileName + ". \n";

		if (core.levelName == core.defaultLevelName) {
			ErrorMessages.text += "You picked the default levelName: " + core.levelName + ". \n";
		}
		else {
			ErrorMessages.text += "You picked the levelName: " + core.levelName + ". \n";
		}

		if (core.authorName == core.defaultAuthorName) {
			ErrorMessages.text += "You picked the default authorName: " + core.authorName + ". \n";
		}
		else {
			ErrorMessages.text += "You picked the authorName: " + core.authorName + ". \n";
		}
		#endregion

		BinaryFormatter formatter = new BinaryFormatter();
		using (FileStream file = File.Create(fullpath)) {
			SaveData save = new SaveData();

			for (int i = 0; i < core.cellList.Count; i++) {
				EditCell c = core.cellList[i];

				S_Cell serCell = new S_Cell();
				serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
				serCell.elementCount = c.elementCount;
				serCell.maxElementCount = c.maxElements;
				serCell.team = (int)c.cellTeam;
				serCell.regenerationPeriod = c.regenPeriod;
				serCell.installedUpgrades = c.upgrade_manager.upgrades;
				save.cells.Add(serCell);
			}

			save.savedAtAspect = Camera.main.aspect;


			save.difficulty = core.aiDifficultyDict;


			save.gameSize = core.gameSize;
			save.levelInfo = new LevelInfo(core.levelName, core.authorName, DateTime.Now);

			
			save.clans = teams.DictWithAllInfo();

			ErrorMessages.text += "  displayName:(" + save.levelInfo.levelName + ")";
			formatter.Serialize(file, save);
			file.Close();
			return fullpath;
		}
	}

	public void Load(string path) {
		if (File.Exists(path) != true) {
			Debug.LogError("No file found at that path");
			return;
		}

		foreach (EditCell c in core.cellList) {
			Destroy(c.gameObject);
		}
		core.cellList.Clear();
		teams.clanDict.Clear();
		core.aiDifficultyDict.Clear();


		BinaryFormatter formatter = new BinaryFormatter();
		using (FileStream file = File.Open(path, FileMode.Open)) {
			SaveData save = (SaveData)formatter.Deserialize(file);
			core.gameSize = save.gameSize;

			core.aiDifficultyDict = save.difficulty;

			if (save.difficulty != null) {
				core.aiDifficultyDict = save.difficulty;
			}
			else {
				core.aiDifficultyDict = new Dictionary<Cell.enmTeam, float>();
			}
			if (save.clans != null) {
				teams.clanDict = save.clans;
			}
			else {
				teams.clanDict = new Dictionary<Cell.enmTeam, AIHolder>();
			}

			file.Close();

			for (int j = 0; j < save.cells.Count; j++) {

				EditCell c = Instantiate(prefab).GetComponent<EditCell>();

				c.cellPosition = (Vector3)save.cells[j].pos;
				c.gameObject.transform.position = c.cellPosition;
				c.elementCount = save.cells[j].elementCount;
				c.maxElements = save.cells[j].maxElementCount;
				c.cellTeam = (Cell.enmTeam)save.cells[j].team;
				c.regenPeriod = save.cells[j].regenerationPeriod;
				c.upgrade_manager.upgrades = save.cells[j].installedUpgrades;
				core.AddCell(c,true);
			}
		}
		if (path == Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + "testLevel.phage") {
			File.Delete(path);
		}
	}
}