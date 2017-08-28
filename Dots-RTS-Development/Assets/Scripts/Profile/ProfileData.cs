using UnityEngine;

public class ProfileData : MonoBehaviour {
	private void Start() {
		if (ProfileManager.getCurrentProfile != null) {
			UI_ReferenceHolder.U_profileNameUpgradeStore.text = ProfileManager.getCurrentProfile.profileName;
			UI_ReferenceHolder.U_profileMoney.text = ProfileManager.getCurrentProfile.ownedCoins + " coins";
		}
		else {
			Control.DebugSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
			UnityEngine.SceneManagement.SceneManager.LoadScene("Profiles");
		}
	}
}
