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

	public const int TOTAL_UPGRADES = TOTAL_DEFENSIVE_UPGRADES + TOTAL_OFFENSIVE_UPGRADES + TOTAL_UTILITY_UPGRADES;

	public enum UpgradeType {
		OFFENSIVE = 0,
		DEFENSIVE = 100,
		UTILITY = 200,
	}


	/*
	 * Upgrades enum
	 * 000-099 - Offensive Upgrades
	 * 100-199 - Defensive Upgrades
	 * 200-299 - Utility based Upgrades
	 */

	public enum Upgrades {
		NONE = -1,
		/// <summary>
		/// [NO_STACKING] - Inflicts variable amount of damage over set time.		IMPLEMENTED
		/// </summary>
		ATK_DOT,
		/// <summary>
		/// [STACKING] - Adds a chance to double element damage.		IMPLEMENTED
		/// </summary>
		ATK_CRITICAL_CHANCE,
		/// <summary>
		/// [NO_STACKING] - 100% chance to double element damage.		IMPLEMENTED
		/// </summary>
		ATK_DOUBLE_DAMAGE,
		/// <summary>
		/// Undecided - Slows cell regenaration by a factor of 1.25.		IMPLEMENTED
		/// </summary>
		ATK_SLOW_REGENERATION,

		/// <summary>
		/// [STACKING] - Adds a chance to not take damage from incoming element.		IMPLEMENTED NOT FULLY TESTED
		/// </summary>
		DEF_ELEMENT_RESIST_CHANCE = 100,
		/// <summary>
		/// [NO_STACKING] - Adds a chance to reflect element back at atacker, element changes team to that of the attacked cell.
		/// </summary>
		DEF_REFLECTION,
		/// <summary>
		/// [NO_STACKING]. [TEMPORARY] - Removes this cell from possible targets of enemy AI, lasts for set amount of time.		IMPLEMENTED NOT FULLY TESTED
		/// </summary>
		DEF_CAMOUFLAGE,
		/// <summary>
		/// [NO_STACKING] - incoming elements of the same team have a chance to contain one extra element.		IMPLEMENTED NOT FULLY TESTED
		/// </summary>
		DEF_AID_BONUS_CHANCE,


		/// <summary>
		/// [STACKING] - Increases the speed of elements.
		/// </summary>
		UTIL_FASTER_ELEMENT_SPEED = 200,
		/// <summary>
		/// [STACKING] - Increases cell regeneration rate.		IMPLEMENETED NOT FULLY TESTED IMBALANCED
		/// </summary>
		UTIL_FASTER_REGENERATION,

	}

	public static Dictionary<Upgrades, int> UPGRADE_COST = new Dictionary<Upgrades, int>() {
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

	public static Dictionary<Upgrades, Sprite> UPGRADE_GRAPHICS = new Dictionary<Upgrades, Sprite>() {
		{Upgrades.NONE, null } ,
		{Upgrades.ATK_DOT, null },
		{Upgrades.ATK_CRITICAL_CHANCE, null },
		{Upgrades.ATK_DOUBLE_DAMAGE, null },
		{Upgrades.ATK_SLOW_REGENERATION, null },

		//{Upgrades.NONE_DEFENCE, null },
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
				//print(values[i]);
				t[i] = GetSprite((Upgrades)values[i]);
			}
		}

		Sprite[] sprites = await Task.WhenAll(t);

		for (int i = 0; i < sprites.Length; i++) {
			UPGRADE_GRAPHICS[(Upgrades)values[i]] = sprites[i];
		}
	}

	public static int GetCost(Upgrades upgrade) {
		foreach (KeyValuePair<Upgrades, int> kvp in UPGRADE_COST) {
			if (kvp.Key == upgrade) {
				return kvp.Value;
			}
		}
		Debug.LogWarning("No Upgrade found! Dictionary may be incomplete.");
		return -1;
	}



}