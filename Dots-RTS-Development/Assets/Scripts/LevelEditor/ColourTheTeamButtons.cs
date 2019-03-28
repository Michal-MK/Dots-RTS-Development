using UnityEngine;
using UnityEngine.UI;

public class ColourTheTeamButtons : MonoBehaviour {
	// Use this for initialization
	void Start() {

		int team = int.Parse(gameObject.name[6].ToString());
		gameObject.GetComponent<Image>().color = GetColorBasedOnTeam((Team)team);
	}

	public static Color32 GetColorBasedOnTeam(Team team) {

		switch (team) {
			case Team.NEUTRAL: {
					return CellColours.NeutralColour;
				}
			case Team.ALLIED: {
					return CellColours.AllyColour;
				}
			case Team.ENEMY1: {
					return CellColours.Enemy1Colour;
				}
			case Team.ENEMY2: {
					return CellColours.Enemy2Colour;
				}
			case Team.ENEMY3: {
					return CellColours.Enemy3Colour;
				}
			case Team.ENEMY4: {
					return CellColours.Enemy4Colour;
				}
			case Team.ENEMY5: {
					return CellColours.Enemy5Colour;
				}
			case Team.ENEMY6: {
					return CellColours.Enemy6Colour;
				}
			case Team.ENEMY7: {
					return CellColours.Enemy7Colour;
				}
			case Team.ENEMY8: {
					return CellColours.Enemy8Colour;
				}
			default: {
					return Color.white;
				}
		}
	}
	public static string GetDescriptionBasedOnTeam(Team team) {

		switch (team) {
			case Team.NEUTRAL: {
					return "Neutral";
				}
			case Team.ALLIED: {
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
	public static Color32 GetContrastColorBasedOnTeam(Team team) {

		switch (team) {
			case Team.NEUTRAL: {
					return Color.black;
				}
			case Team.ALLIED: {
					return Color.black;
				}
			case Team.ENEMY1: {
					return Color.black;
				}
			case Team.ENEMY2: {
					return Color.white;
				}
			case Team.ENEMY3: {
					return Color.black;
				}
			case Team.ENEMY4: {
					return Color.white;
				}
			case Team.ENEMY5: {
					return Color.black;
				}
			case Team.ENEMY6: {
					return Color.black;
				}
			case Team.ENEMY7: {
					return Color.black;
				}
			case Team.ENEMY8: {
					return Color.black;
				}
			default: {
					return Color.black;
				}
		}
	}
}
