using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileInfo : MonoBehaviour {
	[HideInInspector]
	public Profile selected;
	public static event ProfileManager.profileDeleted OnProfileDeleted;
	public Text profileName;
	public RawImage careerLevel;

	public GameObject controlsObj;

	public void SelectProfile() {
		ProfileManager.currentProfile = selected;
		SceneManager.LoadScene(Control.DebugSceneIndex);
	}

	public void InitializeProfile(Profile p, string profileName) {

		selected = p;
		this.profileName.text = profileName;
		Texture2D tex = new Texture2D(160, 90);
		tex.LoadImage(p.onLevelImage);
		careerLevel.texture = tex;
	}
	private bool controls = false;
	public void ShowControls() {
		controls = !controls;
		controlsObj.SetActive(!controlsObj.activeInHierarchy);
	}

	public void DeleteProfile() {
		File.Delete(gameObject.name);
		Destroy(gameObject);

		if (OnProfileDeleted != null) {
			OnProfileDeleted(this);
		}
	}
}
