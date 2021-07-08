using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoadEditor : MonoBehaviour {
	public Text errorMessages;
	public LevelEditorCore core;
	public TeamSetup teams;
	public GameObject prefab;

	private const string TEMP_FILE = "templevel";

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
		string fullName = Path.Combine(Application.persistentDataPath, "Saves", fileName + ".phage");

		if (isTemp) {
			fullName += TEMP_FILE;
		}

		return fullName;
	}

	private string Save(bool temp = false) {

		string filePath = BuildPath(temp);

		if (core.cellList.Count(c => c.Cell.team == Team.Allied) == 0 ||
			core.cellList.Count(c => c.Cell.team >= Team.Allied) == 0) {
			errorMessages.text = "Your level is missing an enemy or player cell!";
			return "";
		}

		errorMessages.text += $"File Name: {filePath}.\n";
		errorMessages.text += $"Level Name: {core.LevelName}.\n";
		errorMessages.text += $"Author: {core.AuthorName}.\n";

		using StreamWriter file = File.CreateText(filePath);
		SaveData save = new SaveData();

		foreach (EditCell c in core.cellList) {
			SerializedCell serCell = new SerializedCell {
				Position = new SerializedVector3(c.transform.position),
				Elements = c.Cell.elementCount,
				MaximumElements = c.Cell.maxElements,
				Team = c.Cell.team,
				RegenerationPeriod = c.Cell.regenPeriod,
				InstalledUpgrades = c.UpgradeManager.InstalledUpgrades
			};
			save.Cells.Add(serCell);
		}

		save.GameAspect = Camera.main.aspect;

		save.GameSize = core.GameSize;
		save.SaveMeta = new SaveMeta(core.LevelName, core.AuthorName, DateTime.Now);
		save.Teams = teams.DictWithAllInfo(core.aiDifficultyDict);
		file.Write(JsonUtility.ToJson(save));
		return filePath;
	}

	public void Load(string path) {
		core.ResetScene();

		SaveData save = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

		core.GameSize = save.GameSize;
		core.aiDifficultyDict = save.Teams.ToDictionary(d1 => d1.Team, d2 => d2.Difficulty);

		teams.clanDict = save.Teams.ToDictionary(d1 => d1.Team, d2 => d2.ConfigHolder);

		foreach (SerializedCell savedCell in save.Cells) {
			EditCell c = Instantiate(prefab).GetComponent<EditCell>();

			c.gameObject.transform.position = c.Cell.cellPosition;

			c.Cell.cellPosition = (Vector3)savedCell.Position;
			c.Cell.elementCount = savedCell.Elements;
			c.Cell.maxElements = savedCell.MaximumElements;
			c.Cell.team = savedCell.Team;
			c.Cell.regenPeriod = savedCell.RegenerationPeriod;
			c.UpgradeManager.PreinstallUpgrades(savedCell.InstalledUpgrades);
			core.AddCell(c, true);
			c.UpdateVisual();
		}

		if (path.EndsWith(TEMP_FILE)) {
			File.Delete(path);
		}
	}
}