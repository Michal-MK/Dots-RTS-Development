﻿using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UM_Store : Upgrade_Manager {

	/// <summary>
	/// Selected item in the upgrade shop.
	/// </summary>
	public static int selectedUpgrade = -1;

	/// <summary>
	/// Buy selected upgrade from the store
	/// </summary>
	public void BuyUpgrade() {
		//GameObject upgrade = GameObject.Find("Upgrade" + selectedUpgrade);
		Animator anim = GameObject.Find("Warning").GetComponent<Animator>();
		//Preform some highlights

		//Subtract total money + add the upgrade to proflie
		int cost = Upgrade.GetCost((Upgrades)selectedUpgrade);

		foreach (KeyValuePair<Upgrades, int> col in ProfileManager.CurrentProfile.acquiredUpgrades) {
			if (col.Key == (Upgrades)selectedUpgrade) {
				if (cost <= ProfileManager.CurrentProfile.Coins) {
					ProfileManager.CurrentProfile.Coins -= cost;
					ProfileManager.CurrentProfile.acquiredUpgrades[col.Key] += 1;
					ProfileManager.SerializeChanges();
					UI_ReferenceHolder.U_profileMoney.text = ProfileManager.CurrentProfile.Coins + " coins";
					UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.CurrentProfile.acquiredUpgrades[col.Key] + " pcs.";
					return;
				}
				else {
					anim.GetComponent<TextMeshProUGUI>().text = "You are missing\n" + (cost - ProfileManager.CurrentProfile.Coins) + " coins.";
					anim.Play("Show");
				}
			}
		}

	}
}
