using System;

[Serializable]
public class SaveMeta {
	public string LevelName { get; set; }
	public string CreatorName { get; set; }
	public DateTime CreationTime { get; set; }

	public SaveMeta(string levelName, string creator, DateTime time) {
		LevelName = levelName;
		CreatorName = creator;
		CreationTime = time;
	}
}
