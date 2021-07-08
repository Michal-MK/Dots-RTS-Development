using UnityEngine;

public static class CellColours {
	private static readonly Color32 ALLY = new Color32(0, 255, 0, 255);        //Default ally colour
	private static readonly Color32 NEUTRAL = new Color32(255, 255, 255, 255); //Default neutral colour
	private static readonly Color32 ENEMY1 = new Color32(255, 0, 0, 255);      //Default enemy1 colour
	private static readonly Color32 ENEMY2 = new Color32(80, 0, 255, 255);     //Default enemy2 colour
	private static readonly Color32 ENEMY3 = new Color32(220, 255, 0, 255);    //Default enemy3 colour
	private static readonly Color32 ENEMY4 = new Color32(120, 60, 0, 255);     //Default enemy4 colour
	private static readonly Color32 ENEMY5 = new Color32(150, 140, 0, 255);    //Default enemy5 colour
	private static readonly Color32 ENEMY6 = new Color32(255, 0, 255, 255);    //Default enemy6 colour
	private static readonly Color32 ENEMY7 = new Color32(0, 0, 0, 255);        //Default enemy7 colour
	private static readonly Color32 ENEMY8 = new Color32(255, 150, 200, 255);  //Default enemy8 colour

	public static Color32 GetColor(Team team) {
		return team switch {
			Team.Neutral => NEUTRAL,
			Team.Allied => ALLY,
			Team.Enemy1 => ENEMY1,
			Team.Enemy2 => ENEMY2,
			Team.Enemy3 => ENEMY3,
			Team.Enemy4 => ENEMY4,
			Team.Enemy5 => ENEMY5,
			Team.Enemy6 => ENEMY6,
			Team.Enemy7 => ENEMY7,
			Team.Enemy8 => ENEMY8,
			_ => Color.white
		};
	}

	public static Color32 GetContrastColor(Team team) {
		return team switch {
			Team.Neutral => Color.black,
			Team.Allied  => Color.black,
			Team.Enemy1  => Color.black,
			Team.Enemy2  => Color.white,
			Team.Enemy3  => Color.black,
			Team.Enemy4  => Color.white,
			Team.Enemy5  => Color.black,
			Team.Enemy6  => Color.black,
			Team.Enemy7  => Color.white,
			Team.Enemy8  => Color.black,
			_            => Color.black
		};
	}
}
