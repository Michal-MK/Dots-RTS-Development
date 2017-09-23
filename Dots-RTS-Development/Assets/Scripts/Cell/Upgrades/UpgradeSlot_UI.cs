using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeSlot_UI : UpgradeSlot, IPointerClickHandler {

	private Image uiSlot;
	private Image uiSlotHighlight;

	private Sprite transparent;
	private Sprite highlight;

	private static bool isSubscribed = false;

	public delegate void UiUpgradeClick(UpgradeSlot_UI slot);
	public static event UiUpgradeClick OnUIUpgradeSlotClicked;

	// Use this for initialization
	protected override void Start() {
		base.Start();
		if (!isSubscribed) {
			UpgradePickerInstance.OnPickerClicked += UpgradePickerInstance_OnPickerClicked;
			ClearUpgradeSlot.OnSlotClear += ClearUpgradeSlot_OnSlotClear;
			UI_Manager.OnWindowClose += WindowClosed;
		}
		uiSlot = transform.Find("UpgradeImg").GetComponent<Image>();
		uiSlotHighlight = transform.Find("Slot Highlight").GetComponent<Image>();

		transparent = uiSlot.sprite;
		highlight = uiSlotHighlight.sprite;
		uiSlotHighlight.sprite = transparent;
		instances[_slot] = _type;
	}

	private void WindowClosed(Window changed) {
		if(changed.window.name == "UPGRADE_Selection_To_UI") {
			highlightedSlot = null;
			uiSlotHighlight.sprite = transparent;
			uiSlotHighlight.GetComponent<Animator>().SetTrigger("Stop");
		}
	}

	protected override void ClearUpgradeSlot_OnSlotClear(UpgradeSlot clicked) {
		if (clicked == this) {
			base.ClearUpgradeSlot_OnSlotClear(clicked);
			uiSlot.sprite = transparent;
			instances[getSlotID] = _type = Upgrade.Upgrades.NONE;
		}
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		UpgradePickerInstance.OnPickerClicked -= UpgradePickerInstance_OnPickerClicked;
		ClearUpgradeSlot.OnSlotClear -= ClearUpgradeSlot_OnSlotClear;
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (OnUIUpgradeSlotClicked != null) {
			OnUIUpgradeSlotClicked(this);
			//print("Clicked on UI Slot " + getSlotID);
		}

		if (highlightedSlot != null) {
			UI_Manager.CloseMostRecent();
			return;
		}
		else { 
			highlightedSlot = this;
			//print("Clicked slot " + getSlotID + " my parent is " + transform.parent.name);
			uiSlotHighlight.sprite = highlight;
			uiSlotHighlight.GetComponent<Animator>().SetTrigger("Animate");
			uiSlotHighlight.GetComponent<Animator>().ResetTrigger("Stop");
			UI_Manager.AddWindow(UI_ReferenceHolder.LE_upgradePickerPanel);
		}
	}

	private void UpgradePickerInstance_OnPickerClicked(UpgradeSlot highlighted, UpgradePickerInstance sender) {
		if (highlighted == this) {
			type = sender.upgrade;
			uiSlot.sprite = sender.upgradeImg.sprite;
			highlightedSlot = null;
			uiSlotHighlight.GetComponent<Animator>().SetTrigger("Stop");
			uiSlotHighlight.sprite = transparent;
			instances[getSlotID] = sender.upgrade;
			UI_ReferenceHolder.LE_upgradePickerPanel.SetActive(false);
			//print("Clicked upgrade picker " + sender.upgrade);
		}
	}
}
