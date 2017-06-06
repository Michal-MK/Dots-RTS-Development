using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SavingScript : MonoBehaviour {

	private static List<Cell> cellList = new List<Cell>();

	public static void AddCell(Cell cellInfo) {
		cellList.Add(cellInfo);
	}
	public static void RemoveCell(Cell cellInfo) {
		cellList.Remove(cellInfo);
	}
	// Use this for initialization
	public void Save () {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.dataPath + "/Saves/Level.phage");
		SaveData currentData = new SaveData();

		SaveData.posX = new float[cellList.Count];
		SaveData.posY = new float[cellList.Count];

		SaveData.regenFrequency = new float[cellList.Count];
		SaveData.elementCount = new int[cellList.Count];
		SaveData.maxElementCount = new int[cellList.Count];
		SaveData.team = new int[cellList.Count];

		for (int i = 0; i < cellList.Count; i++) {
			SaveData.posX[i] = cellList[i].transform.position.x;
			SaveData.posY[i] = cellList[i].transform.position.y;

			SaveData.posY[i] = cellList[i].regenFrequency;
			SaveData.posY[i] = cellList[i].elementCount;
			SaveData.posY[i] = cellList[i].maxElements;
			SaveData.posY[i] = (int)cellList[i].cellTeam;
		}

		formatter.Serialize(file, currentData);
		file.Close();
	}
	
	
	void Update () {
		
	}

	
}

[Serializable]
public class SaveData {

	public static float[] posX;
	public static float[] posY;

	public static float[] regenFrequency;
	public static int[] elementCount;
	public static int[] maxElementCount;
	public static int[] team;
}

