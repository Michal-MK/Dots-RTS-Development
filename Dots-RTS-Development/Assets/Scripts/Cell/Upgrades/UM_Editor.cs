using UnityEngine;

public class UM_Editor : Upgrade_Manager {

	#region Unity UI

	private readonly SpriteRenderer[] uiUpgradeSlotsSprite = new SpriteRenderer[8];
	private readonly BoxCollider2D[] uiUpgradeSlotColider = new BoxCollider2D[8];
	private SpriteRenderer upgradeSlotFields;

	#endregion

	public readonly UpgradeSlot_Cell[] upgradeSlots = new UpgradeSlot_Cell[8];

	public void SetupUpgrades(LevelEditorUI UI) {
		GameObject uiUpgrades = UI.upgradePickerButtons;

		int i = 0;
		foreach (UpgradeSlot slot in uiUpgrades.GetComponentsInChildren<UpgradeSlot>()) {
			uiUpgradeSlotsSprite[i] = slot.gameObject.GetComponent<SpriteRenderer>();
			uiUpgradeSlotColider[i] = slot.gameObject.GetComponent<BoxCollider2D>();
			i++;
			slot.OnSlotClicked += Slot_OnSlotClicked;
		}
	}

	private void Slot_OnSlotClicked(object sender, UpgradeSlot e) {
		print($"I belong to {e.transform.parent.parent.name} you clicked slot {e.SlotID}");
	}

	private void OnDestroy() {
		foreach (UpgradeSlot_Cell slot in upgradeSlots) {
			slot.OnSlotClicked -= Slot_OnSlotClicked;
		}
	}

	protected override void UpgradePreinstallSprites() {
		for (int i = 0; i < upgradeSlots.Length; i++) {
			if (upgrades[i] != Upgrades.NONE) {
				upgradeSlots[i].selfSprite.sprite = Upgrade.UpgradeGraphics[upgrades[i]];
				upgradeSlots[i].selfSprite.size = Vector2.one * 25f;
			}
		}
	}


	public void ToggleUpgradeInteraction(bool isOn) {
		foreach (UpgradeSlot_Cell slot in upgradeSlots) {
			slot.GetComponent<BoxCollider2D>().enabled = isOn;
		}
		upgradeSlotFields.color = isOn ? new Color(1, 1, 1, 0.4f) : new Color(1, 1, 1, 0f);
	}
}
