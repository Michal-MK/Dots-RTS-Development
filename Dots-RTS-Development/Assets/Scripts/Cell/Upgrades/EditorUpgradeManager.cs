using UnityEngine;

public class EditorUpgradeManager : UpgradeManager {

	#region Unity UI

	public SpriteRenderer[] uiUpgradeSlotsSprite = new SpriteRenderer[8];
	public BoxCollider2D[] uiUpgradeSlotColider = new BoxCollider2D[8];
	public SpriteRenderer upgradeSlotFields;

	#endregion

	public UpgradeSlot_Cell[] upgradeSlots = new UpgradeSlot_Cell[8];

	public void SetupUpgrades(LevelEditorUI ui) {
		GameObject uiUpgrades = ui.upgradePickerButtons;

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
			if (InstalledUpgrades[i] == Upgrades.None) continue;
			upgradeSlots[i].selfSprite.sprite = Upgrade.UpgradeGraphics[InstalledUpgrades[i]];
			upgradeSlots[i].selfSprite.size = Vector2.one * 25f;
		}
	}


	public void ToggleUpgradeInteraction(bool isOn) {
		foreach (UpgradeSlot_Cell slot in upgradeSlots) {
			slot.GetComponent<BoxCollider2D>().enabled = isOn;
		}
		upgradeSlotFields.color = isOn ? new Color(1, 1, 1, 0.4f) : new Color(1, 1, 1, 0f);
	}
}
