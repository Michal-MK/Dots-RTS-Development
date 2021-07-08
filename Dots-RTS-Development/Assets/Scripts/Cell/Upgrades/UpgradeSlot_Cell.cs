using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot_Cell : UpgradeSlot, IPointerEnterHandler, IPointerExitHandler {

	public SpriteRenderer selfSprite;
	public SpriteRenderer clearUpgrade;

	private EditorUpgradeManager uManager;

	protected override void Start() {
		base.Start();
		OnSlotClicked += UpgradeSlotClicked;
		uManager = transform.parent.parent.GetComponent<EditorUpgradeManager>(); //What?? 
	}

	private void UpgradeSlotClicked(object sender, UpgradeSlot e) {
		if (Type != Upgrades.None) {
			print($"Clicked on slot {e.gameObject.name} invoked by {e.SlotID} in {transform.parent.name}");
			uManager.InstalledUpgrades[SlotID] = Type = Upgrades.None;
			clearUpgrade.color = new Color(1, 1, 1, 0);
			selfSprite.sprite = null;
		}
		else {
			Extensions.Find<LevelEditorUI>().upgradeSelector.GetComponent<UpgradeSelector>().Setup(this);
		}
	}

	protected void OnDestroy() {
		OnSlotClicked -= UpgradeSlotClicked;
	}

	public override void ChangeUpgradeImage(Sprite newSprite) {
		selfSprite.sprite = newSprite;
		selfSprite.size = Vector2.one * 25;
	}

	public void InstallUpgradeDirectly(object _, EditorUpgradePicker e) {
		print("Selected " + e.Upgrade);
		uManager.InstalledUpgrades[SlotID] = Type = e.Upgrade;
		selfSprite.sprite = Upgrade.UpgradeGraphics[Type];
		selfSprite.size = Vector2.one * 25f;
		Extensions.Find<LevelEditorUI>().upgradeSelector.GetComponent<UpgradeSelector>().Clean(this);
	}

	#region Pointer events

	public void OnPointerEnter(PointerEventData eventData) {
		if (Type != Upgrades.None) {
			clearUpgrade.color = new Color(1, 1, 1, 1);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		clearUpgrade.color = new Color(1, 1, 1, 0);
	}

	#endregion
}
