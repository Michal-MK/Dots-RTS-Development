using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class SaveAndLoadEditor : MonoBehaviour {

	private static List<Cell> cellList = new List<Cell>();

	public GameObject prefab;

	public InputField fileNameInput;
	public InputField AiPeriodInput;
	public InputField levelNameInput;
	public InputField authorNameInput;
	public Text ErrorMessages;

	private string fileName;
	private float aiDiff;
	private string levelName;
	private string creator;

	private void Awake() {
		levelName = "DEFAULT";
		creator = SystemInfo.deviceName;
	}

	private void Start() {
		cellList.Clear();
	}
	private void OnDestroy() {
		cellList.Clear();
	}


	//
	#region Input filed data processing
	public void FileNameChanged() {
		fileName = fileNameInput.text;
		ErrorMessages.text = "File name changed.";
	}

	public void AiPeriodChanged() {
		if (float.TryParse(AiPeriodInput.text, out aiDiff) == false) {
			ErrorMessages.text = "Invalid difficulty";
			//aiDiff = 1;
		}
		else {
			ErrorMessages.text = "New difficulty: " + aiDiff;
		}
	}

	public void LevelNameChanged() {
		levelName = levelNameInput.text;
		ErrorMessages.text = "Level name changed.";
	}

	public void AuthorNameChanged() {
		creator = authorNameInput.text;
		ErrorMessages.text = "Author's name changed.";
	}
	#endregion
	//
	//
	#region Functions to add or remove a cell from the static list
	public static void AddCell(Cell c) {
		cellList.Add(c);
	}
	public static void RemoveCell(Cell c) {
		cellList.Remove(c);
	}
	#endregion
	//

#if UNITY_ANDROID


	public void Save() {
		ErrorMessages.text = "Can't chceck is a directory exists";

		if (!Directory.Exists(Application.persistentDataPath + "/Saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
			ErrorMessages.text = "Created the Saves directory";
		}

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/Saves/" + fileName + ".phage");
		if (File.Exists(Application.persistentDataPath + "/Saves/" + fileName + ".phage")) {
			ErrorMessages.text = "Succes, Created a file: " + fileName;
		}
		else {
			ErrorMessages.text = "Fail, change the file name";
			return;
		}
		SaveData save = new SaveData();


		for (int i = 0; i < cellList.Count; i++) {
			Cell c = cellList[i];

			S_Cell serCell = new S_Cell();
			serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
			serCell.elementCount = c.elementCount;
			serCell.maxElementCount = c.maxElements;
			serCell.team = (int)c._team;
			serCell.regenerationPeriod = c.regenPeriod;
			serCell.installedUpgrades = new S_Upgrades { upgrade = c.um.ApplyUpgrades() };

			save.cells.Add(serCell);
		}
		save.difficulty = aiDiff;
		formatter.Serialize(file, save);
		file.Close();
	}

	public void Load() {
		StartCoroutine(LoadCoroutine());
		//WWW loadStreamingAsset = new WWW("jar:file://" + Application.dataPath + "!assets/Saves/" + fileName + ".phage");

		////Freeze until it loads
		//while (loadStreamingAsset.isDone == false) {

		//}

		//if (!string.IsNullOrEmpty(loadStreamingAsset.error)) {
		//	ErrorMessages.text = (loadStreamingAsset.error);
		//}

		//if (loadStreamingAsset.text != "") {
		//	//ErrorMessages.text = loadStreamingAsset.text;
		//}
		//else {
		//	ErrorMessages.text = "No such file as " + fileName;
		//	return;
		//}

		//BinaryFormatter formatter = new BinaryFormatter();
		////DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Saves");
		////int numSaves = dir.GetFiles("*.phage").Length;
		////for (int i = 0; i < numSaves; i++) {
		//File.WriteAllBytes(Application.persistentDataPath + "/Saves/ " + fileName + ".phage", loadStreamingAsset.bytes);
		//FileStream file = File.Open(Application.persistentDataPath + "/Saves/" + fileName + ".phage", FileMode.Open);
		//SaveData save = (SaveData)formatter.Deserialize(file);
		//file.Close();
		//for (int j = 0; j < save.cells.Count; j++) {

		//	Cell c = Instantiate(prefab).GetComponent<Cell>();

		//	c.cellPosition = (Vector3)save.cells[j].pos;
		//	c.gameObject.transform.position = (Vector3)c.cellPosition;
		//	c.elementCount = save.cells[j].elementCount;
		//	c.maxElements = save.cells[j].maxElementCount;
		//	c._team = (Cell.enmTeam)save.cells[j].team;
		//	c.regenPeriod = save.cells[j].regenerationPeriod;
		//	c.um.upgrades = save.cells[j].installedUpgrades.upgrade;

		//}
	}
	IEnumerator LoadCoroutine () {
		WWW loadStreamingAsset = new WWW("jar:file://" + Application.dataPath + "!/assets/Saves/" + fileName + ".phage");

		yield return new WaitUntil(() => loadStreamingAsset.isDone == true);

		if (!string.IsNullOrEmpty(loadStreamingAsset.error)) {
			ErrorMessages.text = (loadStreamingAsset.error);
		}

		BinaryFormatter formatter = new BinaryFormatter();
		File.WriteAllBytes(Application.persistentDataPath + "/Saves/ " + fileName + ".phage", loadStreamingAsset.bytes);
		FileStream file = File.Open(Application.persistentDataPath + "/Saves/ " + fileName + ".phage", FileMode.Open);
		SaveData save = (SaveData)formatter.Deserialize(file);
		file.Close();
		for (int j = 0; j < save.cells.Count; j++) {

			Cell c = Instantiate(prefab).GetComponent<Cell>();

			c.cellPosition = (Vector3)save.cells[j].pos;
			c.gameObject.transform.position = (Vector3)c.cellPosition;
			c.elementCount = save.cells[j].elementCount;
			c.maxElements = save.cells[j].maxElementCount;
			c._team = (Cell.enmTeam)save.cells[j].team;
			c.regenPeriod = save.cells[j].regenerationPeriod;
			c.um.upgrades = save.cells[j].installedUpgrades.upgrade;


		}
	}
}
#else

	public void Save() {

		#region Pre-Save Error checking

		if (!Directory.Exists(Application.streamingAssetsPath + "\\Saves")) {
			Directory.CreateDirectory(Application.streamingAssetsPath + "\\Saves");
			ErrorMessages.text = "Created the Saves directory";
		}

		if (aiDiff == 0 || string.IsNullOrEmpty(fileName)) {
			ErrorMessages.text = "You did not provide all the necessary information!";
			if (aiDiff == 0) {
				ErrorMessages.text += " C'mon man, really 0 difficulty...?";
			}
			return;
		}

		int numAllies = 0;
		int numEnemies = 0;
		for (int i = 0; i < cellList.Count; i++) {
			if (cellList[i].cellTeam == Cell.enmTeam.ALLIED) {
				numAllies++;
			}
			if ((int)cellList[i].cellTeam >= (int)Cell.enmTeam.ENEMY1) {
				numEnemies++;
			}
		}
		if (numAllies == 0 || numEnemies == 0) {
			ErrorMessages.text = "Your level is missing an enemy, or you didn't create player's cell!";
			return;
		}

		#endregion

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.streamingAssetsPath + "\\Saves\\" + fileName + ".phage");

		if (File.Exists(Application.streamingAssetsPath + "\\Saves\\" + fileName + ".phage")) {
			ErrorMessages.text = "Succes, Created a file: " + fileName;
		}
		else {
			ErrorMessages.text = "Fail, change the file name";
			file.Close();
			return;
		}

		SaveData save = new SaveData();


		for (int i = 0; i < cellList.Count; i++) {
			if (cellList[i] == null) {
				ErrorMessages.text = "No cell number " + i + "found";
				file.Close();
				return;
			}
			Cell c = cellList[i];

			S_Cell serCell = new S_Cell();
			serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
			serCell.elementCount = c.elementCount;
			serCell.maxElementCount = c.maxElements;
			serCell.team = (int)c._team;
			serCell.regenerationPeriod = c.regenPeriod;
			serCell.installedUpgrades = new S_Upgrades { upgrade = c.um.ApplyUpgrades() };

			save.cells.Add(serCell);
		}
		save.difficulty = aiDiff;
		print(levelName + " " + creator);
		save.levelInfo = new LevelInfo(levelName, creator);
		formatter.Serialize(file, save);
		file.Close();
	}

	public void Load() {
		if (File.Exists(Application.streamingAssetsPath + "\\Saves\\" + fileName + ".phage")) {
			ErrorMessages.text = "Succes, Found a file: " + fileName;
		}
		else {
			ErrorMessages.text = "No such file as " + fileName;
			return;
		}

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Open(Application.streamingAssetsPath + "\\Saves\\" + fileName + ".phage", FileMode.Open);

		SaveData save = (SaveData)formatter.Deserialize(file);
		file.Close();

		for (int j = 0; j < save.cells.Count; j++) {

			Cell c = Instantiate(prefab).GetComponent<Cell>();

			c.cellPosition = (Vector3)save.cells[j].pos;
			c.gameObject.transform.position = (Vector3)c.cellPosition;
			c.elementCount = save.cells[j].elementCount;
			c.maxElements = save.cells[j].maxElementCount;
			c._team = (Cell.enmTeam)save.cells[j].team;
			c.regenPeriod = save.cells[j].regenerationPeriod;
			c.um.upgrades = save.cells[j].installedUpgrades.upgrade;

			cellList.Add(c);


		}
	}
}

#endif


[Serializable]
public class LevelInfo {
	public string levelName;
	public string creator;

	public LevelInfo(string levelName, string creator) {
		this.levelName = levelName;
		this.creator = creator;
	}
}

[Serializable]
public class SaveData {
	public List<S_Cell> cells = new List<S_Cell>();
	public LevelInfo levelInfo;
	public float difficulty;
}

[Serializable]
public class S_Cell {
	public S_Vec3 pos;
	public int elementCount;
	public int maxElementCount;
	public int team;
	public float regenerationPeriod;
	public S_Upgrades installedUpgrades;

}

[Serializable]
public class S_Vec3 {
	public float x, y, z;

	public static explicit operator Vector3(S_Vec3 v) {
		return new Vector3(v.x, v.y, v.z);
	}
}

[Serializable]
public class S_Upgrades {
	public int[] upgrade;
}
