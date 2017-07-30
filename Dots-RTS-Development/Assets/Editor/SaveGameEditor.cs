using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;

[CustomEditor(typeof(SaveAndLoadEditor))]
public class SaveGameEditor : Editor {

	bool isOn = false;
	int difficulty = 1;
	string name = "";

	public override void OnInspectorGUI() {
		isOn = GUILayout.Toggle(isOn, new GUIContent("Campaign Creation"));

		if (!isOn) {
			base.OnInspectorGUI();
		}
		else {

			SaveAndLoadEditor script = (SaveAndLoadEditor)target;
			GUILayout.Label("Level Name");
			name = GUILayout.TextField(name, 50);
			difficulty = EditorGUILayout.IntField(new GUIContent("Level Difficulty"), difficulty);
			EditorGUILayout.LabelField("Current Level", GetCurLevel(difficulty));
			if (GUILayout.Button("Save")) {

				string fileName = "Level_" + GetCurLevel(difficulty);

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


				if (LevelEditorCore.levelName == script.gameObject.GetComponent<LevelEditorCore>().defaultLevelName) {
				}
				else {
				}

				if (LevelEditorCore.authorName == script.GetComponent<LevelEditorCore>().defaultAuthorName) {
				}
				else {
				}
				#endregion

				BinaryFormatter formatter = new BinaryFormatter();
				using (FileStream file = File.Create(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + difficulty + Path.DirectorySeparatorChar + fileName + ".pwl")) {
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
					save.game.difficulty = LevelEditorCore.aiDificulty;
					save.game.gameSize = LevelEditorCore.gameSize;
					save.game.levelInfo = new LevelInfo(name, LevelEditorCore.authorName, DateTime.Now);
					save.isCleared = false;
					string imgPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + difficulty + Path.DirectorySeparatorChar + fileName + ".png";
					ScreenCapture.CaptureScreenshot(imgPath);
					save.preview = imgPath;
					formatter.Serialize(file, save);
					file.Close();
				}
			}
		}
	}

	private string GetCurLevel(int dif) {
		DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + dif);
		return (dir.GetFiles("*.pwl").Length + 1).ToString();
	}
}

public class CampaignLevelCreator : EditorWindow {
	bool showBtn = true;


	[MenuItem("Campaign/SaveLevel")]
	static void Init() {
		CampaignLevelCreator window = (CampaignLevelCreator)GetWindow(typeof(CampaignLevelCreator), true, "My Empty Window");
		window.Show();
	}

	void OnGUI() {
		showBtn = EditorGUILayout.Toggle("Show Button", showBtn);
		if (showBtn) {
			if (GUILayout.Button("Close")) {
				Close();
			}
		}
	}
}

