using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "JsonUtil class uses files only, this is deo to mimic property syntax")]
public class SerializedAI {
	public Team Team;
	public float Difficulty;
	public AIHolder ConfigHolder;
}
