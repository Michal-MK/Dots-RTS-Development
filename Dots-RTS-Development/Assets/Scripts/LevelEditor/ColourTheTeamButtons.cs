using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourTheTeamButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
		switch (gameObject.name) {

			case "Team (0)": {
				gameObject.GetComponent<Image>().color = Cell.neutralColour;return;
			}
			case "Team (1)": {
				gameObject.GetComponent<Image>().color = Cell.allyColour;return;
			}
			case "Team (2)": {
				gameObject.GetComponent<Image>().color = Cell.enemy1Colour;return;
			}
			case "Team (3)": {
				gameObject.GetComponent<Image>().color = Cell.enemy2Colour;return;
			}
			case "Team (4)": {
				gameObject.GetComponent<Image>().color = Cell.enemy3Colour;return;
			}
			case "Team (5)": {
				gameObject.GetComponent<Image>().color = Cell.enemy4Colour;return;
			}
			case "Team (6)": {
				gameObject.GetComponent<Image>().color = Cell.enemy5Colour;return;
			}
			case "Team (7)": {
				gameObject.GetComponent<Image>().color = Cell.enemy6Colour;return;
			}
			case "Team (8)": {
				gameObject.GetComponent<Image>().color = Cell.enemy7Colour;return;
			}
			case "Team (9)": {
				gameObject.GetComponent<Image>().color = Cell.enemy8Colour;return;
			}
		}
	}
}
