using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AllAiDifficultyWriter : MonoBehaviour {

    public static Text myText;
    // Use this for initialization
    private void OnEnable() {
        myText = gameObject.GetComponent<Text>();
    }
    public static void RedoText () {
        myText.text = "";
        Dictionary<int, float>.KeyCollection keys = LevelEditorCore.aiDifficultyDict.Keys;
        List<int> sortedKeys = keys.ToList();
        sortedKeys.Sort();
        foreach(int key in sortedKeys) {
            float diff;
            LevelEditorCore.aiDifficultyDict.TryGetValue(key, out diff);
            myText.text += "Enemy " + (key - 1) + " does an action every " + diff + " seconds \n";
        }
	}
}
