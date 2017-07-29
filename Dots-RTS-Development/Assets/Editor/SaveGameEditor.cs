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

	public override void OnInspectorGUI() {
		isOn = GUILayout.Toggle(isOn, new GUIContent("Campaign Creation"));

		if (!isOn) {
			base.OnInspectorGUI();
		}
		else {

			SaveAndLoadEditor script = (SaveAndLoadEditor)target;
			GUILayout.Label("Level Name");
			string name = GUILayout.TextField("Name", 50);
			int difficulty = EditorGUILayout.IntField(new GUIContent("Level Difficulty"), 1);
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
				using (FileStream file = File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".pwl")) {
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
					save.gameSize = LevelEditorCore.gameSize;
					save.levelInfo = new LevelInfo(LevelEditorCore.levelName, LevelEditorCore.authorName, DateTime.Now);
					formatter.Serialize(file, save);
					file.Close();
				}
			}
		}
	}

	private string GetCurLevel(int dif) {
		DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + dif);
		return dir.GetFiles("*.pwl").Length.ToString();
	}
}

public class EditorGUILayoutToggle : EditorWindow {
	bool showBtn = true;

	[MenuItem("Campaign/Test")]
	static void Init() {
		EditorGUILayoutToggle window = (EditorGUILayoutToggle)GetWindow(typeof(EditorGUILayoutToggle), true, "My Empty Window");
		window.Show();
	}

	void OnGUI() {
		showBtn = EditorGUILayout.Toggle("Show Button", showBtn);
		if (showBtn)
			if (GUILayout.Button("Close"))
				Close();
	}

	[MenuItem("CONTEXT/SaveFileInfo/Double Mass")]
	static void DoubleMass(MenuCommand command) {
		SaveFileInfo body = (SaveFileInfo)command.context;
		Debug.Log("Doubled Rigidbody's Mass to " + body.name + " from Context Menu.");
	}

	[MenuItem("GameObject/MyCategory/Custom Game Object", false, 10)]
	static void CreateCustomGameObject(MenuCommand menuCommand) {
		// Create a custom game object
		GameObject go = new GameObject("Custom Game Object");
		// Ensure it gets reparented if this was a context click (otherwise does nothing)
		GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}
}

