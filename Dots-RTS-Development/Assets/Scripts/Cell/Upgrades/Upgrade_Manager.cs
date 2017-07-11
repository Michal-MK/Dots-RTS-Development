using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Manager : MonoBehaviour {

	public UpgradeSlotState[] slots = new UpgradeSlotState[8];

	public SpriteRenderer upgradeSlotsRenderer;
	public CellBehaviour bScript;
	public Cell cScript;
	public CircleCollider2D col;

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
	public enum enmUpgrade {
		NONE,
		ELEMENT_MOVE_SPEED,
		GENERATION_SPEED,
		MAX_CAPACITY,
		CRITICAL_CHANCE,
	}

	private int[] _upgrades = new int[8];

	public int[] ApplyUpgrades() {
		for (int i = 0; i < _upgrades.Length; i++) {
			_upgrades[i] = (int)slots[i].installedUpgrade;
		}
		return _upgrades;
	}

	/// <summary>
	/// Upgrades of the selected cell
	/// </summary>
	public int[] upgrades {
		get { return _upgrades; }
		set { _upgrades = value; }
	}


	private void Awake() {
		EditCell.changedASelectionOfCell += EditCell_EditModeChanged;
	}
	private void OnDestroy() {
		EditCell.changedASelectionOfCell -= EditCell_EditModeChanged;
	}

	private void EditCell_EditModeChanged(EditCell sender) {
		if (!sender.isCellSelected) {
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
				upgradeSlotsRenderer.color = new Color32(255, 255, 255, 255);
				for (int i = 0; i < slots.Length; i++) {
					slots[i].col.enabled = true;
				}
			}
			else {
				for (int i = 0; i < slots.Length; i++) {
					slots[i].col.enabled = false;
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

	//Installs Upgrade onto specified slot
	public void InstallUpgrade(enmUpgrade type, enmSlot number) {
		slots[(int)number].installedUpgrade = type;
	}

	//Removes Upgrade from a specified slot
	public void UninstallUpgrade(enmSlot number) {
		slots[(int)number].installedUpgrade = 0;
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
