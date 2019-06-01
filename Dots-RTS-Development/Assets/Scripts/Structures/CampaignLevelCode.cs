using System;

[Serializable]
public struct CampaignLevelCode {
	public CampaignLevelCode(int difficulty, int levelInDifficulty) {
		Difficulty = difficulty;
		LevelID = levelInDifficulty;
	}

	public int Difficulty { get; }

	public int LevelID { get; }
}