using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager {
	private GameObject profileVisual;
	private Transform parent;
	public static Profile currentProfile;
	public bool isProfileSelected = false;

	private Button b;
	private GameObject profileCreation;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="visualRepresentation">The Prefab to instantiate</param>
	/// <param name="listParent">Transform under which to sort Items</param>
	public ProfileManager(GameObject visualRepresentation, Transform listParent) {
		profileVisual = visualRepresentation;
		parent = listParent;
		b = GameObject.Find("NoProfiles").GetComponent<Button>();
		b.gameObject.SetActive(false);
		profileCreation = GameObject.Find("ProfileCreation");
		profileCreation.SetActive(false);
	}


	public void ProfileSelection() {
		SceneManager.LoadScene("Profiles");
	}

	public void ListProfiles() {
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "\\Profiles\\");
		FileInfo[] files = dir.GetFiles("*.gp");
		BinaryFormatter bf = new BinaryFormatter();
		
		if(files.Length == 0) {
			b.gameObject.SetActive(true);
			b.onClick.AddListener(() => ShowProfileCreation());
			return;
		}
		for (int i = 0; i < 10; i++) {
			foreach (FileInfo f in files) {
				using (FileStream fs = File.Open(f.FullName, FileMode.Open)) {
					try {
						Profile p = (Profile)bf.Deserialize(fs);

						ProfileInfo pI = GameObject.Instantiate(profileVisual, parent).GetComponent<ProfileInfo>();
						pI.selected = p;
						pI.profileName.text = p.profileName;
						/*
						Texture2D t = new Texture2D(160, 90);
						t.LoadImage(File.ReadAllBytes(byte[] preview of the level that this profile is on));
						pI.careerLevel.texture = t;
						*/
					}
					catch (Exception e) {
						Debug.Log(e);
					}
				}
			}
		}
	}

	private void ShowProfileCreation() {
		b.gameObject.SetActive(false);
		profileCreation.SetActive(true);
		Button creationB = GameObject.Find("CreateProfileButton").GetComponent<Button>();
		creationB.onClick.AddListener(() => CreateNewProfile());
		Debug.Log("Create");
	}

	private void CreateNewProfile() {
		string name = GameObject.Find("Name_IF").GetComponent<InputField>().text;

		if(name.Length < 3) {
			Debug.Log("Invalid name");
		}
		foreach(char ch in Path.GetInvalidFileNameChars()) {
			for (int i = 0; i < name.Length; i++) {
				if(name[i] == ch) {
					Debug.Log("Contains Invalid Character");
					return;
				}
			}
		}

		BinaryFormatter bf = new BinaryFormatter();
		Profile p = new Profile();

		p.profileName = name;
		p.creationTime = DateTime.Now;
		p.currency = 0;
		p.contributedLevels = 0;
		p.totalCreatedLevels = 0;
		p.onLevelBaseGame = 1;
		p.onLevelImage = null;

		using(FileStream fs = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles" + Path.DirectorySeparatorChar + name + ".gp",FileMode.Create)) {
			bf.Serialize(fs, p);
		}
		profileCreation.SetActive(false);
		Debug.Log("Created");
		ListProfiles();
	}

	public Profile SetProfile {
		set {
			currentProfile = value;
		}
	}

	public static Profile GetCurrentProfile {
		get { return currentProfile; }
	} 

}

[Serializable]
public class Profile {
	public byte[] onLevelImage;
	public int onLevelBaseGame;
	public int currency;
	public int contributedLevels;
	public int totalCreatedLevels;
	public DateTime creationTime;
	public string profileName;

}
