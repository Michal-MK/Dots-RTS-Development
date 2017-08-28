using UnityEngine;
using UnityEngine.SceneManagement;

public class Global {




	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Start() {
		Debug.Log("Test");
		if (ProfileManager.getCurrentProfile == null) {
			SceneManager.LoadScene(Scenes.PROFILES);
		}
	}

	void Update() {

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
	}
}
