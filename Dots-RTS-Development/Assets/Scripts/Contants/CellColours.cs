using UnityEngine;

public static class CellColours {
	private static readonly Color32 AllyColour = new Color32(0, 255, 0, 255);        //Default ally colour
	private static readonly Color32 NeutralColour = new Color32(255, 255, 255, 255); //Default neutral colour
	private static readonly Color32 Enemy1Colour = new Color32(255, 0, 0, 255);      //Default enemy1 colour
	private static readonly Color32 Enemy2Colour = new Color32(80, 0, 255, 255);     //Default enemy2 colour
	private static readonly Color32 Enemy3Colour = new Color32(220, 255, 0, 255);    //Default enemy3 colour
	private static readonly Color32 Enemy4Colour = new Color32(120, 60, 0, 255);     //Default enemy4 colour
	private static readonly Color32 Enemy5Colour = new Color32(150, 140, 0, 255);    //Default enemy5 colour
	private static readonly Color32 Enemy6Colour = new Color32(255, 0, 255, 255);    //Default enemy6 colour
	private static readonly Color32 Enemy7Colour = new Color32(0, 0, 0, 255);        //Default enemy7 colour
	private static readonly Color32 Enemy8Colour = new Color32(255, 150, 200, 255);  //Default enemy8 colour

	public static Color32 GetColor(Team team) {
		return team switch {
			Team.NEUTRAL => NeutralColour,
			Team.ALLIED => AllyColour,
			Team.ENEMY1 => Enemy1Colour,
			Team.ENEMY2 => Enemy2Colour,
			Team.ENEMY3 => Enemy3Colour,
			Team.ENEMY4 => Enemy4Colour,
			Team.ENEMY5 => Enemy5Colour,
			Team.ENEMY6 => Enemy6Colour,
			Team.ENEMY7 => Enemy7Colour,
			Team.ENEMY8 => Enemy8Colour,
			_ => Color.white
		};
	}

	public static Color32 GetContrastColor(Team team) {
		return team switch {
			Team.NEUTRAL => Color.black,
			Team.ALLIED => Color.black,
			Team.ENEMY1 => Color.black,
			Team.ENEMY2 => Color.white,
			Team.ENEMY3 => Color.black,
			Team.ENEMY4 => Color.white,
			Team.ENEMY5 => Color.black,
			Team.ENEMY6 => Color.black,
			Team.ENEMY7 => Color.black,
			Team.ENEMY8 => Color.black,
			_ => Color.black
		};
	}
}
