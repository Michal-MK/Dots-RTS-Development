using UnityEngine;

public class UpgradeSelector : MonoBehaviour {

	public void Setup(UpgradeSlot_UI upgradeSlot_UI) {
		gameObject.SetActive(true);

		PopulateUpgradeVisuals visuals = transform.Find("_/UpgradeGrid").GetComponent<PopulateUpgradeVisuals>();
		if (visuals.Instances.Count == 0) {
			visuals.Initialize();
		}

		foreach (var pick in visuals.Instances) {
			pick.OnPickerClicked += upgradeSlot_UI.OnPickerPicked;
		}
	}


	public void Setup(UpgradeSlot_Cell upgradeSlot) {
		gameObject.SetActive(true);

		PopulateUpgradeVisuals visuals = transform.Find("_/UpgradeGrid").GetComponent<PopulateUpgradeVisuals>();
		if (visuals.Instances.Count == 0) {
			visuals.Initialize();
		}

		foreach (var pick in visuals.Instances) {
			pick.OnPickerClicked += upgradeSlot.InstallUpgradeDirectly;
		}
	}

	public void Clean(UpgradeSlot_Cell upgradeSlot) {
		gameObject.SetActive(false);

		PopulateUpgradeVisuals visuals = transform.Find("_/UpgradeGrid").GetComponent<PopulateUpgradeVisuals>();
		if (visuals.Instances.Count == 0) {
			visuals.Initialize();
		}

		foreach (var pick in visuals.Instances) {
			pick.OnPickerClicked -= upgradeSlot.InstallUpgradeDirectly;
		}
	}

	public void Clean(UpgradeSlot_UI upgradeSlot_UI) {
		gameObject.SetActive(false);
		PopulateUpgradeVisuals visuals = transform.Find("_/UpgradeGrid").GetComponent<PopulateUpgradeVisuals>();

		foreach (var pick in visuals.Instances) {
			pick.OnPickerClicked -= upgradeSlot_UI.OnPickerPicked;
		}
	}
}
