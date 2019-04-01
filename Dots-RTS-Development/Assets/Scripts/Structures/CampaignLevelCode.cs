using System;

[Serializable]
public struct CampaignLevelCode {
	public CampaignLevelCode(int difficulty, int levelInDifficulty) {
		Difficulty = difficulty;
		Devel = levelInDifficulty;
	}

	public int Difficulty { get; }

	public int Devel { get; }
}