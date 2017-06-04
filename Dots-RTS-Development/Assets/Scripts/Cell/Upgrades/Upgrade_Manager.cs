using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Manager : MonoBehaviour {

	public GameObject[] slots = new GameObject[8];

	public SpriteRenderer upgradeSlotsRenderer;
	public CellBehaviour cellScript;

	public enum Slot {
		FIRST,
		SECOND,
		THIRD,
		FOURTH,
		FIFTH,
		SIXTH,
		SEVENTH,
		EIGHTH,
	}

	public enum Upgrade {
		NONE,
		ELEMENT_MOVE_SPEED,
		GENERATION_SPEED,
		MAX_CAPACITY,
		CRITICAL_CHANCE,
	}

	//Called when you mouse over the cell
	private void OnMouseEnter() {
		if (cellScript.cellTeam == CellBehaviour.enmTeam.ALLIED) {
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


	//Called when you leave the cell
	private void OnMouseExit() {
		for (int i = 0; i < slots.Length; i++) {
			upgradeSlotsRenderer.color = new Color32(255, 255, 255, 0);
		}
	}

	//Installs Upgrade onto specifiad slot
	public void InstallUpgrade(Upgrade type, Slot number) {
		slots[(int)number].GetComponent<UpgradeSlotState>().current = type;
	}

	//Removes Upgrade from a specified slot
	public void UninstallUpgrade(Slot number) {
		slots[(int)number].GetComponent<UpgradeSlotState>().current = 0;
	}
}
