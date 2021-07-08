using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SerializedCell {
	public SerializedVector3 Position;
	public int Elements;
	public int MaximumElements;
	public Team Team;
	public float RegenerationPeriod;
	public Upgrades[] InstalledUpgrades;
}
