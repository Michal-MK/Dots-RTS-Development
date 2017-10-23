using System;
using System.Collections.Generic;
using UnityEngine;

public static class AI_Helper {
	//Functions to find ally moved to the interface IAlly interface
	public static void AddAlly(this Enemy_AI ai, Enemy_AI ally) {
		ai.getAiAllies.Add(ally);
	}

	public static void AddTarget(this Enemy_AI ai, Enemy_AI target) {
		ai.getAiTargets.Add(target);
	}

}

