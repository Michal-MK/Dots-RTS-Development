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

public static class IAllyExtension {
	public static bool IsAllyOf(this IAlly it, Player p ) {
		return p.IsAllyOF(it);
	}

	public static bool IsAllyOf(this Enemy_AI p, IAlly it) {
		return p.IsAllyOF(it);
	}
	public static bool IsAllyOf(this IAlly it, IAlly p) {
		return p.IsAllyOF(it);
	}
}

