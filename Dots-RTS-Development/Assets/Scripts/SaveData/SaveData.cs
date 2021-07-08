using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SaveData {
	public List<SerializedCell> Cells = new List<SerializedCell>();
	public SaveMeta SaveMeta;
	public float GameSize;
	public float GameAspect;
	public List<SerializedAI> Teams;
}