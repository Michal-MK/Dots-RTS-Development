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


	public static T[] GetAsociatedScripts<T> (string directory, string searchPattern) {
		FileInfo[] files = GetFilesFromDir(directory, searchPattern);
		List<T> scripts = new List<T>();

		BinaryFormatter bf = new BinaryFormatter();

		for (int i = 0; i < files.Length; i++) {
			using (FileStream fs = File.Open(files[i].FullName, FileMode.Open)) {
				T data = (T)bf.Deserialize(fs);
				scripts.Add(data);
				fs.Close();
			}

		}
		return scripts.ToArray();
	}

	//public static T GetAsociatedScript<T>(string directory, string searchPattern) {
	//	FileInfo[] files = GetFilesFromDir(directory, searchPattern);

	//	BinaryFormatter bf = new BinaryFormatter();

	//	for (int i = 0; i < files.Length; i++) {
	//		using (FileStream fs = File.Open(files[i].FullName, FileMode.Open)) {
	//			T data = (T)bf.Deserialize(fs);
	//			fs.Close();
	//		}

	//	}
	//	return scripts.ToArray();
	//}

}

