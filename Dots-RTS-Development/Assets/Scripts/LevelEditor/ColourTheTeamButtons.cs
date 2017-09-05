using UnityEngine;
using UnityEngine.UI;

public class ColourTheTeamButtons : MonoBehaviour {
	// Use this for initialization
	void Start() {

		int team = int.Parse(gameObject.name[6].ToString());
		gameObject.GetComponent<Image>().color = GetColorBasedOnTeam((Cell.enmTeam)team);
	}

	public static Color32 GetColorBasedOnTeam(Cell.enmTeam team) {

		switch (team) {
			case Cell.enmTeam.NEUTRAL: {
					return Cell.neutralColour;
				}
			case Cell.enmTeam.ALLIED: {
					return Cell.allyColour;
				}
			case Cell.enmTeam.ENEMY1: {
					return Cell.enemy1Colour;
				}
			case Cell.enmTeam.ENEMY2: {
					return Cell.enemy2Colour;
				}
			case Cell.enmTeam.ENEMY3: {
					return Cell.enemy3Colour;
				}
			case Cell.enmTeam.ENEMY4: {
					return Cell.enemy4Colour;
				}
			case Cell.enmTeam.ENEMY5: {
					return Cell.enemy5Colour;
				}
			case Cell.enmTeam.ENEMY6: {
					return Cell.enemy6Colour;
				}
			case Cell.enmTeam.ENEMY7: {
					return Cell.enemy7Colour;
				}
			case Cell.enmTeam.ENEMY8: {
					return Cell.enemy8Colour;
				}
			default: {
					return Color.white;
				}
		}
	}
	public static string GetDescriptionBasedOnTeam(Cell.enmTeam team) {

		switch (team) {
			case Cell.enmTeam.NEUTRAL: {
					return "Neutral";
				}
			case Cell.enmTeam.ALLIED: {
					return "Ally";
				}
			//case Cell.enmTeam.ENEMY1: {
			//		return Cell.enemy1Colour;
			//	}
			//case Cell.enmTeam.ENEMY2: {
			//		return Cell.enemy2Colour;
			//	}
			//case Cell.enmTeam.ENEMY3: {
			//		return Cell.enemy3Colour;
			//	}
			//case Cell.enmTeam.ENEMY4: {
			//		return Cell.enemy4Colour;
			//	}
			//case Cell.enmTeam.ENEMY5: {
			//		return Cell.enemy5Colour;
			//	}
			//case Cell.enmTeam.ENEMY6: {
			//		return Cell.enemy6Colour;
			//	}
			//case Cell.enmTeam.ENEMY7: {
			//		return Cell.enemy7Colour;
			//	}
			//case Cell.enmTeam.ENEMY8: {
			//		return Cell.enemy8Colour;
			//	}
			default: {
					return "Enemy";
				}
		}
	}
	public static Color32 GetContrastColorBasedOnTeam(Cell.enmTeam team) {

		switch (team) {
			case Cell.enmTeam.NEUTRAL: {
					return Color.black;
				}
			case Cell.enmTeam.ALLIED: {
					return Color.black;
				}
			case Cell.enmTeam.ENEMY1: {
					return Color.black;
				}
			case Cell.enmTeam.ENEMY2: {
					return Color.white;
				}
			case Cell.enmTeam.ENEMY3: {
					return Color.black;
				}
			case Cell.enmTeam.ENEMY4: {
					return Color.white;
				}
			case Cell.enmTeam.ENEMY5: {
					return Color.black;
				}
			case Cell.enmTeam.ENEMY6: {
					return Color.black;
				}
			case Cell.enmTeam.ENEMY7: {
					return Color.black;
				}
			case Cell.enmTeam.ENEMY8: {
					return Color.black;
				}
			default: {
					return Color.black;
				}
		}
	}
}
