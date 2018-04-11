using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadFromFile : MonoBehaviour {
	public static float refAspect;
	public GameObject cellPrefab;
	//public GameObject gameControlllPrefab;
	public Initialize_AI init;
	// Use this for initialization
	void Start() {


		if (File.Exists(PlayerPrefs.GetString("LoadLevelFilePath"))) {
			gameObject.SendMessage("FoundAFile", SendMessageOptions.DontRequireReceiver);
			Debug.LogWarning("FoundAFile does not have a receiver.");
		}
		else {
			gameObject.SendMessage("NoFileFound");
			return;
		}


		BinaryFormatter formatter = new BinaryFormatter();

		FileStream file = File.Open(PlayerPrefs.GetString("LoadLevelFilePath"), FileMode.Open);

		SaveData customSave;
		SaveDataCampaign campaignSave;

		if (PlayManager.levelState == PlayManager.PlaySceneState.CUSTOM) {
			customSave = (SaveData)formatter.Deserialize(file);
			if (customSave.gameSize != 0) {
				Camera.main.orthographicSize = customSave.gameSize;
			}
			GameObject.Find("Borders").GetComponent<PlayFieldSetup>().ResizeBackground(customSave.savedAtAspect);;

			for (int j = 0; j < customSave.cells.Count; j++) {

				CellBehaviour c = cellPrefab.GetComponent<CellBehaviour>();

				c.cellPosition = (Vector3)customSave.cells[j].pos;
				c.gameObject.transform.position = c.cellPosition;
				c.elementCount = customSave.cells[j].elementCount;
				c.maxElements = customSave.cells[j].maxElementCount;
				c.cellTeam = (Cell.enmTeam)customSave.cells[j].team;
				c.regenPeriod = customSave.cells[j].regenerationPeriod;

				CellBehaviour cg = Instantiate(cellPrefab).GetComponent<CellBehaviour>();

				cg.uManager.PreinstallUpgrades = customSave.cells[j].installedUpgrades;
				cg.gameObject.name = "Cell " + j + " " + c.cellTeam;
				cg.enabled = true;

				//c.UpdateCellInfo();
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

				CellBehaviour c = Instantiate(cellPrefab).GetComponent<CellBehaviour>();

				c.cellPosition = (Vector3)campaignSave.game.cells[j].pos;
				c.gameObject.transform.position = c.cellPosition;
				c.elementCount = campaignSave.game.cells[j].elementCount;
				c.maxElements = campaignSave.game.cells[j].maxElementCount;
				c.cellTeam = (Cell.enmTeam)campaignSave.game.cells[j].team;
				c.regenPeriod = campaignSave.game.cells[j].regenerationPeriod;
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

                CellBehaviour c = Instantiate(cellPrefab).GetComponent<CellBehaviour>();

                c.cellPosition = (Vector3)customSave.cells[j].pos;
                c.gameObject.transform.position = c.cellPosition;
                c.elementCount = customSave.cells[j].elementCount;
                c.maxElements = customSave.cells[j].maxElementCount;
                c.cellTeam = (Cell.enmTeam)customSave.cells[j].team;
                c.regenPeriod = customSave.cells[j].regenerationPeriod;
                //c.um.upgrades = save.cells[j].installedUpgrades.upgrade;

                c.enabled = true;

                c.UpdateCellInfo();
            }
            init.StartAiInitialization(customSave.clans, customSave.difficulty);
        }
        else {
			SceneManager.LoadScene(Scenes.PROFILES);
			//throw new System.Exception();
		}
		file.Close();
	}
}