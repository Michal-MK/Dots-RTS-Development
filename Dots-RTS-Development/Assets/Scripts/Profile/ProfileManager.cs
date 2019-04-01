using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager {

	public static Profile CurrentProfile { get; set; }

	public static bool ProfileSelected => CurrentProfile != null;

	public static ProfileManager Instance { get; set; }

	private readonly GameObject profileVisual;
	private readonly Transform parentTransform;

	private Button createProfile_Button;
	private GameObject profileCreation;

	public static ProfileManager Initialize(GameObject visualRepresentation, Transform listParent) {
		if(Instance != null) {
			return Instance;
		}
		return Instance = new ProfileManager(visualRepresentation, listParent);
	}

	private ProfileManager(GameObject visualRepresentation, Transform listParent) {
		Instance = this;
		profileVisual = visualRepresentation;
		parentTransform = listParent;
		createProfile_Button = GameObject.Find("NoProfiles").GetComponent<Button>();
		createProfile_Button.gameObject.SetActive(false);
		profileCreation = GameObject.Find("ProfileCreation");
		profileCreation.SetActive(false);
	}

	private void ProfileInfo_OnProfileDeleted(object sender, OnProfileInfoDeletedEventArgs pInfo) {
		ListProfiles();
	}

	public void ProfileSelection() {
		Control.DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(Scenes.PROFILES);
	}

	public void ListProfiles() {
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles");
		FileInfo[] files = dir.GetFiles("*.gp");

		foreach (ProfileInfo pF in parentTransform?.GetComponentsInChildren<ProfileInfo>()) {
			GameObject.Destroy(pF.gameObject);
		}

		if (files.Length == 0) {
			Debug.Log("No Profiles Found");
			if (createProfile_Button == null) {
				createProfile_Button = GameObject.Find("Content").transform.Find("NoProfiles").GetComponent<Button>();
			}
			createProfile_Button.gameObject.SetActive(true);
			createProfile_Button.onClick.RemoveAllListeners();
			createProfile_Button.onClick.AddListener(() => ShowProfileCreation());
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();

		foreach (FileInfo f in files) {
			using (FileStream fs = File.Open(f.FullName, FileMode.Open)) {
				Profile p = (Profile)bf.Deserialize(fs);
				ProfileInfo pI = GameObject.Instantiate(profileVisual, parentTransform).GetComponent<ProfileInfo>();
				pI.name = f.FullName;
				pI.OnProfileDeleted += ProfileInfo_OnProfileDeleted;
				pI.InitializeProfile(p, p.Name);
			}
		}
	}

	public void ShowProfileCreation() {
		createProfile_Button.gameObject.SetActive(false);
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
		using (FileStream fs = new FileStream(CurrentProfile.DataFilePath, FileMode.Open)) {
			bf.Serialize(fs, CurrentProfile);
		}
	}
}