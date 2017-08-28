using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour {
	public Dictionary<Upgrade.Upgrades, int> test = new Dictionary<Upgrade.Upgrades, int>() {
		{Upgrade.Upgrades.NONE, 0 },
		{Upgrade.Upgrades.ATK_CRITICAL_CHANCE, 0 },
		{Upgrade.Upgrades.ATK_DOT, 0 },
		{Upgrade.Upgrades.ATK_DOUBLE_DAMAGE, 0 },
		{Upgrade.Upgrades.ATK_SLOW_REGENERATION, 0 },

	};

	private void Start() {
		StartCoroutine(Testaaa());
		FillUpgradeint();
	}

	private IEnumerator Testaaa() {
		yield return new WaitForSeconds(5);
		foreach (KeyValuePair<Upgrade.Upgrades, int> item in test) {
			print(item.Value);
		}
	}

	public async Task FillUpgradeint() {
		List<Task<int>> t = new List<Task<int>>();
		foreach (KeyValuePair<Upgrade.Upgrades, int> dictItem in test) {
			print(dictItem.Key + "    " + dictItem.Value);
			t.Add(MethodA(dictItem.Key));
			//test[dictItem.Key] = await MethodA(dictItem.Key);
		}
		int[] w = await Task.WhenAll(t);

		for (int i = 0; i < test.Count; i++) {
			test[test.ElementAt(i).Key] = w[i];
		}
	}


	public async Task<int> MethodA(Upgrade.Upgrades type) {
		print("Awaiting " + type);
		await Task.Delay(500);
		print("Expected output " + (int)type);
		return (int)type;
	}
}
