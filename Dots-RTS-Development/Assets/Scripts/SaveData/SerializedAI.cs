using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SerializedAI {
	public Team Team;
	public float Difficulty;
	public AIHolder ConfigHolder;
}