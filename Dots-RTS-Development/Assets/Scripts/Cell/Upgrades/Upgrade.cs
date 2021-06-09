using System.Linq;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;

public class Upgrade : MonoBehaviour {

	public const int TOTAL_OFFENSIVE_UPGRADES = 4;
	public const int TOTAL_DEFENSIVE_UPGRADES = 4;
	public const int TOTAL_UTILITY_UPGRADES = 2;

	private const int TOTAL_UPGRADES = TOTAL_DEFENSIVE_UPGRADES + TOTAL_OFFENSIVE_UPGRADES + TOTAL_UTILITY_UPGRADES;


	/*
	 * Upgrades enum
	 * 000-099 - Offensive Upgrades
	 * 100-199 - Defensive Upgrades
	 * 200-299 - Utility based Upgrades
	 */


	private static readonly Dictionary<Upgrades, int> UpgradeCosts = new Dictionary<Upgrades, int>() {
		{Upgrades.NONE, 0 },
		{Upgrades.ATK_DOT, 4 },
		{Upgrades.ATK_CRITICAL_CHANCE, 6 },
		{Upgrades.ATK_DOUBLE_DAMAGE, 8 },
		{Upgrades.ATK_SLOW_REGENERATION, 10 },

		{Upgrades.DEF_ELEMENT_RESIST_CHANCE, 5 },
		{Upgrades.DEF_REFLECTION, 12 },
		{Upgrades.UTIL_FASTER_REGENERATION, 8 },
		{Upgrades.DEF_CAMOUFLAGE, 6 },
		{Upgrades.DEF_AID_BONUS_CHANCE, 10 },

		{Upgrades.UTIL_FASTER_ELEMENT_SPEED, 4 },

	};

	public static readonly Dictionary<Upgrades, Sprite> UpgradeGraphics = new Dictionary<Upgrades, Sprite>() {
		{Upgrades.NONE, null } ,
		{Upgrades.ATK_DOT, null },
		{Upgrades.ATK_CRITICAL_CHANCE, null },
		{Upgrades.ATK_DOUBLE_DAMAGE, null },
		{Upgrades.ATK_SLOW_REGENERATION, null },

		{Upgrades.DEF_ELEMENT_RESIST_CHANCE,null },
		{Upgrades.DEF_REFLECTION, null},
		{Upgrades.UTIL_FASTER_REGENERATION, null },
		{Upgrades.DEF_CAMOUFLAGE,null },
		{Upgrades.DEF_AID_BONUS_CHANCE,null },

		{Upgrades.UTIL_FASTER_ELEMENT_SPEED,null },
	};

	public static async Task<Sprite> GetSprite(Upgrades type) {
		try {
			using (FileStream fs = new FileStream(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeImages" + Path.DirectorySeparatorChar + FolderAccess.GetUpgradeFunctionalName(type) + ".png", FileMode.Open, FileAccess.Read, FileShare.Read, 4069, true)) {
				byte[] result = new byte[fs.Length];
				await fs.ReadAsync(result, 0, (int)fs.Length);
				Texture2D tex = new Texture2D(1024, 1024);
				tex.LoadImage(result);
				return Sprite.Create(tex, new Rect(Vector2.zero, Vector2.one * 1024), Vector2.one * 0.5f);
			}
		}
		catch (FileNotFoundException) {
			return FolderAccess.GetNIYImage();
		}
	}

	public static async Task FillUpgradeSpriteDict() {

		Task<Sprite>[] t = new Task<Sprite>[TOTAL_UPGRADES];

		int[] values = (int[])Enum.GetValues(typeof(Upgrades));

		for (int i = 0; i < values.Length; i++) {
			if(values[i] != -1) {
				t[i] = GetSprite((Upgrades)values[i]);
			}
		}

		Sprite[] sprites = await Task.WhenAll(t);

		for (int i = 0; i < sprites.Length; i++) {
			UpgradeGraphics[(Upgrades)values[i]] = sprites[i];
		}
	}

	public static int GetCost(Upgrades upgrade) {
		foreach (KeyValuePair<Upgrades, int> kvp in UpgradeCosts) {
			if (kvp.Key == upgrade) {
				return kvp.Value;
			}
		}
		Debug.LogWarning("No Upgrade found! Dictionary may be incomplete.");
		return -1;
	}
}