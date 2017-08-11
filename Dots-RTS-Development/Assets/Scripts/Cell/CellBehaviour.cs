//using System;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellBehaviour : Cell, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler {

	public static List<CellBehaviour> cellsInSelection = new List<CellBehaviour>();									//Global list of selected cells
	public static event Control.TeamChangeEventHandler TeamChanged;
	public static CellBehaviour lastEnteredCell = null;

	public bool isSelected = false;                                                                                 //Is cell in global selection ?

	public Coroutine generateCoroutine;

	public GameObject cellObj;
	public GameObject elementObj;


	//Add cell to global list
	private void Awake() {
		Control.cells.Add(this);
	}

	//Set default
	private void Start() {
		if (maxElements == 0) {
			maxElements = 50;
		}
		if (elementCount == 0) {
			elementCount = 10;
		}
		if (regenPeriod == 0) {
			regenPeriod = 0.3f;
		}
		UpdateCellInfo();
	}

	#region Cell Selection/Deselection
	//Checks the list whether the cell is already selected ? removes it : adds it
	public static void ModifySelection(CellBehaviour cell) {
		for (int i = 0; i < cellsInSelection.Count; i++) {
			if (cell == cellsInSelection[i]) {
				cellsInSelection.RemoveAt(i);
				cell.SetSelected();
				return;
			}
		}
		cellsInSelection.Add(cell);
		cell.SetSelected();
	}

	//Selects or deselects a cell
	public void SetSelected() {
		if (isSelected) {
			isSelected = false;
			elementNrDisplay.color = new Color32(255, 255, 255, 255);
			cellSelected.enabled = false;
		}
		else {
			isSelected = true;
			elementNrDisplay.color = new Color32(255, 0, 0, 255);
			cellSelected.enabled = true;
		}
	}

	//Resets Cell colour and clears the selection list
	public static void ClearSelection() {
		//print("Clearing");
		for (int i = 0; i < cellsInSelection.Count; i++) {
			cellsInSelection[i].isSelected = false;
			cellsInSelection[i].elementNrDisplay.color = new Color(1, 1, 1);
			cellsInSelection[i].cellSelected.enabled = false;
		}
		cellsInSelection.Clear();
	}
	#endregion

	//Wrapper for cell atacking
	public static void AttackWrapper(CellBehaviour target, enmTeam team) {
		if (cellsInSelection.Count != 0) {
			if ((int)team >= 2) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].AttackCell(target);
				}
			}
			else if (team == enmTeam.ALLIED) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].EmpowerCell(target);
				}
			}
			else if (team == enmTeam.NEUTRAL) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].AttackCell(target);
				}
			}
		}
		ClearSelection();
	}

	#region Attack Handling

	//Code to attack selected cell
	public void AttackCell(CellBehaviour target) {
		if (elementCount > 1) {
			int numElements = elementCount = elementCount / 2;
			for (int i = 0; i < numElements; i++) {
				Element e = Instantiate(elementObj, ElementSpawnPoint(), Quaternion.identity).GetComponent<Element>();
				e.target = target;
				e.attacker = this;
				e.team = this.cellTeam;
			}
			UpdateCellInfo();
		}
	}

	//Called when "attacking" your own cell
	public void EmpowerCell(CellBehaviour target) {
		if (elementCount > 1 && target != this) {
			int numElements = elementCount = elementCount / 2;
			for (int i = 0; i < numElements; i++) {
				Element e = Instantiate(elementObj, ElementSpawnPoint(), Quaternion.identity).GetComponent<Element>();
				e.target = target;
				e.attacker = this;
				e.team = this.cellTeam;
			}
			UpdateCellInfo();
		}
	}
	#endregion

	#region Element Damage Handling
	//Called when an element enters a cell, isAllied ? feed the cell : damage the cell
	public void DamageCell(enmTeam elementTeam, int amoutOfDamage, Upgrade.Upgrades additionalArg = 0) {

		if (cellTeam == elementTeam) {
			elementCount++;
			UpdateCellInfo();
			return;
		}
		if (additionalArg != 0) {
			bool isAffected = false;
			for (int i = 0; i < appliedDebuffs.Count; i++) {
				if (appliedDebuffs[i] == additionalArg) {
					isAffected = true;
				}
			}
			if (!isAffected) {
				if (additionalArg == Upgrade.Upgrades.SLOW_REGENERATION) {
					regenPeriod *= 2;
					appliedDebuffs.Add(additionalArg);
				}
				if (additionalArg == Upgrade.Upgrades.DOT) {
					StartCoroutine(DoT(1, 4));
					appliedDebuffs.Add(additionalArg);
				}
			}
		}
		elementCount -= amoutOfDamage;
		if (elementCount < 0) {
			if (TeamChanged != null) {
				TeamChanged(this, cellTeam, elementTeam);
			}
			if (isSelected) {
				ModifySelection(this);
			}
			elementCount = -elementCount;
			cellTeam = elementTeam;
		}
		UpdateCellInfo();
	}


	public Vector3 ElementSpawnPoint() {
		float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
		float x = Mathf.Sin(angle);
		float y = Mathf.Cos(angle);
		return new Vector3(transform.position.x + x * cellRadius * 0.70f, transform.position.y + y * cellRadius * 0.70f);
	}
	#endregion


	//Overriden function to include regeneration call
	public override void UpdateCellInfo(bool calledFromBase = false) {
		circle.sortingLayerName = "Cells";
		circle.sortingOrder = 0;


		base.UpdateCellInfo();

		if (calledFromBase == false) {

			circle.sortingOrder = 0;

			if (!isRegenerating && (cellTeam == enmTeam.ALLIED || (int)cellTeam >= 2)) {

				StartCoroutine(GenerateElements());
			}

			if (elementCount > maxElements) {
				Decay(0.5f);
			}
			if (isSelected) {
				cellSelected.enabled = true;
			}
			else {
				cellSelected.enabled = false;
			}
		}
	}

	Vector3 oldPos = Vector3.zero;
	private void FixedUpdate() {
		if (oldPos == Vector3.zero) {
			oldPos = transform.position;
		}

		Vector3 vel = transform.position - oldPos;

		if (vel != Vector3.zero) {
			UpdateCellInfo(false);
		}
	}

	private void Update() {
		if (Input.GetMouseButtonUp(0) && lastEnteredCell == this) {
			AttackWrapper(lastEnteredCell, lastEnteredCell.cellTeam);
		}
	}

	private void OnMouseOver() {
		#region Cell Debug - Change regen speed and max size by hovering over the cell and pressing "8" to increase max count or "6" to increase regenSpeed, opposite buttons decrease the values
		//Adjust max cell size
		if (Input.GetKeyDown(KeyCode.Keypad8)) {
			if (maxElements < 100) {
				maxElements++;
				print(maxElements);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad2)) {
			if (maxElements >= 2) {
				maxElements--;
				print(maxElements);
			}
		}

		//Adjust cell regeneration rate
		if (Input.GetKeyDown(KeyCode.Keypad6)) {
			if (regenPeriod < 10) {
				regenPeriod += 0.5f;
				print(regenPeriod);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			if (regenPeriod > 0.5f) {
				regenPeriod -= 0.5f;
				print(regenPeriod);
			}
		}
		#endregion
	}

	//Old Attack code
	//private void OnMouseOver() {
	//	Legacy Attack behaviour
	//		if (Input.GetMouseButtonUp(0)) {
	//		print("OnMouseOver " + gameObject.name);
	//			AttackWrapper(this, cellTeam);
	//}
	//}
	//
	//private void OnMouseEnter() {
	//print("Entering cell");
	//Legacy Attack behaviour
	//if (Input.GetMouseButton(0)) {
	//	if (cellTeam == enmTeam.ALLIED) {
	//		ModifySelection(this);
	//print("Added " + gameObject.name);
	//}
	//}
	//}
	//
	//Hides Upgrade Slots
	//private void OnMouseExit() {
	//print("Exitting Cell");
	//base.UpdateCellInfo();
	//}
	//
	//
	//Determine action depending on clicked cell's team.
	//private void OnMouseDown() {
	//print("Clicking on Cell");
	//if (cellTeam == enmTeam.ALLIED) {
	//	ModifySelection(this);
	//	//print("Added " + gameObject.name);
	//}
	//}
	//
	//private void OnMouseUp() {
	//print("Releasing button over cell");
	////print(gameObject.name);
	//if (cellTeam != enmTeam.ALLIED) {
	//	AttackWrapper(this, cellTeam);
	//}
	//else {
	//	EmpowerCell(this);
	//}
	//circle.positionCount = 0;
	//}

	public void OnPointerEnter(PointerEventData eventData) {

		Background.onReleaseClear = false;
		lastEnteredCell = this;

		if (Input.GetMouseButton(0) && cellTeam == enmTeam.ALLIED) {
			ModifySelection(this);
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		ClearSelection();
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (cellTeam == enmTeam.ALLIED) {
			ModifySelection(this);
		}
	}
}
