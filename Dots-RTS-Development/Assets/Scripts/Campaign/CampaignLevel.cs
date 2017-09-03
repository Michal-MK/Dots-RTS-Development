using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class CampaignLevel : MonoBehaviour {

	#region Preafab References
	public RawImage preview;
	public Image passedImg;
	public TextMeshProUGUI levelName;
	public TextMeshProUGUI clearTime;
	#endregion

	public static CampaignLevel current = null;
	public SaveDataCampaign currentSaveData;

	[HideInInspector]
	public string levelPath;

	private void Start() {
		if(ProfileManager.getCurrentProfile == null) {
			Control.DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene(Scenes.PROFILES);
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();

		SaveDataCampaign levelInfo;
		using (FileStream fs = new FileStream(levelPath, FileMode.Open)) {

			levelInfo = (SaveDataCampaign)bf.Deserialize(fs);
			fs.Close();
		}
		foreach (KeyValuePair<SaveDataCampaign,float> passedLevel in ProfileManager.getCurrentProfile.clearedCampaignLevels) {
			if(passedLevel.Value != 0f) {
				passedImg.gameObject.SetActive(true);
				clearTime.text = string.Format("{0:00}:{1:00}.{2:00} minutes", passedLevel.Value / 60, passedLevel.Value % 60f, passedLevel.Value.ToString().Remove(0, passedLevel.Value.ToString().Length - 2));
			}
			else {
				passedImg.gameObject.SetActive(false);
				clearTime.text = "TBD";

			}
		}
		Texture2D tex = new Texture2D(190, 80);
		tex.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + levelInfo.preview));
		preview.texture = tex;
		levelName.text = levelInfo.game.levelInfo.levelName;
	}

	public void StartLevel() {
		transform.parent = null;
		DontDestroyOnLoad(gameObject);
		current = this;
		PlayerPrefs.SetString("LoadLevelFilePath", levelPath);
		Control.levelState = Control.PlaySceneState.CAMPAIGN;
		SceneManager.LoadScene(Scenes.PLAYER);
	}
}

[Serializable]
public class CampaignLevelCode {
	private int Difficulty;
	private int LevelInDiff;
	public CampaignLevelCode(int difficulty, int levelInDifficulty) {
		this.Difficulty = difficulty;
		this.LevelInDiff = levelInDifficulty;
	}
	
	public int difficulty {
		get {
			return Difficulty;
		}
	}

	public int level {
		get {
			return LevelInDiff;
		}
	}
}

