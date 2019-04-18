using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignLevel : MonoBehaviour {

	#region Prefab References
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
		if (ProfileManager.CurrentProfile == null) {
			Control.DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene(Scenes.PROFILES);
			return;
		}


		using (FileStream fs = new FileStream(levelPath, FileMode.Open)) {
			BinaryFormatter bf = new BinaryFormatter();

			SaveDataCampaign levelInfo = (SaveDataCampaign)bf.Deserialize(fs);
			Texture2D tex = new Texture2D(190, 80);
			tex.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + levelInfo.preview));
			preview.texture = tex;

			levelName.text = levelInfo.game.levelInfo.levelName;
		}

		foreach (KeyValuePair<SaveDataCampaign, float> passedLevel in ProfileManager.CurrentProfile.ClearedCampaign) {
			if (passedLevel.Value != 0f) {
				passedImg.gameObject.SetActive(true);
				clearTime.text = string.Format("{0:00}:{1:00}.{2:00} minutes", passedLevel.Value / 60, passedLevel.Value % 60f, passedLevel.Value.ToString().Remove(0, passedLevel.Value.ToString().Length - 2));
			}
			else {
				passedImg.gameObject.SetActive(false);
				clearTime.text = "TBD";
			}
		}
	}

	public void StartLevel() {
		current = this;
		PlayerPrefs.SetString("LoadLevelFilePath", levelPath);
		PlayManager.levelState = PlayManager.PlaySceneState.CAMPAIGN;
		SceneManager.LoadScene(Scenes.GAME);
		transform.parent = null;
		DontDestroyOnLoad(gameObject);
	}
}

