using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PopulateUpgradeVisuals : MonoBehaviour {

	public GameObject populateWithPrefab;

	public List<EditorUpgradePicker> Instances { get; set; } = new List<EditorUpgradePicker>();

	public void Initialize() {
		if (SceneManager.GetActiveScene().name == Scenes.UPGRADE_SHOP) {
			PopulateStore();
		}
		else if (SceneManager.GetActiveScene().name == Scenes.LEVEL_EDITOR) {
			PopulateEditorPicker();
		}
		else {
			Populate(transform);
		}
	}

	private void PopulateEditorPicker() {
		Upgrades[] upgrades = (Upgrades[])Enum.GetValues(typeof(Upgrades));
		string[] upgradeNames = Enum.GetNames(typeof(Upgrades));

		for (int i = 0; i < Enum.GetValues(typeof(Upgrades)).Length; i++) {
			if (upgrades[i] != Upgrades.NONE) {
				EditorUpgradePicker instance = Instantiate(populateWithPrefab, transform).GetComponent<EditorUpgradePicker>();
				instance.gameObject.name = upgradeNames[i];
				instance.Upgrade = upgrades[i];

				if ((int)upgrades[i] <= 99) {
					instance.UpgradeType = UpgradeType.Offensive;
				}
				else if ((int)upgrades[i] >= 100 && (int)upgrades[i] < 199) {
					instance.UpgradeType = UpgradeType.Defensive;
				}
				else {
					instance.UpgradeType = UpgradeType.Utility;
				}
				Sprite s;
				if (Upgrade.UpgradeGraphics.TryGetValue(upgrades[i], out s)) {
					instance.upgradeImg.sprite = s;
				}
				Instances.Add(instance);
			}
		}
	}

	public void Populate(Transform parent) {
		//foreach (UpgradePanelData data in parent.GetComponentsInChildren<UpgradePanelData>()) {
		//	foreach (KeyValuePair<Upgrades, int> owned in ProfileManager.CurrentProfile.AcquiredUpgrades) {
		//		if (owned.Key == data.type) {
		//			data.count = owned.Value;

		//			data.name = FolderAccess.GetUpgradeName(owned.Key);
		//			data.UpdateUpgradeOverview();
		//		}
		//		else if (data.type == Upgrades.NONE) {
		//			data.UpdateUpgradeOverview();
		//		}
		//	}
		//}
		//StartCoroutine(CYCLES());
	}

	public void PopulateStore() {

		foreach (Image i in transform.Find("UPGRADE_Panel/ATK_Upgrades").GetComponentsInChildren<Image>()) {
			if (i.gameObject.name != "ATK_Upgrades" && i.gameObject.name != "Image") {
				try {
					i.sprite = Upgrade.UpgradeGraphics[(Upgrades)(int.Parse(string.Format(i.gameObject.name).Remove(0, 8)))];
				}
				catch (KeyNotFoundException) {
					i.sprite = FolderAccess.GetNIYImage();
				}
			}
		}
		foreach (Image i in transform.Find("UPGRADE_Panel/DEF_Upgrades").GetComponentsInChildren<Image>()) {
			if (i.gameObject.name != "DEF_Upgrades" && i.gameObject.name != "Image") {
				try {
					i.sprite = Upgrade.UpgradeGraphics[(Upgrades)(int.Parse(string.Format(i.gameObject.name).Remove(0, 8)))];
				}
				catch (KeyNotFoundException) {
					i.sprite = FolderAccess.GetNIYImage();
				}
			}
		}
		foreach (Image i in transform.Find("UPGRADE_Panel/UTIL_Upgrades").GetComponentsInChildren<Image>()) {
			if (i.gameObject.name != "UTIL_Upgrades" && i.gameObject.name != "Image") {
				try {
					i.sprite = Upgrade.UpgradeGraphics[(Upgrades)(int.Parse(string.Format(i.gameObject.name).Remove(0, 8)))];
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
	//		print("Sprite " + (Upgrades)inte);
	//		i.sprite = Upgrade.UPGRADE_GRAPHICS[(Upgrades)inte];
	//		inte++;
	//		if (inte > 3) {
	//			inte = 0;
	//		}
	//	}
	//}
	#endregion
}
