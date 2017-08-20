using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SaveAndLoadEditor : MonoBehaviour {

	public GameObject prefab;


	public Text ErrorMessages;

	public static string fileName;


	private void Awake() {
		ErrorMessages.text = Application.persistentDataPath;
	}

	private void Start() {
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("LoadLevelFilePath"))) {
			Load(PlayerPrefs.GetString("LoadLevelFilePath"));
           // print("thathappened");
		}
	}

    public void TryLevel() {
        string path = Save(true);
        PlayerPrefs.SetString("LoadLevelFilePath", path);
        Control.levelState = Control.PlaySceneState.PREVIEW;
        SceneManager.LoadScene("Level_Player");
    }

	public string Save(bool temp = false) {
		fileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        if (temp) {
            fileName = "testLevel";
        }
        string fullpath;
#if UNITY_ANDROID
        fullpath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".phage";
#else
        fullpath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + fileName + ".phage";
#endif

        #region Pre-Save Error checking

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
			return fullpath;
		}

		ErrorMessages.text += "You picked the fileName: " + fileName + ". \n";

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

		BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(fullpath);
        SaveData save = new SaveData();

        for (int i = 0; i < LevelEditorCore.cellList.Count; i++) {
			Cell c = LevelEditorCore.cellList[i];

			S_Cell serCell = new S_Cell();
			serCell.pos = new S_Vec3 { x = c.transform.position.x, y = c.transform.position.y, z = c.transform.position.z };
			serCell.elementCount = c.elementCount;
			serCell.maxElementCount = c.maxElements;
			serCell.team = (int)c.cellTeam;
			serCell.regenerationPeriod = c.regenPeriod;
			//serCell.installedUpgrades = new S_Upgrades { upgrade = c.um.ApplyUpgrades() };

			save.cells.Add(serCell);
		}
        save.savedAtAspect = Camera.main.aspect;
		save.difficulty = LevelEditorCore.aiDifficultyDict;
		save.gameSize = LevelEditorCore.gameSize;
		save.levelInfo = new LevelInfo(LevelEditorCore.levelName, LevelEditorCore.authorName, DateTime.Now);
        save.clans = TeamSetup.clanDict;
		ErrorMessages.text += "  displayName:(" + save.levelInfo.levelName + ")";
		formatter.Serialize(file, save);
		file.Close();
        return fullpath;
	}

	public void Load(string path) {
        foreach (Cell c in LevelEditorCore.cellList) {
            Destroy(c.gameObject);
        }
        LevelEditorCore.cellList.Clear();
        TeamSetup.clanDict.Clear();
        LevelEditorCore.aiDifficultyDict.Clear();


        BinaryFormatter formatter = new BinaryFormatter();
		//File.WriteAllBytes(Application.persistentDataPath + "/Saves/ " + fileName + ".phage", loadStreamingAsset.bytes);
		FileStream file = File.Open(path, FileMode.Open);
		SaveData save = (SaveData)formatter.Deserialize(file);
        LevelEditorCore.gameSize = save.gameSize;

        LevelEditorCore.aiDifficultyDict = save.difficulty;

        if (save.difficulty != null) {
            LevelEditorCore.aiDifficultyDict = save.difficulty;
        }
        else {
            LevelEditorCore.aiDifficultyDict =new Dictionary<int, float>();
            //print("diff null");
        }
        if (save.clans != null) {
            TeamSetup.clanDict = save.clans;
        }
        else {
            TeamSetup.clanDict = new Dictionary<int, int>();
            //print("clan null");
        }


        //Dictionary<int, float>.KeyCollection keys = LevelEditorCore.aiDifficultyDict.Keys;
        //foreach (int j in keys) {
        //    //print(j + " Key");
        //   float diffOfJ;
        //    LevelEditorCore.aiDifficultyDict.TryGetValue(j, out diffOfJ);
        //    string s = ("teaj " + j + " is " + diffOfJ + " difficult");
        //    print(s);

        //}
        file.Close();

		for (int j = 0; j < save.cells.Count; j++) {

			Cell c = Instantiate(prefab).GetComponent<Cell>();

			c.cellPosition = (Vector3)save.cells[j].pos;
			c.gameObject.transform.position = c.cellPosition;
			c.elementCount = save.cells[j].elementCount;
			c.maxElements = save.cells[j].maxElementCount;
			c.cellTeam = (Cell.enmTeam)save.cells[j].team;
			c.regenPeriod = save.cells[j].regenerationPeriod;
			//c.um.upgrades = save.cells[j].installedUpgrades.upgrade;

			LevelEditorCore.AddCell(c);
		}
	}
}



