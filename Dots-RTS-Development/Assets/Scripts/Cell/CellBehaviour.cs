using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : Cell {

	public static List<CellBehaviour> cellsInSelection = new List<CellBehaviour>();
	public static event GameControll.TeamChangeEventHandler TeamChanged;

	public bool isSelected = false;                                                                                 //Is cell selected for attack?

	public Coroutine generateCoroutine;

	public GameObject cellObj;
	public GameObject elementObj;

	private void Awake() {
		GameControll.cells.Add(this);
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
			regenPeriod = 2f;
		}

		cellRadius = gameObject.GetComponent<CircleCollider2D>().radius * transform.localScale.x;

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
		for (int i = 0; i < cellsInSelection.Count; i++) {
			cellsInSelection[i].isSelected = false;
			cellsInSelection[i].elementNrDisplay.color = new Color(1, 1, 1);
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
				e.team = this._team;
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
				e.team = this._team;
			}
			UpdateCellInfo();
		}
	}

	//Called when an element enters a cell, isAllied ? feed the cell : damage the cell
	public void DamageCell(enmTeam elementTeam) {
		if (cellTeam == elementTeam) {
			elementCount++;
			UpdateCellInfo();
			return;
		}
		elementCount--;
		if (elementCount < 0) {
			TeamChanged(this, cellTeam, elementTeam);
			elementCount = -elementCount;
			cellTeam = elementTeam;
		}
		UpdateCellInfo();
	}


	public Vector3 ElementSpawnPoint() {
		float angle = Random.Range(0, 2 * Mathf.PI);
		float x = Mathf.Sin(angle);
		float y = Mathf.Cos(angle);
		return new Vector3(transform.position.x + x * cellRadius,transform.position.y + y * cellRadius);
	}
	#endregion


	//Overriden function to include regeneration call
	public override void UpdateCellInfo() {
		base.UpdateCellInfo();
		if (!isRegenerating && _team == enmTeam.ALLIED || !isRegenerating && (int)_team >= 2) {
			StartCoroutine(GenerateElements());
		}
		if (isSelected) {
			circle.enabled = true;
			CreateCircle_AndLine(transform.position, cellRadius, 20);
		}
		else {
			circle.enabled = false;
			circle.positionCount = 0;
		}
	}

	//Generic code to set all the values of a cell
	//public void SetCellData(Vector2 position, enmTeam team, int startingCount = 10, int maximum = 50, float regenerationFreq = 1.5f, float radius = 1) {
	//	gameObject.transform.position = position;
	//	elementCount = startingCount;
	//	maxElements = maximum;
	//	regenPeriod = regenerationFreq;
	//	cellTeam = team;
	//	cellRadius = radius;
	//}


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
			CreateCircle_AndLine(transform.position, cellRadius, 20);

		}
	}

	//
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


	//
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
		//um.upgradeSlotsRenderer.color = new Color32(255, 255, 255, 0);
	}


	//Determine action depending on clicked cell's team.
	private void OnMouseDown() {
		if (cellTeam == enmTeam.ALLIED) {
			ModifySelection(this);
			print("Added " + gameObject.name);
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

	public void CreateCircle_AndLine(Vector3 _position, float _r, int segments) {

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

	//	private void Update() {
	//		if (isSelected) {
	//			lineToMouse.SetPosition(0, transform.position);
	//			lineToMouse.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));

	//		}
	//	}
}
