using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IAlly {
	Cell.enmTeam Team { get; }
	List<IAlly> Targets { get; set; }
	List<IAlly> Allies { get; set; }

	bool IsAllyOf(IAlly other);
	bool IsTargetOf(IAlly other);
}


