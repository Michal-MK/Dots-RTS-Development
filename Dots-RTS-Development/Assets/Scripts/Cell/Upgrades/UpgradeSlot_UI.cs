using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeSlot_UI : UpgradeSlot {

	private Image uiSlot;
	private Image uiSlotHighlight;

	private Sprite transparent;
	private Sprite highlight;

	public LevelEditorUI UI;

	public event EventHandler<UpgradeSlot_UI> OnUIUpgradeSlotClicked;

	//TODO a lot of hardcoded strings for scene references


	protected override void Start() {
		base.Start();
		OnSlotClicked += UpgradeSlotClicked;
		uiSlot = transform.Find("UpgradeImg").GetComponent<Image>();
		uiSlotHighlight = transform.Find("Slot Highlight").GetComponent<Image>();

		transparent = uiSlot.sprite;
		highlight = uiSlotHighlight.sprite;
		uiSlotHighlight.sprite = transparent;
	}

	private void UpgradeSlotClicked(object sender, UpgradeSlot e) {
		UI.upgradeSelector.GetComponent<UpgradeSelector>().Setup(this);
		OnUIUpgradeSlotClicked?.Invoke(this, this);
	}

	public void OnPickerPicked(object _, EditorUpgradePicker e) {
		Extensions.Find<UpgradeSelector>().Clean(this);
		Type = e.Upgrade;
		uiSlot.sprite = e.upgradeImg.sprite;
		uiSlotHighlight.GetComponent<Animator>().SetTrigger("Stop");
		uiSlotHighlight.sprite = transparent;
		Type = e.Upgrade;
	}

	public void ClearSlot() {
		uiSlot.sprite = transparent;
		Type = Upgrades.NONE;
	}

	public override void ChangeUpgradeImage(Sprite newSprite) {
		/*Pass*/
	}
}
