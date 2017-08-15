using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Upgrade : MonoBehaviour {

	public Upgrades upgrade;

	public enum Upgrades {
		NONE,
		DOT,
		DOUBLE_DAMAGE,
		SLOW_REGENERATION,
		CRITICAL_CHANCE,
	}

	public static Dictionary<Upgrades, int> UPGRADE_COST = new Dictionary<Upgrades, int>() {
		{Upgrades.NONE , 0 },
		{Upgrades.CRITICAL_CHANCE, 15 },
		{Upgrades.DOUBLE_DAMAGE, 8 },
		{Upgrades.SLOW_REGENERATION, 11 },
		{Upgrades.DOT, 4 },

	};


	public static int GetCost(Upgrades upgrade) {
		foreach(KeyValuePair<Upgrades,int> kvp in UPGRADE_COST) {
			if(kvp.Key == upgrade) {
				return kvp.Value;
			}
		}
		Debug.LogWarning("No Upgrade found! Dictionary may be incomplete.");
		return -1;
	}


}