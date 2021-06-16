using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UpgradePanelData : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public Upgrades type;
	new public string name;
	public int count;
	public Image typeImage;
	private TextMeshProUGUI desc;

	private static InGameUpgradeManager currentCell;

	private static bool isListeningForSlot = true;

	private void Start() {
		if (SceneManager.GetActiveScene().name == Scenes.GAME || SceneManager.GetActiveScene().name == Scenes.DEBUG) {
			if (type != Upgrades.NONE) {
				count = 0;//ProfileManager.CurrentProfile.AcquiredUpgrades[type];
				UpdateUpgradeOverview();
			}
			desc = transform.parent.parent.Find("Description").GetComponent<TextMeshProUGUI>();
		}
	}

	private void Upgrade_Manager_OnUpgradeQuit(object sender, InGameUpgradeManager e) {
		e.OnUpgradeQuit -= Upgrade_Manager_OnUpgradeQuit;

		print($"Upgrade Quit {e.gameObject.name}");
		e.slotRender.color = new Color(1, 1, 1, 0);
		foreach (BoxCollider2D col in e.slotHolder.GetComponentsInChildren<BoxCollider2D>()) {
			col.enabled = false;
		}
		UpgradeManager.isUpgrading = false;
		currentCell = null;
	}

	private void Upgrade_Manager_OnUpgradeBegin(object sender, InGameUpgradeManager e) {
		e.OnUpgradeQuit += Upgrade_Manager_OnUpgradeQuit;

		print("Upgrade Begin " + e.gameObject.name);
		currentCell = e;
		currentCell.slotRender.color = new Color(1, 1, 1, 0.2f);
		foreach (BoxCollider2D col in currentCell.slotHolder.GetComponentsInChildren<BoxCollider2D>()) {
			col.enabled = true;
		}
	}

	//Updates upgrade image visuals
	public void UpdateUpgradeOverview() {
		if (count == 0) {
			GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
			transform.Find("UpgradeCount").gameObject.GetComponent<TextMeshProUGUI>().text = "";
		}
		else {
			GetComponent<Image>().color = new Color(1, 1, 1, 1);
			transform.Find("UpgradeCount").gameObject.GetComponent<TextMeshProUGUI>().text = count.ToString();
		}
		typeImage.sprite = Upgrade.UpgradeGraphics[type];
	}

	//Upgrade installation Logic
	public void OnPointerClick(PointerEventData eventData) {
		if (SceneManager.GetActiveScene().name != Scenes.PROFILES) {
			if (currentCell.HasFreeSlots() && count > 0) {

				if (eventData.clickCount == 1) {
					if (isListeningForSlot) {
						//UpgradeSlot.OnSlotClicked += InstallUpgradeTo; //TODO
						isListeningForSlot = false;
					}
					currentCell.slotRender.color = new Color(1, 1, 1, 0.8f);
				}
				else if (eventData.clickCount == 2) {
					//UpgradeSlot.OnSlotClicked -= InstallUpgradeTo; //TODO
					int i = currentCell.GetFirstFreeSlot();
					InstallUpgradeTo(null, new OnUpgradeSlotClickedEventArgs(null, i));
				}
			}
		}
	}

	//Stuff to do with installation, moving numbers around
	private void InstallUpgradeTo(object sender, OnUpgradeSlotClickedEventArgs e) {
		//TODO 
		//e.Slot.OnSlotClicked -= InstallUpgradeTo;
		currentCell.InstallUpgrade(currentCell.cell.Cell, e.SlotID, type);
		count--;
		UpdateUpgradeOverview();
		if (sender == null) {
			SpriteRenderer s = currentCell.transform.Find("UpgradeSlots/" + e.Slot).GetComponent<SpriteRenderer>();
			s.sprite = Upgrade.UpgradeGraphics[type];
			s.size = Vector2.one * 25;
		}
		else {
			e.Slot.ChangeUpgradeImage(Upgrade.UpgradeGraphics[type]);
		}
		currentCell.slotRender.color = new Color(1, 1, 1, 0.25f);
		isListeningForSlot = true;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (desc != null) {
			desc.text = FolderAccess.GetUpgradeName(type);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		if (desc != null) {
			desc.text = "";
		}
	}
}
