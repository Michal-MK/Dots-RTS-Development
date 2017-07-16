using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MainMenuUI  {
	
	public void Continue() {
		Control.UnPause();
	}

	public void EditMode() {
		throw new System.NotImplementedException("Transition to Level Editor is not yet Implemented");
	}

	public void PauseGameorEscape() {
		if (!Control.isPaused) {
			Control.Pause();
		}
		else {
			Control.UnPause();
		}
	}

}