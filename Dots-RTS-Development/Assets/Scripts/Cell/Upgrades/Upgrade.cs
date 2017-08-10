using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Upgrade : MonoBehaviour {

	public Upgrades upgrade;

	public enum Upgrades {
		NONE,
		DOUBLE_DAMAGE,
		CRITICAL_CHANCE,
		SLOW_REGENERATION,
		DOT,
	}

	public static Dictionary<Upgrades, int> UPGRADE_COST = new Dictionary<Upgrades, int>() {
		{Upgrades.NONE , 0 },
		{Upgrades.CRITICAL_CHANCE, 10 },
		{Upgrades.DOUBLE_DAMAGE, 20 },
		{Upgrades.SLOW_REGENERATION, 8 },
		{Upgrades.DOT, 8 },

	};


	public int GetCost(Upgrades upgrade) {
		foreach(KeyValuePair<Upgrades,int> kvp in UPGRADE_COST) {
			if(kvp.Key == upgrade) {
				return kvp.Value;
			}
		}
		Debug.LogWarning("No Upgrade found! Dictionary may be incomplete.");
		return -1;
	}


}