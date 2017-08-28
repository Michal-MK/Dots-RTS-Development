using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorCheckMark : MonoBehaviour {

	public Image checkMarkImg;                                                                              //Attached Image
	public LevelEditorCore.Mode correspondingMode;                                                         // ManuallySetMode which if it is the one thats current it makes the script turn on the image

	void Start() {
		LevelEditorCore.modeChange += UpdateCheckmark;
	}
	void OnDestroy() {
		LevelEditorCore.modeChange -= UpdateCheckmark;
	}

	// ActualReaction
	public void UpdateCheckmark(LevelEditorCore.Mode mode) {
		//print("called");
		// It is set in GameControll that this passes a mode along
		if (mode == correspondingMode) {
			checkMarkImg.enabled = true;
		}
		else {
			checkMarkImg.enabled = false;
		}
	}
}
