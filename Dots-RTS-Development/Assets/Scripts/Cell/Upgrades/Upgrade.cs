using System.Linq;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour {

	public Upgrades upgrade;
	public const int TOTAL_UPGRADES = 4;

	public enum Upgrades {
		NONE = -1,
		DOT,
		CRITICAL_CHANCE,
		DOUBLE_DAMAGE,
		SLOW_REGENERATION,
	}

	public static Dictionary<Upgrades, int> UPGRADE_COST = new Dictionary<Upgrades, int>() {
		{Upgrades.NONE, 0 },
		{Upgrades.DOT, 4 },
		{Upgrades.CRITICAL_CHANCE, 15 },
		{Upgrades.DOUBLE_DAMAGE, 8 },
		{Upgrades.SLOW_REGENERATION, 11 },

	};

	public static Dictionary<Upgrades, Sprite> UPGRADE_GRAPHICS = new Dictionary<Upgrades, Sprite>() {
		{Upgrades.NONE, null } ,
		{Upgrades.DOT, null },
		{Upgrades.CRITICAL_CHANCE, null },
		{Upgrades.DOUBLE_DAMAGE, null },
		{Upgrades.SLOW_REGENERATION, null },

	};

	public static async Task<Sprite> GetSprite(Upgrades type) {
		using (FileStream fs = new FileStream(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeImages" + Path.DirectorySeparatorChar + FolderAccess.GetUpgradeFunctionalName(type) + ".png", FileMode.Open, FileAccess.Read, FileShare.Read, 4069, true)) {
			//print("Got Image for " + type + " on path " + Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeImages" + Path.DirectorySeparatorChar + FolderAccess.GetUpgradeFunctionalName(type) + ".png");
			byte[] result = new byte[fs.Length];
			await fs.ReadAsync(result, 0, (int)fs.Length);
			Texture2D tex = new Texture2D(1024, 1024);
			tex.LoadImage(result);
			return Sprite.Create(tex, new Rect(Vector2.zero, Vector2.one * 1024), Vector2.one * 0.5f);
		}
	}

	public static async Task FillUpgradeSpriteDict() {

		Task<Sprite>[] t = new Task<Sprite>[TOTAL_UPGRADES];

		for (int i = 0; i < UPGRADE_GRAPHICS.Count; i++) {
			if (i < TOTAL_UPGRADES) {
				//print((Upgrades)i);
				t[i] = GetSprite((Upgrades)i);
			}
		}

		Sprite[] sprites = await Task.WhenAll(t);

		for (int i = 0; i < sprites.Length; i++) {
			UPGRADE_GRAPHICS[(Upgrades)i] = sprites[i];
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