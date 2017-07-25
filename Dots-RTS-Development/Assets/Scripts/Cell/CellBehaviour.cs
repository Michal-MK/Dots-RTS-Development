using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : Cell {

	public static List<CellBehaviour> cellsInSelection = new List<CellBehaviour>();
	/// <summary>
	/// Called when cell Changes team after collision with an element.
	/// </summary>
	public static event Control.TeamChangeEventHandler TeamChanged;

	public bool isSelected = false;                                                                                 //Is cell selected for attack?

	public Coroutine generateCoroutine;

	public GameObject cellObj;
	public GameObject elementObj;

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

		cellRadius = col.radius * transform.localScale.x;
		UpdateCellInfo();
	}


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

	//Resets Cell colour and clears the selection list
	public static void ClearSelection() {
		print("Clearing");
		for (int i = 0; i < cellsInSelection.Count; i++) {
			cellsInSelection[i].isSelected = false;
			cellsInSelection[i].elementNrDisplay.color = new Color(1, 1, 1);
			cellsInSelection[i].circle.enabled = false;
			cellsInSelection[i].circle.positionCount = 0;
		}
		cellsInSelection.Clear();
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
	public void DamageCell(enmTeam elementTeam, int amoutOfDamage, Element.enmDebuffs additionalArg = 0) {

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
				if(additionalArg == Element.enmDebuffs.SLOW_REGENERATION) {
					regenPeriod *= 2;
					appliedDebuffs.Add(additionalArg);
				}
				if (additionalArg == Element.enmDebuffs.DOT) {
					StartCoroutine(DoT(1,4));
					appliedDebuffs.Add(additionalArg);
				}
			}
		}
		elementCount -= amoutOfDamage;
		if (elementCount < 0) {
			if (TeamChanged != null) {
				TeamChanged(this, cellTeam, elementTeam);
			}
			elementCount = -elementCount;
			cellTeam = elementTeam;
		}
		UpdateCellInfo();
	}


	public Vector3 ElementSpawnPoint() {
		float angle = Random.Range(0, 2 * Mathf.PI);
		float x = Mathf.Sin(angle);
		float y = Mathf.Cos(angle);
		return new Vector3(transform.position.x + x * cellRadius * 0.70f, transform.position.y + y * cellRadius* 0.70f);
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
				print("Decaying :" + elementCount + " is greater than " + maxElements);
				Decay(0.5f);
			}
			if (isSelected) {
				circle.enabled = true;
				//circle.positionCount = 0;
				CreateCircle(transform.position, cellRadius, 30);
			}
			else {
				circle.enabled = false;
				circle.positionCount = 0;
			}
		}
	}


	//Selects or deselects a cell
	public void SetSelected() {
		if (isSelected) {
			isSelected = false;
			elementNrDisplay.color = new Color32(255, 255, 255, 255);
			circle.enabled = false;

		}
		else {
			isSelected = true;
			elementNrDisplay.color = new Color32(255, 0, 0, 255);
			circle.enabled = true;
			CreateCircle(transform.position, cellRadius, 30);

		}
	}
	Vector3 oldPos;
	private void FixedUpdate() {
		//Does not work for some reason..
		//if(rg.velocity != Vector2.zero) {
		//	UpdateCellInfo(false);
		//}
		if(oldPos == null) {
			oldPos = transform.position;
		}
		Vector3 vel = transform.position - oldPos;

		if(vel != Vector3.zero) {
			UpdateCellInfo(false);
		}
	}

	private void OnMouseOver() {
		if (Input.GetMouseButtonDown(1)) {
			print("Edit");

		}
		//Legacy Attack behaviour
		if (Input.GetMouseButtonUp(0)) {
			AttackWrapper(this, cellTeam);
		}

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

	private void OnMouseEnter() {
		//Legacy Attack behaviour
		if (Input.GetMouseButton(0)) {
			if (cellTeam == enmTeam.ALLIED) {
				ModifySelection(this);
				print("Added " + gameObject.name);
			}
		}
	}

	//Hides Upgrade Slots
	private void OnMouseExit() {
		base.UpdateCellInfo();
	}


	//Determine action depending on clicked cell's team.
	private void OnMouseDown() {
		if (cellTeam == enmTeam.ALLIED) {
			ModifySelection(this);
			//print("Added " + gameObject.name);
		}
	}

	private void OnMouseUp() {
		//print(gameObject.name);
		if (cellTeam != enmTeam.ALLIED) {
			AttackWrapper(this, cellTeam);
		}
		else {
			EmpowerCell(this);
		}
		circle.positionCount = 0;

	}

	public void CreateCircle(Vector3 _position, float _r, int segments) {

		circle.positionCount = segments + 1;
		circle.useWorldSpace = true;

		float x;
		float y;
		float z = 0f;

		float angle = 20f;

		for (int i = 0; i < (segments + 1); i++) {
			x = Mathf.Sin(Mathf.Deg2Rad * angle) * _r;
			y = Mathf.Cos(Mathf.Deg2Rad * angle) * _r;

			circle.SetPosition(i, new Vector3(_position.x + x, _position.y + y, z));

			angle += (360f / segments);
		}
	}
}
