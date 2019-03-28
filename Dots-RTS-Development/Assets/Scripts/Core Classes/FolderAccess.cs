using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

class FolderAccess {
	public static XDocument upgradeData;
	private static Sprite NIY;

	public static FileInfo[] GetFilesFromDir(string directoryPath, string searchPattern) {
		DirectoryInfo d = new DirectoryInfo(directoryPath);
		FileInfo[] files = d.GetFiles();
		return files;
	}

	public static T GetAsociatedScript<T>(string filePath) {
		try {
			using (FileStream file = new FileStream(filePath, FileMode.Open)) {
				BinaryFormatter bf = new BinaryFormatter();

				T data = (T)bf.Deserialize(file);
				file.Close();
				return data;
			}
		}
		catch(FileNotFoundException e) {
			Debug.Log("File " + e.FileName + " not found!");
			return default(T);
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
		SaveDataCampaign cLevel = GetAsociatedScript<SaveDataCampaign>(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + difficulty + Path.DirectorySeparatorChar + "Level_" + level + ".pwl");
		if(cLevel == default(SaveDataCampaign)) {
			return null;
		}
		return cLevel;
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
									upgradeInfo[2] = Upgrade.GetCost((Upgrades)upgrade).ToString();
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
			Debug.LogWarning("No Upgrade of type " + (Upgrades)upgrade + " found!");
			return null;
		}
	}

	private static void RetrieveXmlUpgradeData() {
		upgradeData = XDocument.Load(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeDesc.xml");
	}

	public static string GetUpgradeName(Upgrades type) {
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

	public static string GetUpgradeFunctionalName(Upgrades type) {
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


	public static string GetUpgradeDesc(Upgrades type) {
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

	/// <summary>
	///  Returns name and description of an upgrade in a form of string[], [0] = Upgrade name, [1] = Upgrade description
	/// </summary>
	/// <param name="type">The upgrade to get information about</param>
	public static string[] GetUpgrade(Upgrades type) {
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

