using System;
using System.Collections.Generic;

[Serializable]
public class SaveData {
	public List<SerializedCell> Cells { get; set; } = new List<SerializedCell>();
	public SaveMeta SaveMeta { get; set; }
	public Dictionary<Team, float> Difficulties { get; set; }
	public float GameSize { get; set; }
	public float GameAspect { get; set; }
	public Dictionary<Team, AIHolder> Teams { get; set; }
}

