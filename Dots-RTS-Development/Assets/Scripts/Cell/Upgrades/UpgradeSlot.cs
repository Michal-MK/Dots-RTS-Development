using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour,IPointerClickHandler, IPointerDownHandler {
	public int slot;
	public SpriteRenderer selfSprite;
	public delegate void OnUpgradeSlotClick(UpgradeSlot sender, int slot);
	public static event OnUpgradeSlotClick OnSlotClicked;

	void Start() {
        slot = int.Parse(transform.name);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (OnSlotClicked != null) {
			OnSlotClicked(this, slot);
		}

	}
	public void OnPointerDown(PointerEventData eventData) {
		//Has to be implemented for some reason.
	}
}
