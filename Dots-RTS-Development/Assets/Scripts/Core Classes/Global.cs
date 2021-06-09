using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class Global {
	public static bool baseLoaded;
	public static readonly Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Start() {

		if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves");
		}
		if (!Directory.Exists(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves")) {
			Directory.CreateDirectory(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves");
		}
		if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles")) {
			Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles");
		}

		List<Task> tasks = new List<Task> { Upgrade.FillUpgradeSpriteDict() };

		try {
			_ = Task.Run(async () => {
				await Task.WhenAll(tasks);
				baseLoaded = true;
			});
		}
		catch {
			// TODO Log
			Application.Quit(1);
		}
	}
}

namespace UnityEngine.SceneManagement {
	public static class Scenes {
		public const string PROFILES = "Profiles";
		public const string MENU = "Main_Menu";
		public const string LEVEL_EDITOR = "Level_Editor";
		public const string GAME = "Level_Player";
		public const string LEVEL_SELECT = "Level_Select";
		public const string UPGRADE_SHOP = "Upgrade_Store";
		public const string LEVEL_SHARE = "Level_Sharing";
		public const string DEBUG = "Debug";
		public const string POST_GAME = "Post_Game";
		public const string SPLASH = "SplashScreen";
	}
}
