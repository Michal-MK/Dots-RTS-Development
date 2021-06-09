using System.Collections.Generic;

public class Player : IAlly {

	public Team Team => Team.ALLIED;

	public List<IAlly> Targets { get; set; } = new List<IAlly>();

	public List<IAlly> Allies { get; set; } = new List<IAlly>();

	public List<GameCell> MyCells { get; } = new List<GameCell>();

	public List<GameCell> Selection { get; } = new List<GameCell>();



	#region Ally/Target manipulation
	public bool IsAllyOf(IAlly other) {
		return Allies.Contains(other);
	}

	public bool IsTargetOf(IAlly other) {
		return Targets.Contains(other);
	}

	public void AddAlly(IAlly ally) {
		Allies.Add(ally);
	}

	public void RemoveAlly(IAlly ally) {
		Allies.Remove(ally);
	}

	public void AddTarget(IAlly target) {
		Targets.Add(target);
	}

	public void RemoveTarget(IAlly target) {
		Targets.Remove(target);
	}
	#endregion
}