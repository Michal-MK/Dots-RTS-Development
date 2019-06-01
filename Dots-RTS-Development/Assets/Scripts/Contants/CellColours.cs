using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

class CellColours {
	public static readonly Color32 AllyColour = new Color32(0, 255, 0, 255);        //Default ally colour
	public static readonly Color32 NeutralColour = new Color32(255, 255, 255, 255); //Default neutral colour
	public static readonly Color32 Enemy1Colour = new Color32(255, 0, 0, 255);      //Default enemy1 colour
	public static readonly Color32 Enemy2Colour = new Color32(80, 0, 255, 255);     //Default enemy2 colour
	public static readonly Color32 Enemy3Colour = new Color32(220, 255, 0, 255);    //Default enemy3 colour
	public static readonly Color32 Enemy4Colour = new Color32(120, 60, 0, 255);     //Default enemy4 colour
	public static readonly Color32 Enemy5Colour = new Color32(150, 140, 0, 255);    //Default enemy5 colour
	public static readonly Color32 Enemy6Colour = new Color32(255, 0, 255, 255);    //Default enemy6 colour
	public static readonly Color32 Enemy7Colour = new Color32(0, 0, 0, 255);        //Default enemy7 colour
	public static readonly Color32 Enemy8Colour = new Color32(255, 150, 200, 255);  //Default enemy8 colour

	public static Color32 GetColor(Team team) {
		switch (team) {
			case Team.NEUTRAL:
				return NeutralColour;
			case Team.ALLIED:
				return AllyColour;
			case Team.ENEMY1:
				return Enemy1Colour;
			case Team.ENEMY2:
				return Enemy2Colour;
			case Team.ENEMY3:
				return Enemy3Colour;
			case Team.ENEMY4:
				return Enemy4Colour;
			case Team.ENEMY5:
				return Enemy5Colour;
			case Team.ENEMY6:
				return Enemy6Colour;
			case Team.ENEMY7:
				return Enemy7Colour;
			case Team.ENEMY8:
				return Enemy8Colour;
			default:
				Debugger.Break();
				Debug.Break();
				return Color.white;
		}
	}

	public static Color32 GetContrastColor(Team team) {
		switch (team) {
			case Team.NEUTRAL:
				return Color.black;
			case Team.ALLIED:
				return Color.black;
			case Team.ENEMY1:
				return Color.black;
			case Team.ENEMY2:
				return Color.white;
			case Team.ENEMY3:
				return Color.black;
			case Team.ENEMY4:
				return Color.white;
			case Team.ENEMY5:
				return Color.black;
			case Team.ENEMY6:
				return Color.black;
			case Team.ENEMY7:
				return Color.black;
			case Team.ENEMY8:
				return Color.black;
			default:
				return Color.black;
		}
	}
}
