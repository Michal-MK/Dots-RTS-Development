using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSlotState : MonoBehaviour {

	public bool isUpgraded = false;
	public BoxCollider2D col;

	public CellBehaviour cb;
	public Upgrade_Manager um;
	private Upgrade_Manager.enmUpgrade _current = Upgrade_Manager.enmUpgrade.NONE;

	private void Awake() {
		EditCell.EditModeChanged += EditCell_EditModeChanged;
	}
	private void OnDestroy() {
		EditCell.EditModeChanged -= EditCell_EditModeChanged;
	}

	private void EditCell_EditModeChanged(EditCell sender) {
		if (!sender.isEditing) {
			col.enabled = false;
		}
		else {
			col.enabled = true;
		}
	}


	//Returns this UpgradeSlot's installed upgrade
	public Upgrade_Manager.enmUpgrade installedUpgrade {
		get { return _current; }
		set { _current = value; }
	}

	//Show upgrade slots on enter
	private void OnMouseEnter() {
		if (cb != null) {
			if (cb.cellTeam == Cell.enmTeam.ALLIED) {
				um.upgradeSlotsRenderer.color = new Color32(255, 255, 255, 255);
				foreach(UpgradeSlotState s in um.slots) {
					print(s.gameObject.name);
					s.col.enabled = true;
				}
			}
		}
	}

	//Display info about installed upgrades in slots
	private void OnMouseOver() {
		if (Input.GetMouseButtonDown(1)) {
			print("Slot ID: " + um.SlotID(this) + " Installed upgrade: " + installedUpgrade);
		}
	}

}
