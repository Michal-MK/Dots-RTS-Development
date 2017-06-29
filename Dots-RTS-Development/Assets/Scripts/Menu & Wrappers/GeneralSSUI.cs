using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralSSUI : MonoBehaviour {

	// Use this for initialization
	public void ToMainMenu () {
		SceneManager.LoadScene("Main Menu");
	}

	// Update is called once per frame
	public void ToLevelSelect() {
		SceneManager.LoadScene("LevelSelect");
	}
}
