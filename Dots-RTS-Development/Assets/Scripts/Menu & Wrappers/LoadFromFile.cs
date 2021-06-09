using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadFromFile : MonoBehaviour {

	public GameObject cellPrefab;

	[HideInInspector]
	public string FilePath { get; set; }

	private PlayManager playManager;



	public void Load(string filePath, PlayManager instance) {
		playManager = instance;
		FilePath = filePath;

		using (FileStream file = File.Open(filePath, FileMode.Open)) {
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
	}

	private void LoadPreview(BinaryFormatter formatter, FileStream file) {
		gameObject.SendMessage("ChangeLayoutToPreview", SendMessageOptions.DontRequireReceiver);
		CommonSetup((SaveData)formatter.Deserialize(file));
	}

	private void LoadCustom(BinaryFormatter formatter, FileStream file) {
		CommonSetup((SaveData)formatter.Deserialize(file));
	}

	private void LoadCampaign(BinaryFormatter formatter, FileStream file) {
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
			c.Cell.CellPosition = (Vector3)cell.Position;
			c.gameObject.transform.position = c.Cell.CellPosition;

			c.Cell.ElementCount = cell.Elements;
			c.Cell.MaxElements = cell.MaximumElements;
			c.Cell.Team = cell.Team;
			c.Cell.RegenPeriod = cell.RegenerationPeriod;
			c.uManager.PreinstallUpgrades(cell.InstalledUpgrades);
			c.enabled = true;

			c.UpdateCellInfo();
			playManager.AllCells.Add(c);
			if (c.Cell.Team == Team.NEUTRAL) {
				playManager.NeutralCells.Add(c);
			}
			else if (c.Cell.Team == Team.ALLIED) {
				playManager.Player.MyCells.Add(c);
			}
		}
	}
}