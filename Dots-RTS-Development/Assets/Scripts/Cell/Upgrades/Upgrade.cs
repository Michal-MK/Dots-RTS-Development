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
		{Upgrades.None, 0 },
		{Upgrades.AtkDot, 4 },
		{Upgrades.AtkCriticalChance, 6 },
		{Upgrades.AtkDoubleDamage, 8 },
		{Upgrades.AtkSlowRegeneration, 10 },

		{Upgrades.DefElementResistChance, 5 },
		{Upgrades.DefReflection, 12 },
		{Upgrades.UtilFasterRegeneration, 8 },
		{Upgrades.DefCamouflage, 6 },
		{Upgrades.DefAidBonusChance, 10 },

		{Upgrades.UtilFasterElementSpeed, 4 },

	};

	public static readonly Dictionary<Upgrades, Sprite> UpgradeGraphics = new Dictionary<Upgrades, Sprite>() {
		{Upgrades.None, null } ,
		{Upgrades.AtkDot, null },
		{Upgrades.AtkCriticalChance, null },
		{Upgrades.AtkDoubleDamage, null },
		{Upgrades.AtkSlowRegeneration, null },

		{Upgrades.DefElementResistChance,null },
		{Upgrades.DefReflection, null},
		{Upgrades.UtilFasterRegeneration, null },
		{Upgrades.DefCamouflage,null },
		{Upgrades.DefAidBonusChance,null },

		{Upgrades.UtilFasterElementSpeed,null },
	};

	public static async Task<Sprite> GetSprite(Upgrades type) {
		try {
			using FileStream fs = new FileStream(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeImages" + Path.DirectorySeparatorChar + FolderAccess.GetUpgradeFunctionalName(type) + ".png", FileMode.Open, FileAccess.Read, FileShare.Read, 4069, true);
			byte[] result = new byte[fs.Length];
			await fs.ReadAsync(result, 0, (int)fs.Length);
			Texture2D tex = new Texture2D(1024, 1024);
			tex.LoadImage(result);
			return Sprite.Create(tex, new Rect(Vector2.zero, Vector2.one * 1024), Vector2.one * 0.5f);
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
