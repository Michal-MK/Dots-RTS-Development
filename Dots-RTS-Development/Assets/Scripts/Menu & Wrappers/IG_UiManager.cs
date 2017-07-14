using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IG_UiManager : MonoBehaviour {

	public Text errorText;
	// Use this for initialization
	public void FoundAFile() {
		
	}

	// Update is called once per frame
	public void NoFileFound() {
		StartCoroutine(ReturnToLevelSelectIn(5));
		errorText.gameObject.SetActive(true);
		errorText.text = "No file found, returing to level select";
	}

	IEnumerator ReturnToLevelSelectIn(float seconds) {
		yield return new WaitForSeconds(seconds);
		SceneManager.LoadScene(2);
	}
}
