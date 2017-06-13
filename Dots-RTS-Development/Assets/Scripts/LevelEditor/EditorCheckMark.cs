using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorCheckMark : MonoBehaviour {

	//This script reacts to the LevelEditorModeChanging

	//Attached Image
	//TODO: if there is no image attached it crashes
	Image myVeryOwnRenderer;
	
	// ManuallySetMode which if it is the one thats current it makes the script turn on the image
	public LevelEditorCore.Mode correspondingMode;
	
	
	void Start () {
		//Fetching the Image Component from the gameObject
		myVeryOwnRenderer = gameObject.GetComponent<Image>();

		// subscribing to the Event in LevelEditorCore
		LevelEditorCore.modeChange += ModeChangedWoah;
	}
	void OnDestroy() {
		// unbscribing to the Event in LevelEditorCore
		LevelEditorCore.modeChange -= ModeChangedWoah;
	}

	// ActualReaction
	public void ModeChangedWoah (LevelEditorCore.Mode mode) {
		// It is set in GameControll that this passes a mode along
		if (mode == correspondingMode) {
			myVeryOwnRenderer.enabled = true;
		}
		else{
			myVeryOwnRenderer.enabled = false;
		}
	}
}
