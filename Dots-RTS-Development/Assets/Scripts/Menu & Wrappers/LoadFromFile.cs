using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadFromFile : MonoBehaviour {

	public GameObject cellPrefab;

	private PlayManager playManager;
	
	public void Load(string filePath, PlayManager instance) {
		playManager = instance;

		using FileStream file = File.Open(filePath, FileMode.Open);
		BinaryFormatter formatter = new BinaryFormatter();

		if (instance.LevelState == PlaySceneState.Campaign) {
			LoadCampaign(formatter, file);
		}
		else if (instance.LevelState == PlaySceneState.Custom) {
			LoadCustom(formatter, file);
		}
		else {
			LoadPreview(formatter, file);
		}
	}

	private void LoadPreview(IFormatter formatter, Stream file) {
		gameObject.SendMessage("ChangeLayoutToPreview", SendMessageOptions.DontRequireReceiver);
		CommonSetup((SaveData)formatter.Deserialize(file));
	}

	private void LoadCustom(IFormatter formatter, Stream file) {
		CommonSetup((SaveData)formatter.Deserialize(file));
	}

	private void LoadCampaign(IFormatter formatter, Stream file) {
		CommonSetup(((SaveDataCampaign)formatter.Deserialize(file)).Data);
	}

	private void CommonSetup(SaveData data) {
		if (data.GameSize != 0) {
			Camera.main.orthographicSize = data.GameSize;
		}

		GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(data.GameAspect);
		SetupCells(data.Cells);

		playManager.InitializedActors.StartAiInitialization(data.Teams, data.Difficulties, playManager);
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
			if (c.Cell.team == Team.NEUTRAL) {
				playManager.NeutralCells.Add(c);
			}
			else if (c.Cell.team == Team.ALLIED) {
				playManager.Player.MyCells.Add(c);
			}
		}
	}
}
