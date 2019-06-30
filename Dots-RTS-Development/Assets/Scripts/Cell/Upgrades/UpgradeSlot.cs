using System;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * This script is used for data storage, it is included in upgrade prefabs in editor,
 * as well as in play scene in the upgrade panel, pretty much every "TeamBox" upgrade has a script that derives from this.
 */

public abstract class UpgradeSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler {

	public int SlotID { get; set; }
	public Upgrades Type { get; set; } = Upgrades.NONE;

	public event EventHandler<UpgradeSlot> OnSlotClicked;

	protected virtual void Start() {
		SlotID = int.Parse(transform.name);
	}

	public abstract void ChangeUpgradeImage(Sprite newSprite);

	public void OnPointerClick(PointerEventData eventData) {
		OnSlotClicked?.Invoke(this, this);
	}

	public void OnPointerDown(PointerEventData eventData) { }
}
