using System;
using System.Collections.Generic;
using UnityEngine;

public static class AI_Helper {
	public static bool IsTargetOf(this Enemy_AI ai, Enemy_AI other) {
		foreach (Enemy_AI temp in ai.getTargets) {
			if (temp == other) {
				return true;
			}
		}
		return false;
	}

	public static bool IsAllyOf(this Enemy_AI ai, Enemy_AI other) {
		foreach (Enemy_AI temp in other.getAllies) {
			if (ai == temp) {
				return true;
			}
		}
		return false;

	}

	//public static bool IsAllyOf(this IAlly it, Player other) {
	//	return other.IsAllyOF(it);
	//}

	public static bool IsAllyOf(this IAlly it, IAlly other) {
		return other.IsAllyOF(it);
	}

	public static void AddAlly(this Enemy_AI ai, Enemy_AI ally) {
		ai.getAllies.Add(ally);
	}

	public static void AddTarget(this Enemy_AI ai,Enemy_AI target) {
		ai.getTargets.Add(target);
	}
}

