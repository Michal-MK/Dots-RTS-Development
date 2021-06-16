using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class SaveAndLoadEditor : MonoBehaviour {

	public Text errorMessages;
	public LevelEditorCore core;
	public TeamSetup teams;
	public GameObject prefab;


	public void TryLevel() {
		string path = Save(true);
		Extensions.Find<LevelEditScenePauseHandler>().Unpause(this);
		PlaySceneSetupCarrier.Create().LoadPlayScene(PlaySceneState.Preview, path);
		Time.timeScale = 1;
	}

	public void SaveButton() {
		Save();
	}

	private string BuildPath(bool isTemp) {
		DateTime dt = DateTime.Now;
		string fileName = $"{dt.Year}-{dt.Month}-{dt.Day}-{dt.Hour}-{dt.Minute}-{dt.Second}";

		if (isTemp) {
			fileName += "testLevel";
		}

		string fullpath;
		if (GameEnvironment.IsAndroid) {
			fullpath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".phage";
		}
		else {
			fullpath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".phage";
		}
		return fullpath;
	}

	public string Save(bool temp = false) {

		string filePath = BuildPath(temp);

		//Check amount of player and enemy cells
		if (core.cellList.Count(c => c.Cell.team == Team.ALLIED) == 0 || core.cellList.Count(c => c.Cell.team > Team.ALLIED) == 0) { 
			errorMessages.text = "Your level is missing an enemy or player cell!";
			return "";
		}

		errorMessages.text += $"File Name: {filePath}.\n"; ;
		errorMessages.text += $"Level Name: {core.LevelName}.\n";
		errorMessages.text += "Author: {core.AuthorName}.\n";

		BinaryFormatter formatter = new BinaryFormatter();

		using (FileStream file = File.Create(filePath)) {
			SaveData save = new SaveData();

			for (int i = 0; i < core.cellList.Count; i++) {
				EditCell c = core.cellList[i];

				SerializedCell serCell = new SerializedCell {
					Position = new SerializedVector3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z },
					Elements = c.Cell.elementCount,
					MaximumElements = c.Cell.maxElements,
					Team = c.Cell.team,
					RegenerationPeriod = c.Cell.regenPeriod,
					InstalledUpgrades = c.upgrade_manager.upgrades
				};
				save.Cells.Add(serCell);
			}

			save.GameAspect = Camera.main.aspect;
			save.Difficulties = core.aiDifficultyDict;

			save.GameSize = core.GameSize;
			save.SaveMeta = new SaveMeta(core.LevelName, core.AuthorName, DateTime.Now);
			save.Teams = teams.DictWithAllInfo();
			formatter.Serialize(file, save);
			return filePath;
		}
	}

	public void Load(string path) {
		core.ResetScene();

		BinaryFormatter formatter = new BinaryFormatter();

		using (FileStream file = File.Open(path, FileMode.Open)) {
			SaveData save = (SaveData)formatter.Deserialize(file);

			core.GameSize = save.GameSize;
			core.aiDifficultyDict = save.Difficulties;

			core.aiDifficultyDict = save.Difficulties;
			teams.clanDict = save.Teams;


			for (int j = 0; j < save.Cells.Count; j++) {

				EditCell c = Instantiate(prefab).GetComponent<EditCell>();

				c.Cell.cellPosition = (Vector3)save.Cells[j].Position;
				c.gameObject.transform.position = c.Cell.cellPosition;

				c.Cell.elementCount = save.Cells[j].Elements;
				c.Cell.maxElements = save.Cells[j].MaximumElements;
				c.Cell.team = save.Cells[j].Team;
				c.Cell.regenPeriod = save.Cells[j].RegenerationPeriod;
				c.upgrade_manager.upgrades = save.Cells[j].InstalledUpgrades;
				core.AddCell(c, true);
				c.UpdateVisual();
			}
		}

		if (path.Contains(Path.GetTempPath())) {
			File.Delete(path);
		}
	}
}
