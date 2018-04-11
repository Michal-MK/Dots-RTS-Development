using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData {
	public List<S_Cell> cells = new List<S_Cell>();
	public LevelInfo levelInfo;
	public Dictionary<Cell.enmTeam, float> difficulty;
	public float gameSize;
    public float savedAtAspect;
    public Dictionary<Cell.enmTeam,AIHolder> clans;

}

[Serializable]
public class SaveDataCampaign {
	public SaveData game;
	public string preview;
	public CampaignLevelCode code;
}

[Serializable]
public class S_Cell {
	public S_Vec3 pos;
	public int elementCount;
	public int maxElementCount;
	public int team;
	public float regenerationPeriod;
	public Upgrade.Upgrades[] installedUpgrades;

}

[Serializable]
public class S_Vec3 {
	public float x, y, z;

	public static explicit operator Vector3(S_Vec3 v) {
		return new Vector3(v.x, v.y, v.z);
	}
}

[Serializable]
public class LevelInfo {
	public string levelName;
	public string creator;
	public DateTime creationTime;

	public LevelInfo(string levelName, string creator, DateTime time) {
		this.levelName = levelName;
		this.creator = creator;
		this.creationTime = time;
	}
}
