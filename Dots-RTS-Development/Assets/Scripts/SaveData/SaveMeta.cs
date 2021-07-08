using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SaveMeta {
	public string LevelName;
	public string CreatorName;
	public DateTime CreationTime;

	public SaveMeta(string levelName, string creator, DateTime time) {
		LevelName = levelName;
		CreatorName = creator;
		CreationTime = time;
	}
}
