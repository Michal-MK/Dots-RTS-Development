using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhageWars.Server {
	public class FileManager {

		private const string LEVELS_PATH = "levels";

		private static string BaseLevelsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LEVELS_PATH);

		static FileManager() {
			if (!Directory.Exists(BaseLevelsPath)) {
				Directory.CreateDirectory(BaseLevelsPath);
			}
		}

		public static byte[] GetLevel(string levelName) {
			string path = Path.Combine(BaseLevelsPath, levelName);
			return File.Exists(path) ? File.ReadAllBytes(path) : default;
		}

		public static List<string> GetLevelNames() {
			return new DirectoryInfo(BaseLevelsPath).GetFiles("*.pwl").Select(s => s.Name).ToList();
		}

		public static string GetLevelAbsPath(string levelName) {
			string path = Path.Combine(BaseLevelsPath, levelName);
			return File.Exists(path) ? path : null;
		}

		public static bool StoreLevel(string name, string levelData) {
			string path = Path.Combine(BaseLevelsPath, name);
			if (File.Exists(path)) {
				return false;
			}
			File.WriteAllText(path + ".pwl", levelData);
			return true;
		}
	}
}
