using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourTheTeamButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (gameObject.name == "Team (0)") {
			gameObject.GetComponent<Image>().color = Cell.neutral;
		}
		else if (gameObject.name == "Team (1)") {
			gameObject.GetComponent<Image>().color = Cell.ally;
		}
		else if (gameObject.name == "Team (2)") {
			gameObject.GetComponent<Image>().color = Cell.enemy1;
		}
		else if (gameObject.name == "Team (3)") {
			gameObject.GetComponent<Image>().color = Cell.enemy2;
		}
		else if (gameObject.name == "Team (4)") {
			gameObject.GetComponent<Image>().color = Cell.enemy3;
		}
		else if (gameObject.name == "Team (5)")	 {
			gameObject.GetComponent<Image>().color = Cell.enemy4;
		}
		else if (gameObject.name == "Team (6)") {
			gameObject.GetComponent<Image>().color = Cell.enemy5;
		}
		else if (gameObject.name == "Team (7)") {
			gameObject.GetComponent<Image>().color = Cell.enemy6;
		}
		else if (gameObject.name == "Team (8)") {
			gameObject.GetComponent<Image>().color = Cell.enemy7;
		}
		else if (gameObject.name == "Team (9)") {
			gameObject.GetComponent<Image>().color = Cell.enemy8;
		}
	}
}
