using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileInfo : MonoBehaviour {

	[HideInInspector]
	public Profile selected;

	public static event EventHandler<ProfileInfo> OnProfileDeleted;

	#region Prefab References
	public Text profileName;
	public RawImage careerLevel;
	public GameObject self;
	#endregion

	public void LoadProfile() {
		if (Input.GetKey(KeyCode.A)){;
			foreach (Upgrades u in Enum.GetValues(typeof(Upgrades))) {
				if (u != Upgrades.NONE) {
					ProfileManager.CurrentProfile.acquiredUpgrades[u] = 8;
				}
			}
			ProfileManager.SerializeChanges();
		}
		if (Control.DebugSceneIndex > 1) {
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
		SaveDataCampaign campaignLevel = FolderAccess.GetCampaignLevel(p.CurrentCampaignLevel.difficulty, p.CurrentCampaignLevel.level);
		tex.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + campaignLevel.preview));
		careerLevel.texture = tex;
	}

	public void ShowProfileInfo() {
		ProfileManager.CurrentProfile = selected;

		SaveDataCampaign campaignLevel = FolderAccess.GetCampaignLevel(ProfileManager.CurrentProfile.CurrentCampaignLevel.difficulty, ProfileManager.CurrentProfile.CurrentCampaignLevel.level);

		if(campaignLevel != null) {
			Texture2D tex = new Texture2D(160, 90);
			try {
				tex.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + campaignLevel.preview));
			}
			catch (FileNotFoundException e) {
				print($"File {e.FileName} Not Found! --> No Level texture will be shown.");
			}
			UI_ReferenceHolder.PO_OnLevel.text = campaignLevel.game.levelInfo.levelName;
			UI_ReferenceHolder.PO_OnLevelImage.texture = tex;
		}
		else {
			print($"Loaded level is {campaignLevel}!");
			UI_ReferenceHolder.PO_OnLevel.text = "No level found!";

		}

		UI_ReferenceHolder.PO_Name.text = ProfileManager.CurrentProfile.Name;
		UI_ReferenceHolder.PO_CurrentCoins.text = $"Coins : {ProfileManager.CurrentProfile.Coins}";
		UI_ReferenceHolder.PO_GamesPlayed.text = $"Custom : {ProfileManager.CurrentProfile.CompletedCustomLevels }\nCampaign : {ProfileManager.CurrentProfile.CompletedCampaignLevels}";
		UI_ReferenceHolder.PO_DeleteProfile.self = self;
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
		OnProfileDeleted?.Invoke(this, this);
		Destroy(self);
	}
}
