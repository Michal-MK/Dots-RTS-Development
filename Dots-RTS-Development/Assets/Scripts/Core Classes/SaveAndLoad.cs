using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SaveAndLoad {

	private static List<Cell> cellList = new List<Cell>();

	public static void AddCell(Cell c) {
		cellList.Add(c);
	}
	public static void RemoveCell(Cell c) {
		cellList.Remove(c);
	}


	public void Save() {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.dataPath + "/Saves/Level.phage");
		SaveData save = new SaveData();


		for (int i = 0; i < cellList.Count; i++) {
			Cell c = cellList[i];

			S_Cell serCell = new S_Cell();
			serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
			serCell.elementCount = c.elementCount;
			serCell.maxElementCount = c.maxElements;
			serCell.team = (int)c._team;
			serCell.regenerationPeriod = c.regenPeriod;
			serCell.installedUpgrades = new S_Upgrades { upgrade = c.um.ApplyUpgrades()};

			save.cells.Add(serCell);
		}

		formatter.Serialize(file, save);
		file.Close();
	}

	public void Load(int index = 0) {
		BinaryFormatter formatter = new BinaryFormatter();
		//DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Saves");
		//int numSaves = dir.GetFiles("*.phage").Length;
		//for (int i = 0; i < numSaves; i++) {
			FileStream file = File.Open(Application.dataPath + "/Saves/Level" + index + ".phage", FileMode.Open);
			SaveData save = (SaveData)formatter.Deserialize(file);
			file.Close();
			for (int j = 0; j < save.cells.Count; j++) {

				Cell c = new Cell();

				c.cellPosition = (Vector3)save.cells[j].pos;
				c.elementCount = save.cells[j].elementCount;
				c.maxElements = save.cells[j].maxElementCount;
				c._team = (Cell.enmTeam)save.cells[j].team;
				c.regenPeriod = save.cells[j].regenerationPeriod;
				c.um.upgrades = save.cells[j].installedUpgrades.upgrade;
			}
		//}
	}
}

[Serializable]
public class SaveData {
	public List<S_Cell> cells = new List<S_Cell>();
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
