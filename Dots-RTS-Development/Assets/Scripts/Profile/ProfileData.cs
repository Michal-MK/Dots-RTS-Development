using UnityEngine;
using System.Collections;

public class ProfileData : MonoBehaviour {
	private void Start() {
		if (ProfileManager.getCurrentProfile != null) {
			UI_ReferenceHolder.profileNameUpgradeStore.text = ProfileManager.getCurrentProfile.profileName;
			UI_ReferenceHolder.profileMoney.text = ProfileManager.getCurrentProfile.ownedCoins + " coins";
		}
		else {
			Control.DebugSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
			UnityEngine.SceneManagement.SceneManager.LoadScene("Profiles");
		}
	}
}
