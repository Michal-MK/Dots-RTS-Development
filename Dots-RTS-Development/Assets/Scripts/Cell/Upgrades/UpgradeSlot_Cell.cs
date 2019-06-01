using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot_Cell : UpgradeSlot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

	public SpriteRenderer selfSprite;
	public SpriteRenderer clearUpgrade;

	private UM_Editor uManager;

	protected override void Start() {
		base.Start();
		if (!IsSubscribed) {
			OnSlotClicked += UpgradeSlot_OnSlotClicked;
		}
		uManager = transform.parent.parent.GetComponent<UM_Editor>(); //What?? 
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		OnSlotClicked -= UpgradeSlot_OnSlotClicked;
	}

	private void UpgradeSlot_OnSlotClicked(object sender, OnUpgradeSlotClickedEventArgs e) {
		if (e.Slot == this) {
			print($"Clicked on slot {e.Slot} invoked by {e.SlotID} in {transform.parent.name}");
			selfSprite.sprite = null;
			clearUpgrade.color = new Color(1, 1, 1, 0);
			Type = Upgrades.NONE;
			uManager.upgrades[e.SlotID] = Upgrades.NONE;
		}
	}

	protected override void ClearUpgradeSlot_OnSlotClear(object sender, UpgradeSlot clicked) {
		if (clicked == this) {
			base.ClearUpgradeSlot_OnSlotClear(sender, clicked);
			print($"Clered slot {SlotID} in cell {transform.parent.name}");
		}
	}

	public override void ChangeUpgradeImage(Sprite newSprite) {
		selfSprite.sprite = newSprite;
		selfSprite.size = Vector2.one * 25;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (Type != Upgrades.NONE) {
			clearUpgrade.color = new Color(1, 1, 1, 1);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		clearUpgrade.color = new Color(1, 1, 1, 0);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (Type != Upgrades.NONE) {
			uManager.upgrades[SlotID] = Type = Upgrades.NONE;
			clearUpgrade.color = new Color(1, 1, 1, 0);
			selfSprite.sprite = null;
		}
		else {
			//highlightedSlot = this;
			UpgradePickerInstance.OnPickerClicked += InstallUpgradeDirectly;
			UpgradePickerInstance.SetSelected(this);
			UI_Manager.AddWindow(UI_ReferenceHolder.LE_upgradePickerPanel);
		}
	}

	private void InstallUpgradeDirectly(object senderm,OnPickerClickedEventArgs e) {

		UpgradePickerInstance.OnPickerClicked -= InstallUpgradeDirectly;
		if (e.Slot == this) {
			print("Selected " + e.Slot.SlotID + " This " + SlotID);
			uManager.upgrades[SlotID] = Type = e.Instance.upgrade;
			selfSprite.sprite = Upgrade.UPGRADE_GRAPHICS[Type];
			selfSprite.size = Vector2.one * 25f;
		}
		UI_Manager.CloseMostRecent();
		//highlightedSlot = null; //TODO
	}

	public void OnPointerDown(PointerEventData eventData) {
		//Has to be implemented
	}
}
