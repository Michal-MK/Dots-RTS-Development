using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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

	public void UpgradeOverview() {
		if (count > 0) {
			GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
		}
		else {
			GetComponent<Image>().color = new Color(1, 1, 1, 1);
		}

		transform.Find("UpgradeCount").gameObject.GetComponent<TextMeshProUGUI>().text = count.ToString();

		if (Upgrade.UPGRADE_GRAPHICS[type] != null) {
			typeImage.sprite = Upgrade.UPGRADE_GRAPHICS[type];
		}
	}
}
