using System;

[Serializable]
public class SaveDataCampaign {
	public SaveData Data { get; set; }
	public string LevelPreviewImagePath { get; set; }
	public CampaignLevelCode LevelCode { get; set; }
}
