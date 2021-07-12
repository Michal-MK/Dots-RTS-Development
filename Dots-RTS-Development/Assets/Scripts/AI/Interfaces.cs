using System.Collections.Generic;

public interface IAlly {
	Team Team { get; }
	List<IAlly> Targets { get; set; }
	List<IAlly> Allies { get; set; }
	List<GameCell> MyCells { get; }

	bool IsAllyOf(IAlly other);
	bool IsTargetOf(IAlly other);

	void AddAlly(IAlly ally);
	void RemoveAlly(IAlly ally);
	void AddTarget(IAlly target);
	void RemoveTarget(IAlly target);
	void ProcessData(AIDataHolder currData, bool addTargets);
}
