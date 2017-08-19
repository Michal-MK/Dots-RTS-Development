using System.Linq;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Upgrade : MonoBehaviour {

	public Upgrades upgrade;

	public enum Upgrades {
		NONE,
		DOT,
		DOUBLE_DAMAGE,
		SLOW_REGENERATION,
		CRITICAL_CHANCE,
	}

	public static Dictionary<Upgrades, int> UPGRADE_COST = new Dictionary<Upgrades, int>() {
		{Upgrades.NONE , 0 },
		{Upgrades.CRITICAL_CHANCE, 15 },
		{Upgrades.DOUBLE_DAMAGE, 8 },
		{Upgrades.SLOW_REGENERATION, 11 },
		{Upgrades.DOT, 4 },

	};

	public static Dictionary<Upgrades, Sprite> UPGRADE_GRAPHICS = new Dictionary<Upgrades, Sprite>() {
		{Upgrades.NONE, null },
		{Upgrades.CRITICAL_CHANCE, null },
		{Upgrades.DOT, null },
		{Upgrades.DOUBLE_DAMAGE, null },
		{Upgrades.SLOW_REGENERATION, null },

	};

	public static async Task<Sprite> GetSprite(Upgrades type) {
		using (FileStream fs = new FileStream(Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeImages" + Path.DirectorySeparatorChar + FolderAccess.GetUpgradeFunctionalName(type) + ".png", FileMode.Open, FileAccess.Read, FileShare.Read, 4069, true)) {
			byte[] result = new byte[fs.Length];
			await fs.ReadAsync(result, 0, (int)fs.Length);
			Texture2D tex = new Texture2D(1024, 1024);
			tex.LoadImage(result);
			return Sprite.Create(tex, new Rect(Vector2.zero, Vector2.one * 1024), Vector2.one * 0.5f);
		}
	}

	public static async Task FillUpgradeSpriteDict() {
		List<Task<Sprite>> t = new List<Task<Sprite>>();
		foreach (KeyValuePair<Upgrades,Sprite> dictItem in UPGRADE_GRAPHICS) {
			if (dictItem.Key != Upgrades.NONE) {
				t.Add(GetSprite(dictItem.Key));
			}
		}

		Sprite[] sprites = await Task.WhenAll(t);

		for (int i = 1; i < UPGRADE_GRAPHICS.Count - 1; i++) {
			UPGRADE_GRAPHICS[UPGRADE_GRAPHICS.ElementAt(i).Key] = sprites[i]; 
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