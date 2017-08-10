using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadFromFile : MonoBehaviour {

	public GameObject cellPrefab;
	//public GameObject gameControlllPrefab;
	public Initialize_AI init;
	// Use this for initialization
	void Start () {
		

		if (File.Exists(PlayerPrefs.GetString("LoadLevelFilePath"))) {
			gameObject.SendMessage("FoundAFile",SendMessageOptions.DontRequireReceiver);
			Debug.LogWarning("FoundAFile does not have a receiver.");
		}
		else {
			gameObject.SendMessage("NoFileFound");
			return;
		}

		BinaryFormatter formatter = new BinaryFormatter();

		FileStream file = File.Open(PlayerPrefs.GetString("LoadLevelFilePath"), FileMode.Open);
		SaveData save = (SaveData)formatter.Deserialize(file);
		
		file.Close();
		if (save.gameSize != 0) {
			Camera.main.orthographicSize = save.gameSize;
		}

		Dictionary<int, float>.KeyCollection diffKeys = save.difficulty.Keys;
		foreach (int key in diffKeys) {
			save.difficulty.TryGetValue(key, out init.decisionSpeeds[key - 2]);
		}
		
		for (int j = 0; j < save.cells.Count; j++) {

			CellBehaviour c = Instantiate(cellPrefab).GetComponent<CellBehaviour>();

			c.cellPosition = (Vector3)save.cells[j].pos;
			c.gameObject.transform.position = c.cellPosition;
			c.elementCount = save.cells[j].elementCount;
			c.maxElements = save.cells[j].maxElementCount;
			c.cellTeam = (Cell.enmTeam)save.cells[j].team;
			c.regenPeriod = save.cells[j].regenerationPeriod;
			//c.um.upgrades = save.cells[j].installedUpgrades.upgrade;

			c.enabled = true;

			c.UpdateCellInfo();
		}
		init.StartAiInitialization(save.clans);
	}
}
