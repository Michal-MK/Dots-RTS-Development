﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AllAiDifficultyWriter : MonoBehaviour {

	public static Text myText;

	public static void RedoText() {
		if (myText == null) {
			myText = GameObject.Find("Canvas").transform.Find("GameSettingsPanel/AiDiffWriter").GetComponent<Text>();
		}

		myText.text = "";

		foreach (Cell.enmTeam teamEnm in LevelEditorCore.teamList) {
			int team = (int)teamEnm;
			//print(team);
			float diff;
			// print(team);
			//if (LevelEditorCore.aiDifficultyDict.TryGetValue(team, out diff)) {
			//    print("rip me");
			//}
			if (LevelEditorCore.aiDifficultyDict.TryGetValue(team, out diff)) {
				myText.text += "Enemy " + (team - 1) + " does an action every " + diff + " seconds \n";
			}
			else {
				myText.text += "Enemy " + (team - 1) + "'s difficulty is not assinged \n";
			}

		}
	}
}
