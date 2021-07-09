using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "JsonUtil class uses files only, this is deo to mimic property syntax")]
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
