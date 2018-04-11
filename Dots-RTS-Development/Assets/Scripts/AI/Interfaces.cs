using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IAlly {
	Cell.enmTeam Team { get; }
	List<IAlly> Targets { get; set; }
	List<IAlly> Allies { get; set; }
	List<CellBehaviour> MyCells { get; }

	bool IsAllyOf(IAlly other);
	bool IsTargetOf(IAlly other);

	void AddAlly(IAlly ally);
	void RemoveAlly(IAlly ally);
	void AddTarget(IAlly target);
	void RemoveTarget(IAlly target);
}


