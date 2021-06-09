using UnityEngine;

public class UpgradeSelector : MonoBehaviour {

	// TODO The same code is repeated infinitely many times here...
	public void Setup(UpgradeSlot_UI upgradeSlot) {
		gameObject.SetActive(true);

		PopulateUpgradeVisuals visuals = transform.Find("_/UpgradeGrid").GetComponent<PopulateUpgradeVisuals>();

		if (visuals.Instances.Count == 0) {
			visuals.Initialize();
		}

		foreach (EditorUpgradePicker pick in visuals.Instances) {
			pick.OnPickerClicked += upgradeSlot.OnPickerPicked;
		}
	}


	public void Setup(UpgradeSlot_Cell upgradeSlot) {
		gameObject.SetActive(true);

		PopulateUpgradeVisuals visuals = transform.Find("_/UpgradeGrid").GetComponent<PopulateUpgradeVisuals>();
		if (visuals.Instances.Count == 0) {
			visuals.Initialize();
		}

		foreach (EditorUpgradePicker pick in visuals.Instances) {
			pick.OnPickerClicked += upgradeSlot.InstallUpgradeDirectly;
		}
	}

	public void Clean(UpgradeSlot_Cell upgradeSlot) {
		gameObject.SetActive(false);

		PopulateUpgradeVisuals visuals = transform.Find("_/UpgradeGrid").GetComponent<PopulateUpgradeVisuals>();

		if (visuals.Instances.Count == 0) {
			visuals.Initialize();
		}

		foreach (EditorUpgradePicker pick in visuals.Instances) {
			pick.OnPickerClicked -= upgradeSlot.InstallUpgradeDirectly;
		}
	}

	public void Clean(UpgradeSlot_UI upgradeSlot) {
		gameObject.SetActive(false);

		PopulateUpgradeVisuals visuals = transform.Find("_/UpgradeGrid").GetComponent<PopulateUpgradeVisuals>();

		foreach (EditorUpgradePicker pick in visuals.Instances) {
			pick.OnPickerClicked -= upgradeSlot.OnPickerPicked;
		}
	}
}
