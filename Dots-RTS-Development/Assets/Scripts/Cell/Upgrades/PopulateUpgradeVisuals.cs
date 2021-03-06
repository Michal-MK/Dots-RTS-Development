﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PopulateUpgradeVisuals : MonoBehaviour {

	public GameObject populateWithPrefab;

	private void Start() {
		if (SceneManager.GetActiveScene().name == Scenes.SHOP) {
			PopulateStore();
		}
		else if (SceneManager.GetActiveScene().name == Scenes.EDITOR) {
			PopulateEditorPicker();
		}
		else {
			Populate(transform);
		}
	}

	private void PopulateEditorPicker() {
		Upgrade.Upgrades[] upgrades = (Upgrade.Upgrades[])Enum.GetValues(typeof(Upgrade.Upgrades));
		string[] upgradeNames = Enum.GetNames(typeof(Upgrade.Upgrades));

		for (int i = 0; i < Enum.GetValues(typeof(Upgrade.Upgrades)).Length; i++) {
			if (upgrades[i] != Upgrade.Upgrades.NONE) {
				UpgradePickerInstance instance = Instantiate(populateWithPrefab, transform).GetComponent<UpgradePickerInstance>();
				instance.gameObject.name = upgradeNames[i];
				instance.upgrade = upgrades[i];

				if ((int)upgrades[i] <= 99) {
					instance.upgradeType = Upgrade.UpgradeType.OFFENSIVE;
				}
				else if ((int)upgrades[i] >= 100 && (int)upgrades[i] < 199) {
					instance.upgradeType = Upgrade.UpgradeType.DEFENSIVE;
				}
				else {
					instance.upgradeType = Upgrade.UpgradeType.UTILITY;
				}
				Sprite s;
				if (Upgrade.UPGRADE_GRAPHICS.TryGetValue(upgrades[i], out s)) {
					instance.upgradeImg.sprite = s;
				}
			}
		}
	}

	public void Populate(Transform parent) {
		foreach (UpgradePanelData data in parent.GetComponentsInChildren<UpgradePanelData>()) {
			foreach (KeyValuePair<Upgrade.Upgrades, int> owned in ProfileManager.getCurrentProfile.acquiredUpgrades) {
				if (owned.Key == data.type) {
					data.count = owned.Value;

					data.name = FolderAccess.GetUpgradeName(owned.Key);
					data.UpdateUpgradeOverview();
				}
				else if (data.type == Upgrade.Upgrades.NONE) {
					data.UpdateUpgradeOverview();
				}
			}
		}
		//StartCoroutine(CYCLES());
	}

	public void PopulateStore() {

		foreach (Image i in transform.Find("UPGRADE_Panel/ATK_Upgrades").GetComponentsInChildren<Image>()) {
			if (i.gameObject.name != "ATK_Upgrades" && i.gameObject.name != "Image") {
				try {
					i.sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrade.Upgrades)(int.Parse(string.Format(i.gameObject.name).Remove(0, 8)))];
				}
				catch (KeyNotFoundException) {
					i.sprite = FolderAccess.GetNIYImage();
				}
			}
		}
		foreach (Image i in transform.Find("UPGRADE_Panel/DEF_Upgrades").GetComponentsInChildren<Image>()) {
			if (i.gameObject.name != "DEF_Upgrades" && i.gameObject.name != "Image") {
				try {
					i.sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrade.Upgrades)(int.Parse(string.Format(i.gameObject.name).Remove(0, 8)))];
				}
				catch (KeyNotFoundException) {
					i.sprite = FolderAccess.GetNIYImage();
				}
			}
		}
		foreach (Image i in transform.Find("UPGRADE_Panel/UTIL_Upgrades").GetComponentsInChildren<Image>()) {
			if (i.gameObject.name != "UTIL_Upgrades" && i.gameObject.name != "Image") {
				try {
					i.sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrade.Upgrades)(int.Parse(string.Format(i.gameObject.name).Remove(0, 8)))];
				}
				catch (KeyNotFoundException) {
					i.sprite = FolderAccess.GetNIYImage();
				}
			}
		}
	}

	#region Debug CYCLES()
	//private IEnumerator CYCLES() {
	//	Image i = GameObject.Find("Cycle").transform.Find("UpgradeImg").gameObject.GetComponent<Image>();
	//	int inte = 0;
	//	while (true) {
	//		yield return new WaitForSeconds(0.5f);
	//		print("Sprite " + (Upgrade.Upgrades)inte);
	//		i.sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrade.Upgrades)inte];
	//		inte++;
	//		if (inte > 3) {
	//			inte = 0;
	//		}
	//	}
	//}
	#endregion
}
