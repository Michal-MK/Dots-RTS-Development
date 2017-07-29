using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
}

