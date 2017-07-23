using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class SaveAndLoadEditor : MonoBehaviour {


	public GameObject prefab;

	public InputField fileNameInput;
	public InputField levelNameInput;
	public InputField authorNameInput;
	public Text ErrorMessages;

	public static string fileName;


	private void Awake() {
		ErrorMessages.text = Application.persistentDataPath;
	}

	private void Start() {

		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("LoadLevelFilePath"))) {
			Load(PlayerPrefs.GetString("LoadLevelFilePath"));
		}
		//fileName = string.Format(DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString());
	}
	private void OnDestroy() {
	}


	//
	//
	//

	//

	public void Save() {
		fileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
		
		#region Pre-Save Error checking

#if !(UNITY_ANDROID || UNITY_IOS)
		if (!Directory.Exists(Application.streamingAssetsPath + "\\Saves")) {
			Directory.CreateDirectory(Application.streamingAssetsPath + "\\Saves");
			ErrorMessages.text = "Created the Saves directory";
		}
#else
		if (!Directory.Exists(Application.persistentDataPath + "/Saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
			ErrorMessages.text = "Created the Saves directory";
		}
#endif
		ErrorMessages.text = "";

		int numAllies = 0;
		int numEnemies = 0;
		for (int i = 0; i < LevelEditorCore.cellList.Count; i++) {
			if (LevelEditorCore.cellList[i].cellTeam == Cell.enmTeam.ALLIED) {
				numAllies++;
			}
			if ((int)LevelEditorCore.cellList[i].cellTeam >= (int)Cell.enmTeam.ENEMY1) {
				numEnemies++;
			}
		}
		if (numAllies == 0 || numEnemies == 0) {
			ErrorMessages.text = "Your level is missing an enemy, or you didn't create player's cell!";
			return;
		}

		ErrorMessages.text += "You picked the fileName: " + fileName+ ". \n";

		if (LevelEditorCore.levelName == gameObject.GetComponent<LevelEditorCore>().defaultLevelName) {
			ErrorMessages.text += "You picked the default levelName: " + LevelEditorCore.levelName + ". \n";
		}
		else {
			ErrorMessages.text += "You picked the levelName: " + LevelEditorCore.levelName + ". \n";
		}

		if (LevelEditorCore.authorName == gameObject.GetComponent<LevelEditorCore>().defaultAuthorName) {
			ErrorMessages.text += "You picked the default authorName: " + LevelEditorCore.authorName + ". \n";
		}
		else {
			ErrorMessages.text += "You picked the authorName: " + LevelEditorCore.authorName + ". \n";
		}


		#endregion

#if (UNITY_ANDROID || UNITY_IOS)

		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/Saves/" + fileName+ ".phage");
		if (File.Exists(Application.persistentDataPath + "/Saves/" + fileName+ ".phage")) {
			ErrorMessages.text += "Succes, Created a file: " + fileName;
		}
		else {
			ErrorMessages.text += "Fail, change the file name";
			return;
		}
		SaveData save = new SaveData();


		for (int i = 0; i < LevelEditorCore.cellList.Count; i++) {
			Cell c = LevelEditorCore.cellList[i];

			S_Cell serCell = new S_Cell();
			serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
			serCell.elementCount = c.elementCount;
			serCell.maxElementCount = c.maxElements;
			serCell.team = (int)c.cellTeam;
			serCell.regenerationPeriod = c.regenPeriod;
			serCell.installedUpgrades = new S_Upgrades { upgrade = c.um.ApplyUpgrades() };

			save.cells.Add(serCell);
		}
		save.difficulty = LevelEditorCore.aiDificulty;
		save.levelInfo = new LevelInfo(LevelEditorCore.levelName, LevelEditorCore.authorName, DateTime.Now);
		ErrorMessages.text += "  displayName:(" + save.levelInfo.levelName + ")";
		formatter.Serialize(file, save);
		file.Close();
#else



		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.streamingAssetsPath + "\\Saves\\" + fileName+ ".phage");

		if (File.Exists(Application.streamingAssetsPath + "\\Saves\\" + fileName+ ".phage")) {
			ErrorMessages.text += "Succes, Created a file: " + fileName;
		}
		else {
			ErrorMessages.text += "Fail, change the file name";
			file.Close();
			return;
		}

		SaveData save = new SaveData();


		for (int i = 0; i < LevelEditorCore.cellList.Count; i++) {
			if (LevelEditorCore.cellList[i] == null) {
				ErrorMessages.text += "No cell number " + i + "found";
				file.Close();
				return;
			}
			Cell c = LevelEditorCore.cellList[i];

			S_Cell serCell = new S_Cell();
			serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
			serCell.elementCount = c.elementCount;
			serCell.maxElementCount = c.maxElements;
			serCell.team = (int)c.cellTeam;
			serCell.regenerationPeriod = c.regenPeriod;
			serCell.installedUpgrades = new S_Upgrades { upgrade = c.um.ApplyUpgrades() };

			save.cells.Add(serCell);
		}
		save.difficulty = LevelEditorCore.aiDificulty;
		save.gameSize = LevelEditorCore.gameSize;
		print(LevelEditorCore.levelName + " " + LevelEditorCore.authorName);
		save.levelInfo = new LevelInfo(LevelEditorCore.levelName, LevelEditorCore.authorName, DateTime.Now);
		formatter.Serialize(file, save);
		file.Close();
#endif
	}

	public void Load(string path) {
		BinaryFormatter formatter = new BinaryFormatter();
		//File.WriteAllBytes(Application.persistentDataPath + "/Saves/ " + fileName + ".phage", loadStreamingAsset.bytes);
		FileStream file = File.Open(path, FileMode.Open);
		SaveData save = (SaveData)formatter.Deserialize(file);
		LevelEditorCore.gameSize = save.gameSize;
		gameObject.GetComponent<LevelEditorCore>().RefreshCameraSize();
		file.Close();

		for (int j = 0; j < save.cells.Count; j++) {

			Cell c = Instantiate(prefab).GetComponent<Cell>();

			c.cellPosition = (Vector3)save.cells[j].pos;
			c.gameObject.transform.position = (Vector3)c.cellPosition;
			c.elementCount = save.cells[j].elementCount;
			c.maxElements = save.cells[j].maxElementCount;
			c.cellTeam = (Cell.enmTeam)save.cells[j].team;
			c.regenPeriod = save.cells[j].regenerationPeriod;
			c.um.upgrades = save.cells[j].installedUpgrades.upgrade;

			LevelEditorCore.AddCell(c);
		}
	}
}

