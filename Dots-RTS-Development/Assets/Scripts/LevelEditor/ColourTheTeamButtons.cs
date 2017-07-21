using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourTheTeamButtons : MonoBehaviour {
	// Use this for initialization
	void Start () {

		int team = int.Parse(gameObject.name[6].ToString());
		gameObject.GetComponent<Image>().color = GetColorBasedOnTeam(team);
	}

	public static Color32 GetColorBasedOnTeam(int team) {
		switch (team) {
			case 0: {
				return Cell.neutralColour;
			}
			case 1: {
				return Cell.allyColour;
			}
			case 2: {
				return Cell.enemy1Colour;
			}
			case 3: {
				return Cell.enemy2Colour;
			}
			case 4: {
				return Cell.enemy3Colour;
			}
			case 5: {
				return Cell.enemy4Colour;
			}
			case 6: {
				return Cell.enemy5Colour;
			}
			case 7: {
				return Cell.enemy6Colour;
			}
			case 8: {
				return Cell.enemy7Colour;
			}
			case 9: {
				return Cell.enemy8Colour;
			}
			default: {
				return Color.white;
			}
		}
	}
}
