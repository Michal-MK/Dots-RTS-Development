using UnityEngine;
using System.Collections;

public class ProfileData : MonoBehaviour {
	private void Start() {
		UI_ReferenceHolder.profileNameUpgradeStore.text = ProfileManager.getCurrentProfile.profileName;
		UI_ReferenceHolder.profileMoney.text = ProfileManager.getCurrentProfile.ownedCoins + " coins";
	}
}
