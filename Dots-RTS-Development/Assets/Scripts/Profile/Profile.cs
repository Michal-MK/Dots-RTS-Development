using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Profile {

	public string Name { get; set; }

	public CampaignLevelCode CurrentCampaignLevel { get; set; }

	public int Coins { get; set; }

	public int UserMadeLevels { get; set; }

	public int TotalContributedLevels { get; set; }

	public DateTime CreationDate { get; }

	public string DataFilePath { get; }

	public byte[] LevelImageData { get; }

	public int CompletedCampaignLevels { get; set; }

	public int CompletedCustomLevels { get; set; }

	public Dictionary<Upgrades, int> AcquiredUpgrades { get; set; } = new Dictionary<Upgrades, int>();

	public Dictionary<SaveDataCampaign, float> ClearedCampaign { get; set; } = new Dictionary<SaveDataCampaign, float>(); //TOTO structure instead of a float

	public Profile() {
		foreach (Upgrades u in Enum.GetValues(typeof(Upgrades))) {
			if (u != Upgrades.NONE) {
				AcquiredUpgrades.Add(u, 0);
			}
		}
		CreationDate = DateTime.Now;
	}

	public Profile(string profileName) : this() {
		Name = profileName;
		CurrentCampaignLevel = new CampaignLevelCode(1, 1);
		char sep = Path.DirectorySeparatorChar;
		DataFilePath = $"{Application.persistentDataPath}{sep}Profiles{sep}{Name}.gp";
	}
}