//public void Save() {

//	BinaryFormatter formatter = new BinaryFormatter();
//	FileStream file = File.Create(Application.streamingAssetsPath + "\\Saves\\" + fileName + ".phage");

//	if (File.Exists(Application.streamingAssetsPath + "\\Saves\\" + fileName + ".phage")) {
//		ErrorMessages.text = "Succes, Created a file: " + fileName;
//	}
//	else {
//		ErrorMessages.text = "Fail, change the file name";
//		file.Close();
//		return;
//	}

//	SaveData save = new SaveData();


//	for (int i = 0; i < LevelEditorCore.cellList.Count; i++) {
//		if (LevelEditorCore.cellList[i] == null) {
//			ErrorMessages.text = "No cell number " + i + "found";
//			file.Close();
//			return;
//		}
//		Cell c = LevelEditorCore.cellList[i];

//		S_Cell serCell = new S_Cell();
//		serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
//		serCell.elementCount = c.elementCount;
//		serCell.maxElementCount = c.maxElements;
//		serCell.team = (int)c.cellTeam;
//		serCell.regenerationPeriod = c.regenPeriod;
//		serCell.installedUpgrades = new S_Upgrades { upgrade = c.um.ApplyUpgrades() };

//		save.cells.Add(serCell);
//	}
//	save.difficulty = aiDiff;
//	print(levelName + " " + creator);
//	save.levelInfo = new LevelInfo(levelName, creator);
//	formatter.Serialize(file, save);
//	file.Close();
//}


//}
//}

//#endif


[Serializable]
public class LevelInfo {
	public string levelName;
	public string creator;
	public DateTime creationTime;

	public LevelInfo(string levelName, string creator, DateTime time) {
		this.levelName = levelName;
		this.creator = creator;
		this.creationTime = time;
	}
}

[Serializable]
public class SaveData {
	public List<S_Cell> cells = new List<S_Cell>();
	public LevelInfo levelInfo;
	public float difficulty;
	public float gameSize;
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
