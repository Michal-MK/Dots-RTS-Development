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
		UpgradeInstances[SlotID] = Type;
	}

	private void WindowClosed(Window changed) {
		if(changed.window.name == "UPGRADE_Selection_To_UI") {
			//highlightedSlot = null;//TODO
			uiSlotHighlight.sprite = transparent;
			uiSlotHighlight.GetComponent<Animator>().SetTrigger("Stop");
		}
	}

	protected void ClearUpgradeSlot_OnSlotClear(UpgradeSlot clicked) {
		if (clicked == this) {
			ClearUpgradeSlot_OnSlotClear(clicked);
			uiSlot.sprite = transparent;
			UpgradeInstances[SlotID] = Type = Upgrades.NONE;
		}
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		UpgradePickerInstance.OnPickerClicked -= UpgradePickerInstance_OnPickerClicked;
		ClearUpgradeSlot.OnSlotClear -= ClearUpgradeSlot_OnSlotClear;
	}

	public void OnPointerClick(PointerEventData eventData) {
		OnUIUpgradeSlotClicked?.Invoke(this);
	}

	private void UpgradePickerInstance_OnPickerClicked(object sendern, OnPickerClickedEventArgs e) {
		if (e.Slot == this) {
			Type = e.Instance.upgrade;
			uiSlot.sprite = e.Instance.upgradeImg.sprite;
			uiSlotHighlight.GetComponent<Animator>().SetTrigger("Stop");
			uiSlotHighlight.sprite = transparent;
			UpgradeInstances[SlotID] = e.Instance.upgrade;
			UI_ReferenceHolder.LE_upgradePickerPanel.SetActive(false);
		}
	}

	public override void ChangeUpgradeImage(Sprite newSprite) {
		throw new System.NotImplementedException();
	}
}
