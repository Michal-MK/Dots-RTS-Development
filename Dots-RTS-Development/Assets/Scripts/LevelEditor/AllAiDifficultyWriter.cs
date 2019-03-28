using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllAiDifficultyWriter : MonoBehaviour {

	public static Text myText;

	public static void RedoText(Dictionary<Team,float> diffDict) {
		if (myText == null) {
			myText = GameObject.Find("Canvas").transform.Find("GameSettingsPanel/RIGHT_Side/AI_DebugInfo").GetComponent<Text>();
		}

		myText.text = "";
		Dictionary<Team, float>.KeyCollection teams = diffDict.Keys;
		foreach (Team teamEnm in teams) {
			//print(team);
			float diff;
			if (diffDict.TryGetValue(teamEnm, out diff)) {
				myText.text += teamEnm + " does an action every " + diff + " seconds \n";
			}
			else {
				myText.text += teamEnm  + "'s difficulty is not assinged \n";
			}
		}
	}
}
