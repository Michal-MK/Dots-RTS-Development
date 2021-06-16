using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreUpgradeManager : UpgradeManager {

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

		//Subtract total money + add the upgrade to profile
		int cost = Upgrade.GetCost((Upgrades)selectedUpgrade);

		foreach (KeyValuePair<Upgrades, int> col in ProfileManager.CurrentProfile.AcquiredUpgrades) {
			if (col.Key == (Upgrades)selectedUpgrade) {
				if (cost <= ProfileManager.CurrentProfile.Coins) {
					ProfileManager.CurrentProfile.Coins -= cost;
					ProfileManager.CurrentProfile.AcquiredUpgrades[col.Key] += 1;
					ProfileManager.SerializeChanges();
					UI_ReferenceHolder.U_profileMoney.text = ProfileManager.CurrentProfile.Coins + " coins";
					UI_ReferenceHolder.U_upgradesOwnedHolder.text = ProfileManager.CurrentProfile.AcquiredUpgrades[col.Key] + " pcs.";
					return;
				}
				anim.GetComponent<TextMeshProUGUI>().text = $"You are missing\n{(cost - ProfileManager.CurrentProfile.Coins)} coins.";
				anim.Play(AnimatorStates.SHOW);
			}
		}
	}
}
