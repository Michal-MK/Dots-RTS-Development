using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhageWars.Server {
	public class FileManager {

		const string LEVELS_PATH = "levels";

		private static string BaseLevelsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LEVELS_PATH);

		static FileManager() {
			if (!Directory.Exists(BaseLevelsPath)) {
				Directory.CreateDirectory(BaseLevelsPath);
			}
		}

		public static byte[] GetLevel(string levelName) {
			string path = Path.Combine(BaseLevelsPath, levelName);
			if (File.Exists(path)) {
				return File.ReadAllBytes(path);
			}
			return default;
		}

		public static string GetLevelAbsPath(string levelName) {
			string path = Path.Combine(BaseLevelsPath, levelName);
			if (File.Exists(path)) {
				return path;
			}
			return null;
		}
	}
}
