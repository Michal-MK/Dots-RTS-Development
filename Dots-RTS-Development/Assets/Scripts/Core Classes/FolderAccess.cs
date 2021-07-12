using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class FolderAccess {
	private static Sprite niy;

	public static Sprite GetNIYImage() {
		if (niy != null) return niy;
		Texture2D tex = new Texture2D(512, 512);
		tex.LoadImage(File.ReadAllBytes(Paths.StreamedResource("NIY.png")));
		niy = Sprite.Create(tex, new Rect(Vector2.zero, Vector2.one * 512), Vector2.one * 0.5f);
		return niy;
	}

	private static UpgradeData[] RetrieveUpgradeData() {
		return JsonUtility.FromJson<UpgradeData[]>(Paths.StreamedResource("UpgradeDesc.json"));
	}

	public static string GetUpgradeName(Upgrades type) {
		UpgradeData[] data = RetrieveUpgradeData();

		IEnumerable<string> names = data.Where(w => w.ID == (int)type)
										.Select(s => s.Name);
		try {
			return names.First();
		}
		catch (InvalidOperationException) {
			return "Missing entry for " + type;
		}
	}

	public static string GetUpgradeFunctionalName(Upgrades type) {
		UpgradeData[] data = RetrieveUpgradeData();

		IEnumerable<string> funcNames = data.Where(w => w.ID == (int)type)
											.Select(s => s.FunctionName);
		try {
			return funcNames.First();
		}
		catch (InvalidOperationException) {
			return "Missing entry for " + type;
		}
	}

	/// <summary>
	///  Returns name and description of an upgrade in a form of string[], [0] = Upgrade name, [1] = Upgrade description
	/// </summary>
	/// <param name="type">The upgrade to get information about</param>
	public static UpgradeData GetUpgrade(Upgrades type) {
		return RetrieveUpgradeData()
			.First(w => w.ID == (int)type);
	}
}