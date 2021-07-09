using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "JsonUtil class uses files only, this is deo to mimic property syntax")]
public class SaveData {
	public List<SerializedCell> Cells = new List<SerializedCell>();
	public SaveMeta SaveMeta;
	public float GameSize;
	public float GameAspect;
	public List<SerializedAI> Teams;
}
