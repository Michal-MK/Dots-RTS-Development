using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpgradePanelData : MonoBehaviour {

	public Upgrade.Upgrades type;
	new public string name;
	public int count;
	public Image typeImage;

	private Upgrade_Manager currentCell;

	private void Awake() {
		Upgrade_Manager.OnUpgradeBegin += Upgrade_Manager_OnUpgradeBegin;
		Upgrade_Manager.OnUpgradeQuit += Upgrade_Manager_OnUpgradeQuit;
	}

	private void OnDisable() {
		Upgrade_Manager.OnUpgradeBegin -= Upgrade_Manager_OnUpgradeBegin;
		Upgrade_Manager.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;

	}

	private void Upgrade_Manager_OnUpgradeQuit(Upgrade_Manager sender) {
		currentCell = null;
	}

	private void Upgrade_Manager_OnUpgradeBegin(Upgrade_Manager sender) {
		currentCell = sender;
	}

	// Use this for initialization
	void Start() {

	}

}
