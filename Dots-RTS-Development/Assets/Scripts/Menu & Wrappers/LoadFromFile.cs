using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadFromFile : MonoBehaviour { //TODO Decipher the code

	public GameObject cellPrefab;
	public Initialize_AI init;

	private void Start() {
		if (File.Exists(PlayerPrefs.GetString("LoadLevelFilePath"))) {
			gameObject.SendMessage("FoundAFile", SendMessageOptions.DontRequireReceiver);
			Debug.LogWarning("FoundAFile does not have a receiver.");
		}
		else {
			gameObject.SendMessage("NoFileFound");
			return;
		}

		using (FileStream file = File.Open(PlayerPrefs.GetString("LoadLevelFilePath"), FileMode.Open)) {
			BinaryFormatter formatter = new BinaryFormatter();

			SaveData customSave;
			SaveDataCampaign campaignSave;

			if (PlayManager.levelState == PlayManager.PlaySceneState.CUSTOM) {
				customSave = (SaveData)formatter.Deserialize(file);
				if (customSave.gameSize != 0) {
					Camera.main.orthographicSize = customSave.gameSize;
				}
				GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(customSave.savedAtAspect);
				

				for (int j = 0; j < customSave.cells.Count; j++) {

					GameCell c = cellPrefab.GetComponent<GameCell>();

					c.Cell.CellPosition = (Vector3)customSave.cells[j].pos;
					c.gameObject.transform.position = c.Cell.CellPosition;
					c.Cell.ElementCount = customSave.cells[j].elementCount;
					c.Cell.MaxElements = customSave.cells[j].maxElementCount;
					c.Cell.CellTeam = (Team)customSave.cells[j].team;
					c.Cell.RegenPeriod = customSave.cells[j].regenerationPeriod;

					GameCell cg = Instantiate(cellPrefab).GetComponent<GameCell>();

					cg.uManager.PreinstallUpgrades(customSave.cells[j].installedUpgrades);
					cg.gameObject.name = "Cell " + j + " " + c.Cell.CellTeam;
					cg.enabled = true;
				}
				init.StartAiInitialization(customSave.clans, customSave.difficulty);
			}

			else if (PlayManager.levelState == PlayManager.PlaySceneState.CAMPAIGN) {
				campaignSave = (SaveDataCampaign)formatter.Deserialize(file);
				if (campaignSave.game.gameSize != 0) {
					Camera.main.orthographicSize = campaignSave.game.gameSize;
				}
				GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(campaignSave.game.savedAtAspect);

				for (int j = 0; j < campaignSave.game.cells.Count; j++) {

					GameCell c = Instantiate(cellPrefab).GetComponent<GameCell>();

					c.Cell.CellPosition= (Vector3)campaignSave.game.cells[j].pos;
					c.gameObject.transform.position = c.Cell.CellPosition;
					c.Cell.ElementCount = campaignSave.game.cells[j].elementCount;
					c.Cell.MaxElements = campaignSave.game.cells[j].maxElementCount;
					c.Cell.CellTeam = (Team)campaignSave.game.cells[j].team;
					c.Cell.RegenPeriod = campaignSave.game.cells[j].regenerationPeriod;
					//c.um.upgrades = save.cells[j].installedUpgrades.upgrade;

					c.enabled = true;

					c.UpdateCellInfo();
				}
				init.StartAiInitialization(campaignSave.game.clans, campaignSave.game.difficulty);

			}
			else if (PlayManager.levelState == PlayManager.PlaySceneState.PREVIEW) {
				gameObject.SendMessage("ChangeLayoutToPreview", SendMessageOptions.DontRequireReceiver);
				customSave = (SaveData)formatter.Deserialize(file);
				if (customSave.gameSize != 0) {
					Camera.main.orthographicSize = customSave.gameSize;
				}
				GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(customSave.savedAtAspect);

				for (int j = 0; j < customSave.cells.Count; j++) {

					GameCell c = Instantiate(cellPrefab).GetComponent<GameCell>();

					c.Cell.CellPosition = (Vector3)customSave.cells[j].pos;
					c.gameObject.transform.position = c.Cell.CellPosition;
					c.Cell.ElementCount = customSave.cells[j].elementCount;
					c.Cell.MaxElements = customSave.cells[j].maxElementCount;
					c.Cell.CellTeam = (Team)customSave.cells[j].team;
					c.Cell.RegenPeriod = customSave.cells[j].regenerationPeriod;
					//c.um.upgrades = save.cells[j].installedUpgrades.upgrade;

					c.enabled = true;

					c.UpdateCellInfo();
				}
				init.StartAiInitialization(customSave.clans, customSave.difficulty);
			}
			else {
				//SceneManager.LoadScene(Scenes.PROFILES);
				Debug.Break();
			}
		}
	}
}