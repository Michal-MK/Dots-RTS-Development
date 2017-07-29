using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class ProfileInfo : MonoBehaviour {
	[HideInInspector]
	public Profile selected;

	public Text profileName;
	public RawImage careerLevel;

	public void SelectProfile() {
		ProfileManager.currentProfile = selected;
		SceneManager.LoadScene(0);
		print(ProfileManager.currentProfile.profileName);
	}

}
