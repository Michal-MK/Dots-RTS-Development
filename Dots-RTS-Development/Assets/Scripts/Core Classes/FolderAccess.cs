using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;


class FolderAccess {

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

	public static SaveFileInfo GetCampaignLevel(int difficulty, int level) {
		return GetAsociatedScript<SaveFileInfo>(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Campaign" + Path.DirectorySeparatorChar + "Difficulty" + difficulty + Path.DirectorySeparatorChar + level + ".pwl");
	}

	public static string[] GetUpgradeInfo(int upgrade) {
		using (XmlReader xml = XmlReader.Create(Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "UpgradeDesc.xml")) {
			while (xml.Read()) {
				if (xml.NodeType == XmlNodeType.Element) {
					if (xml.GetAttribute("id") == upgrade.ToString()) {
						XmlReader inner = xml.ReadSubtree();
						bool foundDesc = false;
						bool foundName = false;
						bool foundCost = false;
						string[] upgradeInfo = new string[3];

						while (inner.Read()) {
							if (foundName) {
								upgradeInfo[0] = inner.Value;
							}
							if (foundDesc) {
								upgradeInfo[1] = inner.Value;
							}
							if (foundCost) {
								upgradeInfo[2] = inner.Value;
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
							if (inner.LocalName == "cost" && string.IsNullOrEmpty(upgradeInfo[2])) {
								foundCost = true;
							}
							else {
								foundCost = false;
							}
						}
						inner.Close();
						inner.Dispose();
						xml.Close();
						xml.Dispose();
						break;
					}
				}
			}
			throw new System.Exception();
		}
	}
}

