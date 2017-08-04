using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MainMenuUI  {
	
	public void Continue() {
		Control.UnPause();
	}

	public void EditMode() {
		Control.cells.Clear();
		SceneManager.LoadScene(1);
	}

	public void PauseGameorEscape() {
		if (!Control.isPaused) {
			Control.Pause();
		}
		else {
			Control.UnPause();
		}
	}

	public void CreateNewProfile() {
		Control.pM.profileCreation.SetActive(true);
	}
}