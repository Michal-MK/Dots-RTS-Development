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

	public int TotlaContributedLevels { get; set; }

	public DateTime CreationDate { get; }

	public string DataFilePath { get; }

	public byte[] LevelImageDataFilePath { get; }

	public int CompletedCampaignLevels { get; set; }

	public int CompletedCustomLevels { get; set; }

	public Dictionary<Upgrades, int> acquiredUpgrades { get; set; } = new Dictionary<Upgrades, int>();

	public Dictionary<SaveDataCampaign, float> clearedCampaignLevels { get; set; } = new Dictionary<SaveDataCampaign, float>(); //TOTO structure instead of a float

	public Profile() {
		foreach (Upgrades u in Enum.GetValues(typeof(Upgrades))) {
			if (u != Upgrades.NONE) {
				acquiredUpgrades.Add(u, 0);
			}
		}
		CreationDate = DateTime.Now;
	}

	public Profile(string profileName, int startingConins = 0) : this() {
		Name = profileName;
		CurrentCampaignLevel = new CampaignLevelCode(1, 1);
		char sep = Path.DirectorySeparatorChar;
		DataFilePath = $"{Application.persistentDataPath}{sep}Profiles{sep}{Name}.gp";
	}
}