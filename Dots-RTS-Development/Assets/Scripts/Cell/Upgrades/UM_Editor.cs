using UnityEngine;

public class UM_Editor : Upgrade_Manager {

	#region Unity UI

	private readonly SpriteRenderer[] ui_upgradeSlotsSprite = new SpriteRenderer[8];
	private readonly BoxCollider2D[] ui_upgradeSlotColider = new BoxCollider2D[8];
	public SpriteRenderer upgradeSlotFields;

	#endregion

	private EditCell currentCell;
	public UpgradeSlot[] upgradeSlots = new UpgradeSlot[8];

	private void Start() {
		GameObject uiUpgrades = UI_ReferenceHolder.LE_cellPanel.transform.parent.Find("UI_Upgrades").gameObject;

		int i = 0;
		foreach (UpgradeSlot slot in uiUpgrades.GetComponentsInChildren<UpgradeSlot>()) {
			ui_upgradeSlotsSprite[i] = slot.gameObject.GetComponent<SpriteRenderer>();
			ui_upgradeSlotColider[i] = slot.gameObject.GetComponent<BoxCollider2D>();
			i++;
			slot.OnSlotClicked += Slot_OnSlotClicked;
		}
		currentCell = GetComponent<EditCell>();
	}

	private void Slot_OnSlotClicked(object sender, OnUpgradeSlotClickedEventArgs e) {
		print($"I belong to {(sender as UpgradeSlot).transform.parent.parent.name} you clicked slot {e.SlotID}");
	}

	private void OnDestroy() {
		foreach (UpgradeSlot slot in upgradeSlots) {
			slot.OnSlotClicked -= Slot_OnSlotClicked;
		}
	}


	public void ToggleUpgradeInteraction(bool isOn) {
		for (int i = 0; i < upgradeSlots.Length; i++) {
			upgradeSlots[i].GetComponent<BoxCollider2D>().enabled = isOn;
		}
		if (isOn) {
			upgradeSlotFields.color = new Color(1, 1, 1, 0.4f);
		}
		else {
			upgradeSlotFields.color = new Color(1, 1, 1, 0f);
		}
	}
}
