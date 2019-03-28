using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileData : MonoBehaviour {
	private void Start() {
		if (ProfileManager.CurrentProfile != null) {
			UI_ReferenceHolder.U_profileNameUpgradeStore.text = ProfileManager.CurrentProfile.Name;
			UI_ReferenceHolder.U_profileMoney.text = $"{ProfileManager.CurrentProfile.Coins} coins";
		}
		else {
			Control.DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene("Profiles");
		}
	}
}
