using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global {
	public static bool baseLoaded = false;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static async void Start() {
		List<Task> tasks = new List<Task>();
		tasks.Add(Upgrade.FillUpgradeSpriteDict());

		await Task.WhenAll(tasks);

		baseLoaded = true;
	}
}

namespace UnityEngine.SceneManagement {
	class Scenes {
		public const string PROFILES = "Profiles";
		public const string MENU = "Main_Menu";
		public const string EDITOR = "Level_Editor";
		public const string PLAYER = "Level_Player";
		public const string SELECT = "Level_Select";
		public const string SHOP = "Upgrade_Store";
		public const string SHARING = "Level_Sharing";
		public const string DEBUG = "Debug";
		public const string POSTG = "Post_Game";
		public const string SPLASH = "SplashScreen";
	}
}
