using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

class FolderAccess {
	public static XDocument upgradeData;
	private static Sprite NIY;

	public static FileInfo[] GetFilesFromDir(string directoryPath, string searchPattern) {
		DirectoryInfo d = new DirectoryInfo(directoryPath);
		FileInfo[] files = d.GetFiles();
		return files;
	}

	public static T GetAssociatedScript<T>(string filePath) {
		try {
			using FileStream file = new FileStream(filePath, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();

			T data = (T)bf.Deserialize(file);
			file.Close();
			return data;
		}
		catch(FileNotFoundException e) {
			Debug.Log("File " + e.FileName + " not found!");
			return default;
		}
	}

	public static Sprite GetNIYImage() {
		if (NIY == null) {
			Texture2D tex = new Texture2D(512, 512);
			tex.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "NIY.png"));
			NIY = Sprite.Create(tex, new Rect(Vector2.zero, Vector2.one * 512), Vector2.one * 0.5f);
		}
		return NIY;
	}

	public static SaveDataCampaign GetCampaignLevel(int difficulty, int level) {
		SaveDataCampaign cLevel = GetAssociatedScript<SaveDataCampaign>(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + difficulty + Path.DirectorySeparatorChar + "Level_" + level + ".pwl");
		return cLevel;
	}

	private static void RetrieveXmlUpgradeData() {
		upgradeData = XDocument.Load(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeDesc.xml");
	}

	public static string GetUpgradeName(Upgrades type) {
		if (upgradeData == null) {
			RetrieveXmlUpgradeData();
		}

		IEnumerable<string> strings = upgradeData.Descendants("Upgrade")
			.Where(upgradeName => (int) upgradeName.Attribute("id") == (int) type)
			.Select(upgradeName => upgradeName.Element("name").Value);
		try {
			return strings.First();
		}
		catch (InvalidOperationException) {
			return "Missing entry for " + type;
		}
	}

	public static string GetUpgradeFunctionalName(Upgrades type) {
		if (upgradeData == null) {
			RetrieveXmlUpgradeData();
		}

		IEnumerable<string> strings = upgradeData.Descendants("Upgrade")
			.Where(upgradeName => (int) upgradeName.Attribute("id") == (int) type)
			.Select(upgradeName => upgradeName.Element("funcName").Value);
		try {
			return strings.First();
		}
		catch (InvalidOperationException) {
			return "Missing entry for " + type;
		}
	}


	public static string GetUpgradeDesc(Upgrades type) {
		if (upgradeData == null) {
			RetrieveXmlUpgradeData();
		}

		IEnumerable<string> strings = upgradeData.Descendants("Upgrade")
			.Where(upgradeName => (int) upgradeName.Attribute("id") == (int) type)
			.Select(upgradeName => upgradeName.Element("desc").Value);
		try {
			return strings.First();
		}
		catch (InvalidOperationException) {
			return "Missing entry for " + type;
		}
	}

	/// <summary>
	///  Returns name and description of an upgrade in a form of string[], [0] = Upgrade name, [1] = Upgrade description
	/// </summary>
	/// <param name="type">The upgrade to get information about</param>
	public static string[] GetUpgrade(Upgrades type) {
		if (upgradeData == null) {
			RetrieveXmlUpgradeData();
		}

		IEnumerable<XElement> data = upgradeData.Descendants("Upgrade")
			.Where(upgradeName => (int) upgradeName.Attribute("id") == (int) type).ToList();

		string[] stringData = new string[2];

		stringData[0] = data.First().Elements().ElementAt(0).Value;
		stringData[1] = data.First().Elements().ElementAt(2).Value;
		return stringData;
	}
}

