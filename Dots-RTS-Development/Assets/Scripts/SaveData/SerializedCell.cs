using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "JsonUtil class uses files only, this is deo to mimic property syntax")]
public class SerializedCell {
	public SerializedVector3 Position;
	public int Elements;
	public int MaximumElements;
	public Team Team;
	public float RegenerationPeriod;
	public Upgrades[] InstalledUpgrades;
}
