using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;

public class LoadFromFile : MonoBehaviour {
	public GameObject cellPrefab;

	private PlayManager playManager;

	public void Load(string filePath, PlayManager instance) {
		playManager = instance;

		if (instance.LevelState == PlaySceneState.Campaign) {
			LoadCampaign(filePath);
		}
		else if (instance.LevelState == PlaySceneState.Custom) {
			LoadCustom(filePath);
		}
		else {
			LoadPreview(filePath);
		}
	}

	private void LoadPreview(string filePath) {
		gameObject.SendMessage("ChangeLayoutToPreview", SendMessageOptions.DontRequireReceiver);
		CommonSetup(JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(filePath)));
	}

	private void LoadCustom(string filePath) {
		CommonSetup(JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(filePath)));
	}

	private void LoadCampaign(string filePath) {
		CommonSetup(JsonConvert.DeserializeObject<SaveDataCampaign>(File.ReadAllText(filePath)).Data);
	}

	private void CommonSetup(SaveData data) {
		if (data.GameSize != 0) {
			Camera.main.orthographicSize = data.GameSize;
		}

		GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(data.GameAspect);
		SetupCells(data.Cells);

		playManager.InitializedActors.StartAiInitialization(
			data.Teams.ToDictionary(d1 => d1.Team, d2 => d2.ConfigHolder),
			data.Teams.ToDictionary(d1 => d1.Team, d2 => d2.Difficulty),
			playManager);
	}

	private void SetupCells(List<SerializedCell> cells) {
		foreach (SerializedCell cell in cells) {
			GameCell c = Instantiate(cellPrefab).GetComponent<GameCell>();
			c.Cell.cellPosition = (Vector3)cell.Position;
			c.gameObject.transform.position = c.Cell.cellPosition;

			c.Cell.elementCount = cell.Elements;
			c.Cell.maxElements = cell.MaximumElements;
			c.Cell.team = cell.Team;
			c.Cell.regenPeriod = cell.RegenerationPeriod;
			c.uManager.PreinstallUpgrades(cell.InstalledUpgrades);
			c.enabled = true;

			c.UpdateCellInfo();
			playManager.AllCells.Add(c);
			if (c.Cell.team == Team.Neutral) {
				playManager.NeutralCells.Add(c);
			}
			else if (c.Cell.team == Team.Allied) {
				playManager.Player.MyCells.Add(c);
			}
		}
	}
}
