using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour {
	public Dictionary<Upgrades, int> test = new Dictionary<Upgrades, int>() {
		{Upgrades.None, 0 },
		{Upgrades.AtkCriticalChance, 0 },
		{Upgrades.AtkDot, 0 },
		{Upgrades.AtkDoubleDamage, 0 },
		{Upgrades.AtkSlowRegeneration, 0 },

	};

	private async void Start() {
		StartCoroutine(Testaaa());
		await FillUpgradeint();
	}

	private IEnumerator Testaaa() {
		yield return new WaitForSeconds(5);
		foreach (KeyValuePair<Upgrades, int> item in test) {
			print(item.Value);
		}
	}

	public async Task FillUpgradeint() {
		List<Task<int>> t = new List<Task<int>>();
		foreach (KeyValuePair<Upgrades, int> dictItem in test) {
			print(dictItem.Key + "    " + dictItem.Value);
			t.Add(MethodA(dictItem.Key));
			//test[dictItem.Key] = await MethodA(dictItem.Key);
		}
		int[] w = await Task.WhenAll(t);

		for (int i = 0; i < test.Count; i++) {
			test[test.ElementAt(i).Key] = w[i];
		}
	}


	public async Task<int> MethodA(Upgrades type) {
		print("Awaiting " + type);
		await Task.Delay(500);
		print("Expected output " + (int)type);
		return (int)type;
	}
}
