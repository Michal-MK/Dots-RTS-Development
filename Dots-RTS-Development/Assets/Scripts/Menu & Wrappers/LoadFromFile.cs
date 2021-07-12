using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class LoadFromFile : MonoBehaviour {
	public GameObject cellPrefab;

	private PlayManagerBehaviour playManager;

	public PlaySceneConfig Load(string filePath, PlayManagerBehaviour instance) {
		playManager = instance;

		if (instance.LevelState == PlaySceneState.Campaign) {
			return LoadCampaign(filePath);
		}
		if (instance.LevelState == PlaySceneState.Custom) {
			return LoadCustom(filePath);
		}
		return LoadPreview(filePath);
	}

	private PlaySceneConfig LoadPreview(string filePath) {
		gameObject.SendMessage("ChangeLayoutToPreview", SendMessageOptions.DontRequireReceiver);
		return CommonSetup(JsonUtility.FromJson<SaveData>(File.ReadAllText(filePath)));
	}

	private PlaySceneConfig LoadCustom(string filePath) {
		return CommonSetup(JsonUtility.FromJson<SaveData>(File.ReadAllText(filePath)));
	}

	private PlaySceneConfig LoadCampaign(string filePath) {
		return CommonSetup(JsonUtility.FromJson<SaveDataCampaign>(File.ReadAllText(filePath)).Data);
	}

	private PlaySceneConfig CommonSetup(SaveData data) {
		if (data.GameSize != 0) {
			Camera.main.orthographicSize = data.GameSize;
		}

		GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(data.GameAspect);
		PlaySceneConfig ret = SetupCells(data.Cells);

		playManager.InitializedActors.StartAiInitialization(
			data.Teams.ToDictionary(d1 => d1.Team, d2 => d2.ConfigHolder),
			data.Teams.ToDictionary(d1 => d1.Team, d2 => d2.Difficulty),
			playManager);
		
		return ret;
	}

	private PlaySceneConfig SetupCells(List<SerializedCell> cells) {
		Dictionary<Team, int> counts = new Dictionary<Team, int>();
		PlaySceneConfig ret = new PlaySceneConfig();
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
			if (!counts.ContainsKey(cell.Team)) {
				counts[cell.Team] = 1;
			}
			else {
				counts[cell.Team]++;
			}
			c.gameObject.name = $"Cell: {c.Cell.team} ({counts[cell.Team]})";
			ret.Cells.Add(c);
		}

		return ret;
	}
}
