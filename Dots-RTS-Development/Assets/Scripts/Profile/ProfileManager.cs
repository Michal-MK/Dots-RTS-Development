using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager {

	public delegate void profileDeleted(ProfileInfo sender);

	private GameObject profileVisual;
	private Transform parentTransform;
	private static Profile currentProfile;
	public bool isProfileSelected = false;

	private Button b;
	public GameObject profileCreation;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="visualRepresentation">The Prefab to instantiate</param>
	/// <param name="listParent">Transform under which to sort Items</param>
	public ProfileManager(GameObject visualRepresentation, Transform listParent) {
		profileVisual = visualRepresentation;
		parentTransform = listParent;
		b = GameObject.Find("NoProfiles").GetComponent<Button>();
		b.gameObject.SetActive(false);
		profileCreation = GameObject.Find("ProfileCreation");
		profileCreation.SetActive(false);
		ProfileInfo.OnProfileDeleted += ProfileInfo_OnProfileDeleted;
	}

	private void ProfileInfo_OnProfileDeleted(ProfileInfo sender) {
		ListProfiles();
	}

	public void ProfileSelection() {
		Control.DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(Scenes.PROFILES);
	}

	public void ListProfiles() {
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles");
		FileInfo[] files = dir.GetFiles("*.gp");
		BinaryFormatter bf = new BinaryFormatter();
		if (parentTransform != null) {
			foreach (ProfileInfo pF in parentTransform.GetComponentsInChildren<ProfileInfo>()) {
				GameObject.Destroy(pF.gameObject);
			}
		}

		if (files.Length == 0) {
			Debug.Log("No Profies Found");
			if (b == null) {
				b = GameObject.Find("Content").transform.Find("NoProfiles").GetComponent<Button>();
			}
			b.gameObject.SetActive(true);
			b.onClick.RemoveAllListeners();
			b.onClick.AddListener(() => ShowProfileCreation());
			return;
		}
		foreach (FileInfo f in files) {
			using (FileStream fs = File.Open(f.FullName, FileMode.Open)) {
				try {
					Profile p = (Profile)bf.Deserialize(fs);
					ProfileInfo pI = GameObject.Instantiate(profileVisual, parentTransform).GetComponent<ProfileInfo>();
					pI.name = f.FullName;
					pI.self = pI.gameObject;
					pI.InitializeProfile(p, p.profileName);
				}
				catch (Exception e) {
					Debug.Log(e);
				}
			}
		}
	}

	public void ShowProfileCreation() {
		b.gameObject.SetActive(false);
		profileCreation.SetActive(true);
		Button creationB = GameObject.Find("CreateProfileButton").GetComponent<Button>();
		creationB.onClick.RemoveAllListeners();
		creationB.onClick.AddListener(() => CreateNewProfile());
		Debug.Log("Create");
	}

	private void CreateNewProfile() {
		string name = GameObject.Find("Name_IF").GetComponent<InputField>().text;

		if (name.Length < 3) {
			Debug.Log("Invalid name");
		}
		foreach (char ch in Path.GetInvalidFileNameChars()) {
			for (int i = 0; i < name.Length; i++) {
				if (name[i] == ch) {
					Debug.Log("Contains Invalid Character");
					return;
				}
			}
		}

		BinaryFormatter bf = new BinaryFormatter();
		Profile p = new Profile();

		p.profileName = name;
		p.creationTime = DateTime.Now;
		p.ownedCoins = 0;
		p.contributedLevels = 0;
		p.totalCreatedLevels = 0;
		p.onLevelBaseGame = new CampaignLevelCode(1, 1);
		p.onLevelImage = null;
		p.pathToFile = Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles" + Path.DirectorySeparatorChar + name + ".gp";

		using (FileStream fs = new FileStream(p.pathToFile, FileMode.Create)) {
			bf.Serialize(fs, p);
		}
		profileCreation.SetActive(false);
		Debug.Log("Created");
		ListProfiles();

	}

	public static Profile getCurrentProfile {
		get { return currentProfile; }
	}
	public static Profile setCurrentProfile {
		set { currentProfile = value; }
	}

	public static void SerializeChanges() {
		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream fs = new FileStream(getCurrentProfile.pathToFile, FileMode.Open)) {
			bf.Serialize(fs, getCurrentProfile);
		}
	}

}

[Serializable]
public class Profile {
	public string pathToFile;

	public byte[] onLevelImage;
	public CampaignLevelCode onLevelBaseGame;
	public int ownedCoins;

	public int contributedLevels;
	public int totalCreatedLevels;

	public DateTime creationTime;

	public string profileName;


	public int completedCampaignLevels;
	public int completedCustomLevels;

	public Dictionary<Upgrade.Upgrades, int> acquiredUpgrades = new Dictionary<Upgrade.Upgrades, int>();
	public Dictionary<SaveDataCampaign, float> clearedCampaignLevels = new Dictionary<SaveDataCampaign, float>();

	public Profile() {
		foreach (Upgrade.Upgrades u in Enum.GetValues(typeof(Upgrade.Upgrades))) {
			if (u != Upgrade.Upgrades.NONE) {
				acquiredUpgrades.Add(u,0);
			}
		}
		creationTime = DateTime.Now;
	}
}
