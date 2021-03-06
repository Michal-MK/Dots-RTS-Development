﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class UpgradeSlot_Cell : UpgradeSlot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

	public SpriteRenderer selfSprite;
	public SpriteRenderer clearUpgrade;

	UM_Editor uManager;

	//public delegate void CellUpgradeClick(UpgradeSlot_Cell sender);
	//public static event CellUpgradeClick OnCellUpgradeSlotClicked;


	private static bool isSubscribed = false;
	// Use this for initialization
	protected override void Start() {
		base.Start();
		if (!isSubscribed) {
			OnSlotClicked += UpgradeSlot_OnSlotClicked;
		}
		uManager = transform.parent.parent.GetComponent<UM_Editor>();
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		OnSlotClicked -= UpgradeSlot_OnSlotClicked;
	}



	private void UpgradeSlot_OnSlotClicked(UpgradeSlot sender, int slot) {
		if (sender == this) {
			print("Clicked on slot " + slot + " invoked by " + sender.getSlotID + " in " + sender.transform.parent.name);
			selfSprite.sprite = null;
			clearUpgrade.color = new Color(1, 1, 1, 0);
			type = Upgrade.Upgrades.NONE;
			uManager.upgrades[slot] = Upgrade.Upgrades.NONE;
		}
	}

	protected override void ClearUpgradeSlot_OnSlotClear(UpgradeSlot clicked) {
		if (clicked == this) {
			base.ClearUpgradeSlot_OnSlotClear(clicked);
			print("Clered slot " + getSlotID + " On Cell");
		}
	}

	public override void ChangeUpgradeImage(Sprite newSprite) {
		selfSprite.sprite = newSprite;
		selfSprite.size = Vector2.one * 25;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		if (type != Upgrade.Upgrades.NONE) {
			clearUpgrade.color = new Color(1, 1, 1, 1);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		clearUpgrade.color = new Color(1, 1, 1, 0);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if(type != Upgrade.Upgrades.NONE) {
			uManager.upgrades[getSlotID] = type = Upgrade.Upgrades.NONE;
			clearUpgrade.color = new Color(1, 1, 1, 0);
			selfSprite.sprite = null;
		}
		else {
			highlightedSlot = this;
			UpgradePickerInstance.OnPickerClicked += InstallUpgradeDirectly;
			UI_Manager.AddWindow(UI_ReferenceHolder.LE_upgradePickerPanel);
		}
	}

	private void InstallUpgradeDirectly(UpgradeSlot selected, UpgradePickerInstance sender) {

		UpgradePickerInstance.OnPickerClicked -= InstallUpgradeDirectly;
		if (selected == this) {
			print("Selected " + selected.getSlotID + " This " + getSlotID);
			uManager.upgrades[getSlotID] = type = sender.upgrade;
			selfSprite.sprite = Upgrade.UPGRADE_GRAPHICS[type];
			selfSprite.size = Vector2.one * 25f;
		}
		UI_Manager.CloseMostRecent();
		highlightedSlot = null;
	}

	public void OnPointerDown(PointerEventData eventData) {
		//Has to be implemented
	}
}
