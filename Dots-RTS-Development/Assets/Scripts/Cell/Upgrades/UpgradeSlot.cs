using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour,IPointerClickHandler, IPointerDownHandler {

	private bool _isOnCell;
	private Sprite transparent;
	private Sprite highlight;
	private Image uiSlot;
	private Image uiSlotHighlight;

	public Upgrade.Upgrades type = Upgrade.Upgrades.NONE;
	public SpriteRenderer selfSprite;
	public int slot;


	public delegate void OnUpgradeSlotClick(UpgradeSlot sender, int slot);
	public static event OnUpgradeSlotClick OnSlotClicked;

	private static UpgradeSlot highlightedSlot;
	
	private static bool isSubscribed = false;

	void Start() {
		if (transform.parent.name != "UPGRADE_Grid") {
			slot = int.Parse(transform.name);

			if (!isSubscribed) {
				ClearUpgradeSlot.OnSlotClear += ClearUpgradeSlot_OnSlotClear;
				UpgradePickerInstance.OnPickerClicked += UpgradePickerInstance_OnPickerClicked;
			}

			if (transform.parent.name == "UpgradeSlots") {
				_isOnCell = true;
			}
			else {
				_isOnCell = false;
				uiSlot = transform.Find("UpgradeImg").GetComponent<Image>();
				uiSlotHighlight = transform.Find("Slot Highlight").GetComponent<Image>();

				transparent = uiSlot.sprite;
				highlight = uiSlotHighlight.sprite;
				uiSlotHighlight.sprite = transparent;
			}
		}
	}

	private void OnDestroy() {
		ClearUpgradeSlot.OnSlotClear -= ClearUpgradeSlot_OnSlotClear;
		UpgradePickerInstance.OnPickerClicked -= UpgradePickerInstance_OnPickerClicked;
	}

	private void UpgradePickerInstance_OnPickerClicked(UpgradeSlot clicked, UpgradePickerInstance sender) {
		if(clicked == this) {
			uiSlot.sprite = sender.upgradeImg.sprite;
			highlightedSlot = null;
			uiSlotHighlight.GetComponent<Animator>().SetTrigger("Stop");
			uiSlotHighlight.sprite = transparent;
			UI_ReferenceHolder.LE_upgradePickerPanel.SetActive(false);
		}
	}

	private void ClearUpgradeSlot_OnSlotClear(UpgradeSlot clicked) {
		if(clicked == this) {
			uiSlot.sprite = transparent;
			type = Upgrade.Upgrades.NONE;
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (OnSlotClicked != null) {
			OnSlotClicked(this, slot);
		}
		highlightedSlot = this;
		print("Clicked slot " + slot + " my parent is " + transform.parent.name);
		uiSlotHighlight.sprite = highlight;
		uiSlotHighlight.GetComponent<Animator>().SetTrigger("Animate");
		UI_ReferenceHolder.LE_upgradePickerPanel.SetActive(true);
	}
	public void OnPointerDown(PointerEventData eventData) {
		//Has to be implemented for some reason.
	}


	public bool isOnCell {
		get { return _isOnCell; }
	}

	public static UpgradeSlot getHighlightedSlot {
		get { return highlightedSlot; }
	}
}
