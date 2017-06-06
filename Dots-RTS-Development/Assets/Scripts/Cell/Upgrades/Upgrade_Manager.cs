using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Manager : Upgrade {

	public UpgradeSlotState[] slots = new UpgradeSlotState[8];

	public SpriteRenderer upgradeSlotsRenderer;
	public CellBehaviour bScript;
	public Cell cScript;


	public enum enmSlot {
		NULL = -1,
		FIRST,
		SECOND,
		THIRD,
		FOURTH,
		FIFTH,
		SIXTH,
		SEVENTH,
		EIGHTH,
	}

	private void Awake() {
		EditCell.EditModeChanged += EditCell_EditModeChanged;
	}
	private void OnDestroy() {
		EditCell.EditModeChanged -= EditCell_EditModeChanged;
	}

	private void EditCell_EditModeChanged(EditCell sender) {
		if (!sender.isEditing) {
			for (int i = 0; i < slots.Length; i++) {
				upgradeSlotsRenderer.color = new Color32(255, 255, 255, 0);
				slots[i].gameObject.GetComponent<BoxCollider2D>().enabled = false;
			}
		}
		else {
			for (int i = 0; i < slots.Length; i++) {
				upgradeSlotsRenderer.color = new Color32(255, 255, 255, 255);
				slots[i].gameObject.GetComponent<BoxCollider2D>().enabled = true;
			}
		}
	}

	//Shows upgrade slots if the cell is allied
	private void OnMouseEnter() {
		if (bScript != null) {
			if (bScript.cellTeam == Cell.enmTeam.ALLIED) {
				for (int i = 0; i < slots.Length; i++) {
					upgradeSlotsRenderer.color = new Color32(255, 255, 255, 255);
					slots[i].gameObject.GetComponent<BoxCollider2D>().enabled = true;
				}
			}
			else {
				for (int i = 0; i < slots.Length; i++) {
					slots[i].gameObject.GetComponent<BoxCollider2D>().enabled = false;
				}
			}
		}
	}

	//Makes upgrade slots invisible
	private void OnMouseExit() {
		if (bScript != null) {
			for (int i = 0; i < slots.Length; i++) {
				upgradeSlotsRenderer.color = new Color32(255, 255, 255, 0);
			}
		}
	}

	//Installs Upgrade onto specifiad slot
	public void InstallUpgrade(enmUpgrade type, enmSlot number) {
		slots[(int)number].current = type;
	}

	//Removes Upgrade from a specified slot
	public void UninstallUpgrade(enmSlot number) {
		slots[(int)number].current = 0;
	}

	//Returns slots index in Upgrade Manager array as an enum
	public enmSlot SlotID(UpgradeSlotState slotHolder) {

		for (int i = 0; i < slots.Length; i++) {
			if (slotHolder == slots[i]) {
				return (enmSlot)i;
			}
		}
		return (enmSlot)(-1);
	}
}
