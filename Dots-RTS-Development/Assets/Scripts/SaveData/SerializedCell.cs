using System;

[Serializable]
public class SerializedCell {
	public SerializedVector3 Position { get; set; }
	public int Elements { get; set; }
	public int MaximumElements { get; set; }
	public Team Team { get; set; }
	public float RegenerationPeriod { get; set; }
	public Upgrades[] InstalledUpgrades { get; set; }
}
