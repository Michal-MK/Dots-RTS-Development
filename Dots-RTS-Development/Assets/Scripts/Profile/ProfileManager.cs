using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class ProfileManager {
	public static Profile CurrentProfile { get; set; }

	public static ProfileManager Instance { get; private set; }

	private ProfileManagerBehaviour behaviour;

	private Button createProfileButton;
	private GameObject profileCreation;

	public static ProfileManager Initialize() {
		if (Instance != null) {
			return Instance;
		}
		Instance = new ProfileManager();
		SceneLoader.Instance.OnSceneChanged += Instance.OnOnSceneChanged;
		return Instance;
	}

	private ProfileManager() { }

	private void OnOnSceneChanged(object sender, SceneChangedEventArgs e) {
		if (e.Name != Scenes.PROFILES) return;

		behaviour = UI_ReferenceHolder.ProfilesBehaviour;
		createProfileButton = GameObject.Find("NoProfiles").GetComponent<Button>();
		createProfileButton.gameObject.SetActive(false);
		profileCreation = GameObject.Find("ProfileCreation");
		profileCreation.SetActive(false);
		ListProfiles();
	}

	private void ProfileInfo_OnProfileDeleted(object sender, OnProfileInfoDeletedEventArgs pInfo) {
		ListProfiles();
	}

	private void ListProfiles() {
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles");
		FileInfo[] files = dir.GetFiles("*.gp");

		foreach (ProfileInfo pF in behaviour.listTransform.GetComponentsInChildren<ProfileInfo>()) {
			Object.Destroy(pF.gameObject);
		}

		if (files.Length == 0) {
			Debug.Log("No Profiles Found");
			if (createProfileButton == null) {
				createProfileButton = GameObject.Find("Content").transform.Find("NoProfiles").GetComponent<Button>();
			}
			createProfileButton.gameObject.SetActive(true);
			createProfileButton.onClick.RemoveAllListeners();
			createProfileButton.onClick.AddListener(ShowProfileCreation);
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();

		foreach (FileInfo f in files) {
			using FileStream fs = File.Open(f.FullName, FileMode.Open);
			Profile p = (Profile)bf.Deserialize(fs);
			ProfileInfo pI = Object.Instantiate(behaviour.profileVisual, behaviour.listTransform).GetComponent<ProfileInfo>();
			pI.name = f.FullName;
			pI.OnProfileDeleted += ProfileInfo_OnProfileDeleted;
			pI.InitializeProfile(p, p.Name);
		}
	}

	public void ShowProfileCreation() {
		createProfileButton.gameObject.SetActive(false);
		profileCreation.SetActive(true);
		Button creationB = GameObject.Find("CreateProfileButton").GetComponent<Button>();
		creationB.onClick.RemoveAllListeners();
		creationB.onClick.AddListener(CreateNewProfile);
		Debug.Log("Create");
	}

	private void CreateNewProfile() {
		string name = GameObject.Find("Name_IF").GetComponent<InputField>().text;

		if (name.Length < 3) {
			Debug.Log("Invalid name");
		}
		if (Path.GetInvalidFileNameChars().Any(ch => name.Any(t => t == ch))) {
			Debug.Log("Contains Invalid Character");
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();
		Profile p = new Profile(name);

		//TODO Sanitize FS access
		using (FileStream fs = new FileStream(p.DataFilePath, FileMode.Create)) {
			bf.Serialize(fs, p);
		}

		profileCreation.SetActive(false);
		ListProfiles();
	}

	public static void SerializeChanges() {
		BinaryFormatter bf = new BinaryFormatter();
		using FileStream fs = new FileStream(CurrentProfile.DataFilePath, FileMode.Open);
		bf.Serialize(fs, CurrentProfile);
	}
}