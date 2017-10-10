using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IAlly {
	bool IsAllyOF(IAlly other);

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

