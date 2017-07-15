using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourTheTeamButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (gameObject.name == "Team (0)") {
			gameObject.GetComponent<Image>().color = Cell.neutralColour;
		}
		else if (gameObject.name == "Team (1)") {
			gameObject.GetComponent<Image>().color = Cell.allyColour;
		}
		else if (gameObject.name == "Team (2)") {
			gameObject.GetComponent<Image>().color = Cell.enemy1Colour;
		}
		else if (gameObject.name == "Team (3)") {
			gameObject.GetComponent<Image>().color = Cell.enemy2Colour;
		}
		else if (gameObject.name == "Team (4)") {
			gameObject.GetComponent<Image>().color = Cell.enemy3Colour;
		}
		else if (gameObject.name == "Team (5)")	 {
			gameObject.GetComponent<Image>().color = Cell.enemy4Colour;
		}
		else if (gameObject.name == "Team (6)") {
			gameObject.GetComponent<Image>().color = Cell.enemy5Colour;
		}
		else if (gameObject.name == "Team (7)") {
			gameObject.GetComponent<Image>().color = Cell.enemy6Colour;
		}
		else if (gameObject.name == "Team (8)") {
			gameObject.GetComponent<Image>().color = Cell.enemy7Colour;
		}
		else if (gameObject.name == "Team (9)") {
			gameObject.GetComponent<Image>().color = Cell.enemy8Colour;
		}
		Debug.LogWarning("Colour should be done through a switch case!");
	}
}
