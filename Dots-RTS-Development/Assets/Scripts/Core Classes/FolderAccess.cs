using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


class FolderAccess {
	public static XDocument upgradeData;

	public static FileInfo[] GetFilesFromDir(string directoryPath, string searchPattern) {
		DirectoryInfo d = new DirectoryInfo(directoryPath);
		FileInfo[] files = d.GetFiles();
		return files;
	}

	public static T GetAsociatedScript<T>(string filePath) {
		using (FileStream file = new FileStream(filePath, FileMode.Open)) {
			BinaryFormatter bf = new BinaryFormatter();

			T data = (T)bf.Deserialize(file);
			file.Close();
			return data;
		}
	}

	public static SaveDataCampaign GetCampaignLevel(int difficulty, int level) {
		return GetAsociatedScript<SaveDataCampaign>(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + difficulty + Path.DirectorySeparatorChar + "Level_" + level + ".pwl");
	}

	[System.Obsolete("Use newer method using LINQ to XML : GetUpgrade_____()", true)]
	public static string[] GetUpgradeInfo(int upgrade) {
		using (XmlReader xml = XmlReader.Create(Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeDesc.xml")) {
			while (xml.Read()) {
				if (xml.NodeType == XmlNodeType.Element) {
					if (xml.GetAttribute("id") == upgrade.ToString()) {
						using (XmlReader inner = xml.ReadSubtree()) {

							bool foundDesc = false;
							bool foundName = false;
							string[] upgradeInfo = new string[3];

							while (inner.Read()) {
								if (foundName) {
									upgradeInfo[0] = inner.Value;
								}
								if (foundDesc) {
									upgradeInfo[1] = inner.Value;
									upgradeInfo[2] = Upgrade.GetCost((Upgrade.Upgrades)upgrade).ToString();
									return upgradeInfo;
								}
								if (inner.LocalName == "name" && string.IsNullOrEmpty(upgradeInfo[0])) {
									foundName = true;
								}
								else {
									foundName = false;
								}
								if (inner.LocalName == "desc" && string.IsNullOrEmpty(upgradeInfo[1])) {
									foundDesc = true;
								}
								else {
									foundDesc = false;
								}
							}
						}
					}
				}
			}
			Debug.LogWarning("No Upgrade of type " + (Upgrade.Upgrades)upgrade + " found!");
			return null;
		}
	}

	private static void RetrieveXmlUpgradeData() {
		upgradeData = XDocument.Load(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeDesc.xml");
	}

	public static string GetUpgradeName(Upgrade.Upgrades type) {
		if (upgradeData == null) {
			RetrieveXmlUpgradeData();
		}

		IEnumerable<string> strings = from upgradeName in upgradeData.Descendants("Upgrade")
									  where (int)upgradeName.Attribute("id") == (int)type
									  select upgradeName.Element("name").Value;
		try {
			return strings.First();
		}
		catch (System.InvalidOperationException) {
			return "Missing entry for " + type;
		}
	}

	public static string GetUpgradeFunctionalName(Upgrade.Upgrades type) {
		if (upgradeData == null) {
			RetrieveXmlUpgradeData();
		}

		IEnumerable<string> strings = from upgradeName in upgradeData.Descendants("Upgrade")
									  where (int)upgradeName.Attribute("id") == (int)type
									  select upgradeName.Element("funcName").Value;
		try {
			return strings.First();
		}
		catch (System.InvalidOperationException) {
			return "Missing entry for " + type;
		}
	}


	public static string GetUpgradeDesc(Upgrade.Upgrades type) {
		if (upgradeData == null) {
			RetrieveXmlUpgradeData();
		}

		IEnumerable<string> strings = from upgradeName in upgradeData.Descendants("Upgrade")
									  where (int)upgradeName.Attribute("id") == (int)type
									  select upgradeName.Element("desc").Value;
		try {
			return strings.First();
		}
		catch (System.InvalidOperationException) {
			return "Missing entry for " + type;
		}
	}

	public static string[] GetUpgrade(Upgrade.Upgrades type) {
		if (upgradeData == null) {
			RetrieveXmlUpgradeData();
		}

		IEnumerable<XElement> data = from upgradeName in upgradeData.Descendants("Upgrade")
									 where (int)upgradeName.Attribute("id") == (int)type
									 select upgradeName;

		string[] stringData = new string[2];

		stringData[0] = data.First().Elements().ElementAt(0).Value;
		stringData[1] = data.First().Elements().ElementAt(2).Value;
		return stringData;
	}
}

