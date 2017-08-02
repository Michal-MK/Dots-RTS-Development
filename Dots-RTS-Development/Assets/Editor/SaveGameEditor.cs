using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;


public class EditorGUILayoutToggle : EditorWindow {
	bool isOn = false;
	int difficulty = 1;
	string levelName = "";

	bool saveButton = true;
	static GameObject canvas;

	[MenuItem("Campaign/SaveLevel")]
	static void Init() {
		EditorGUILayoutToggle window = (EditorGUILayoutToggle)GetWindow(typeof(EditorGUILayoutToggle), true, "My Empty Window");
		canvas = GameObject.Find("Canvas");
		window.Show();
		canvas.SetActive(false);
	}

	void OnGUI() {
		GUILayout.Label("Level Name");
		name = GUILayout.TextField(name, 50);
		difficulty = EditorGUILayout.IntField(new GUIContent("Level Difficulty"), difficulty);
		EditorGUILayout.LabelField("Current Level", GetCurLevel(difficulty));

		saveButton = EditorGUILayout.Toggle("Save Level", saveButton);
		if (saveButton) {
			#region Pre-Save Error checking


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
				return;
			}


			if (string.IsNullOrEmpty(name)) {
				return;
			}

			if (difficulty < 0 && difficulty > 5) {
				return;
			}
			#endregion

			BinaryFormatter formatter = new BinaryFormatter();
			string fileName = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + difficulty + Path.DirectorySeparatorChar + "Level_" + GetCurLevel(difficulty);
			using (FileStream file = File.Create(fileName + ".pwl")) {
				SaveDataCampaign save = new SaveDataCampaign();
				save.game = new SaveData();

				for (int i = 0; i < LevelEditorCore.cellList.Count; i++) {
					Cell c = LevelEditorCore.cellList[i];

					S_Cell serCell = new S_Cell();
					serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
					serCell.elementCount = c.elementCount;
					serCell.maxElementCount = c.maxElements;
					serCell.team = (int)c.cellTeam;
					serCell.regenerationPeriod = c.regenPeriod;
					serCell.installedUpgrades = new S_Upgrades { upgrade = c.um.ApplyUpgrades() };

					save.game.cells.Add(serCell);
				}
				save.game.difficulty = LevelEditorCore.aiDifficultyDict;
				save.game.gameSize = LevelEditorCore.gameSize;
				save.game.levelInfo = new LevelInfo(levelName, LevelEditorCore.authorName, DateTime.Now);
				save.isCleared = false;
				ScreenCapture.CaptureScreenshot(fileName + ".png");
				save.preview = fileName + ".png";
				formatter.Serialize(file, save);
				file.Close();
			}
		}
	}

	private void OnDestroy() {
		canvas.SetActive(true);
	}

	private string GetCurLevel(int dif) {
		if (dif > 0 && dif <= 5) {
			DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + dif);
			return (dir.GetFiles("*.pwl").Length + 1).ToString();
		}
		else {
			return "-1";
		}
	}
}

