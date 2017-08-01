using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;

class CampaignLevel : MonoBehaviour {

	#region Preafab References
	public RawImage preview;
	public Image isPassed;
	public Text levelName;
	public Text clearTime;
	#endregion

	[HideInInspector]
	public string levelPath;

	private void Start() {

		BinaryFormatter bf = new BinaryFormatter();

		SaveDataCampaign levelInfo;
		using (FileStream fs = new FileStream(levelPath, FileMode.Open)) {

			levelInfo = (SaveDataCampaign)bf.Deserialize(fs);
			fs.Close();

		}

		Texture2D tex = new Texture2D(190, 80);
		tex.LoadImage(File.ReadAllBytes(levelInfo.preview));

		preview.texture = tex;

		if (levelInfo.isCleared) {
			isPassed.enabled = true;
			clearTime.text = string.Format("{0:00}:{1:00}.{2:00} minutes", levelInfo.timeUnformated / 60, levelInfo.timeUnformated % 60, levelInfo.timeUnformated.ToString().Remove(0, levelInfo.timeUnformated.ToString().Length - 2));

		}
		else {
			isPassed.enabled = false;
			clearTime.text = "TBD";
		}

		levelName.text = levelInfo.game.levelInfo.levelName;
	}

}

