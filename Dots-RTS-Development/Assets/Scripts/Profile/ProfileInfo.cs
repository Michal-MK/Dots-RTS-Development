using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileInfo : MonoBehaviour {

	[HideInInspector]
	public Profile selected;

	public static event ProfileManager.profileDeleted OnProfileDeleted;

	#region Prefab References
	public Text profileName;
	public RawImage careerLevel;
	public GameObject self;
	#endregion

	public void LoadProfile() {
		if (Input.GetKey(KeyCode.A)){;
			foreach (Upgrade.Upgrades u in Enum.GetValues(typeof(Upgrade.Upgrades))) {
				if (u != Upgrade.Upgrades.NONE) {
					ProfileManager.getCurrentProfile.acquiredUpgrades[u] = 8;
				}
			}
			ProfileManager.SerializeChanges();
		}
		if (Control.DebugSceneIndex != 0 && Control.DebugSceneIndex != 1) {
			SceneManager.LoadScene(Control.DebugSceneIndex);
		}
		else {
			SceneManager.LoadScene(Scenes.MENU);
		}
	}

	public void InitializeProfile(Profile p, string profileName) {

		selected = p;
		this.profileName.text = profileName;
		Texture2D tex = new Texture2D(160, 90);
		SaveDataCampaign campaignLevel = FolderAccess.GetCampaignLevel(p.onLevelBaseGame.difficulty, p.onLevelBaseGame.level);
		tex.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + campaignLevel.preview));
		careerLevel.texture = tex;
	}

	public void ShowProfileInfo() {
		Profile p = ProfileManager.setCurrentProfile = selected;

		SaveDataCampaign campaignLevel = FolderAccess.GetCampaignLevel(p.onLevelBaseGame.difficulty, p.onLevelBaseGame.level);

		if(campaignLevel != null) {
			Texture2D tex = new Texture2D(160, 90);
			try {
				tex.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + campaignLevel.preview));
			}
			catch (FileNotFoundException e) {
				print("File Not Found " + e.FileName + "--> No Level texture will be shown.");
			}
			UI_ReferenceHolder.PO_OnLevel.text = campaignLevel.game.levelInfo.levelName;
			UI_ReferenceHolder.PO_OnLevelImage.texture = tex;
		}
		else {
			print("Loaded level is " + campaignLevel + "!");
			UI_ReferenceHolder.PO_OnLevel.text = "No level found!";

		}

		UI_ReferenceHolder.PO_Name.text = p.profileName;
		UI_ReferenceHolder.PO_CurrentCoins.text = "Coins : " + p.ownedCoins;
		UI_ReferenceHolder.PO_GamesPlayed.text = "Custom : " + p.completedCustomLevels + "\nCampaign : " + p.completedCampaignLevels;
		UI_ReferenceHolder.PO_DeleteProfile.self = this.self;
		UI_ReferenceHolder.PO_Canvas.SetActive(true);
		UI_ReferenceHolder.PS_Canvas.SetActive(false);
	}

	public void HideProfileInfo() {
		UI_ReferenceHolder.PO_Canvas.SetActive(false);
		UI_ReferenceHolder.PS_Canvas.SetActive(true);
	}


	public void DeleteProfile() {
		File.Delete(self.name);
		HideProfileInfo();
		Destroy(self);

		if (OnProfileDeleted != null) {
			OnProfileDeleted(this);
		}
	}
}
