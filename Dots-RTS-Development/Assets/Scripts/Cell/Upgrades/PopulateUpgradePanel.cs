using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateUpgradePanel : MonoBehaviour {


	private async void Start() {
		await Upgrade.FillUpgradeSpriteDict();
		Populate(transform);
	}

	public void Populate(Transform parent) {
		foreach (UpgradePanelData data in parent.GetComponentsInChildren<UpgradePanelData>()) {
			foreach (KeyValuePair<Upgrade.Upgrades, int> owned in ProfileManager.getCurrentProfile.acquiredUpgrades) {
				if (owned.Key == data.type) {
					data.count = owned.Value;
					data.name = FolderAccess.GetUpgradeName(owned.Key);
					data.UpgradeOverview();
				}
				else if (data.type == Upgrade.Upgrades.NONE) {
					data.UpgradeOverview();
				}
			}
		}
	}

}
