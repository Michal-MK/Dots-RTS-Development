using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {


	#region Menu Refs

	#endregion

	#region Editor Refs

	#endregion

	#region LevelSelect Refs
	//private bool isDisplayingCampaign;
	private RectTransform rectCampaign;
	private RectTransform rectCustom;
	private GameObject centralToMainMenu;
	private GameObject campaignButton;
	private GameObject customButton;
	#endregion

	#region LevelShare Refs

	#endregion



	private void Awake() {
		SceneManager.activeSceneChanged += SceneChanged;
	}

	private void SceneChanged(Scene oldS, Scene newS) {
		if (newS.buildIndex == 2) {
			rectCampaign = GameObject.Find("Canvas_Campaign").GetComponent<RectTransform>();
			rectCustom = GameObject.Find("Canvas_CustomLevels").GetComponent<RectTransform>();
			centralToMainMenu = GameObject.Find("Return_To_Menu");
			campaignButton = GameObject.Find("Campaign_Button");
			customButton = GameObject.Find("Custom_Button");
		}
	}

	// Turns OFF the game
	public void ExitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
	}



	//Switch scene accroding to its build index
	public void SwitchScene(int sceneIndex) {

		if (SceneManager.GetActiveScene().buildIndex == 1) {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		if (sceneIndex == 1) {
			PlayerPrefs.SetString("LoadLevelFilePath", null);
		}
		Control.cells.Clear();
		SceneManager.LoadScene(sceneIndex);

	}
	public void SwitchScene(string Name) {
		if (SceneManager.GetActiveScene().buildIndex == 1) {
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
		Control.cells.Clear();
		SceneManager.LoadScene(Name);
	}

	public void DisplaySelection(bool isCampaign) {
		if (isCampaign) {
			//isDisplayingCampaign = true;
			rectCustom.anchoredPosition = new Vector3(2048, 0);
			rectCampaign.anchoredPosition = new Vector3(0, 0);

		}
		else {
			//isDisplayingCampaign = false;
			rectCampaign.anchoredPosition = new Vector3(-2048, 0);
			rectCustom.anchoredPosition = new Vector3(0, 0);
		}
		centralToMainMenu.SetActive(false);
		campaignButton.SetActive(false);
		customButton.SetActive(false);

	}

	public void ReturnToDefaultScreen() {
		rectCampaign.anchoredPosition = new Vector3(-2048, 0);
		rectCustom.anchoredPosition = new Vector3(2048, 0);
		centralToMainMenu.SetActive(true);
		campaignButton.SetActive(true);
		customButton.SetActive(true);

	}
}