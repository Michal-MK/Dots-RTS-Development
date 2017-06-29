using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectScript : MonoBehaviour {

	public InputField manualFileNameIF;
	// Use this for initialization
#if UNITY_ANDROID
	public void loadButtonPress() {
		PlayerPrefs.SetString("LoadLevelFilePath", Application.persistentDataPath + "/Saves/" + manualFileNameIF.text + ".phage");
		SceneManager.LoadScene("LevelPlayer");
	}
#else
	public void loadButtonPress() {
		PlayerPrefs.SetString("LoadLevelFilePath", Application.streamingAssetsPath + "/Saves/" + manualFileNameIF.text + ".phage");
		SceneManager.LoadScene("LevelPlayer");
	}
#endif

}
