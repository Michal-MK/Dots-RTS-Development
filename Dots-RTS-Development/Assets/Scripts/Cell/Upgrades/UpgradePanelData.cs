using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class UpgradePanelData : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler {

	public Upgrade.Upgrades type;
	new public string name;
	public int count;
	public Image typeImage;
	private TextMeshProUGUI desc;

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
		//print(sender);
		currentCell = sender;
	}

	// Use this for initialization
	void Start() {
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level_Player" || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Debug") {
			if (ProfileManager.getCurrentProfile == null) {
				Control.DebugSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
				UnityEngine.SceneManagement.SceneManager.LoadScene("Profiles");
				return;
			}
			if (type != Upgrade.Upgrades.NONE) {
				count = ProfileManager.getCurrentProfile.acquiredUpgrades[type];
				UpdateUpgradeOverview();
			}
			desc = transform.parent.parent.Find("Description").GetComponent<TextMeshProUGUI>();
		}
	}

	public void UpdateUpgradeOverview() {
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
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Profiles") {
			if (eventData.clickCount == 1) {
				UpgradeSlot.OnSlotClicked += InstallUpgradeTo;
			}
			else if (eventData.clickCount == 2) {

				if (currentCell.HasFreeSlots()) {
					int i = currentCell.GetFirstFreeSlot();
					if (i != -1) {
						if (count > 0) {
							InstallUpgradeTo(null, i);
						}
					}
					else {
						Debug.Log("No Free Slots Exist.");
					}
				}
			}
		}
	}

	private void InstallUpgradeTo(UpgradeSlot sender, int slot) {
		currentCell.upgrades[slot] = type;
		count--;
		UpdateUpgradeOverview();
		UpgradeSlot.OnSlotClicked -= InstallUpgradeTo;
		if (sender == null) {
			SpriteRenderer s = currentCell.transform.Find("UpgradeSlots/" + slot).GetComponent<SpriteRenderer>();
			s.sprite = Upgrade.UPGRADE_GRAPHICS[type];
			s.size = Vector2.one * 25;
		}
		else {
			sender.selfSprite.sprite = Upgrade.UPGRADE_GRAPHICS[type];
			sender.selfSprite.size = Vector2.one * 25;
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		desc.text = FolderAccess.GetUpgradeName(type);
	}

	public void OnPointerExit(PointerEventData eventData) {
		desc.text = "";
	}
}
