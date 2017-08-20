using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class UpgradePanelData : MonoBehaviour, IPointerClickHandler {

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
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level_Player" || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Debug") {
			if(ProfileManager.getCurrentProfile == null) {
				Control.DebugSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
				UnityEngine.SceneManagement.SceneManager.LoadScene("Profiles");
				return;
			}
			if (type != Upgrade.Upgrades.NONE) {
				typeImage.sprite = Upgrade.UPGRADE_GRAPHICS[type];
				count = ProfileManager.getCurrentProfile.acquiredUpgrades[type];
				transform.Find("UpgradeCount").GetComponent<TextMeshProUGUI>().text = count.ToString();
			}
		}
	}




	public void UpgradeOverview() {
		if (count == 0) {
			GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
		}
		else {
			GetComponent<Image>().color = new Color(1, 1, 1, 1);
		}

		transform.Find("UpgradeCount").gameObject.GetComponent<TextMeshProUGUI>().text = count.ToString();
		typeImage.sprite = Upgrade.UPGRADE_GRAPHICS[type];
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.clickCount == 1) {
			//Subscribe to upgrade slot on cell click to determine position.
			print("Select Pos to install");
		}
		else if (eventData.clickCount == 2) {
			print("DOUBLEEEEA " + currentCell.HasFreeSlots() + "  " + currentCell.GetFirstFreeSlot());
			if (currentCell.HasFreeSlots()) {
				int i = currentCell.GetFirstFreeSlot();
				if (i != -1) {
					if (count > 0) {
						print("INstall Upgrade " + type + " and subtract count by 1");
					}
				}
				else {
					Debug.Log("No Free Slots Exist.");
				}
			}
		}
	}
}
