using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : Cell {

	public static List<CellBehaviour> cellsInSelection = new List<CellBehaviour>();
	public static event GameControll.TeamChangeEventHandler TeamChanged;

	public bool isSelected = false;                                                             //Is cell selected for attack?

	public Coroutine generateCoroutine;

	public TextMesh elementNrDisplay;
	public MeshRenderer textRenderer;
	public SpriteRenderer cellSprite;
	public GameObject cellObj;
	public GameObject elementObj;

	private void Awake() {
		GameControll.cells.Add(this);
	}

	private void Start() {
		maxElements = 50;
		elementCount = 10;
		regenFrequency = 1.5f;
		cellRadius = gameObject.GetComponent<CircleCollider2D>().radius * transform.localScale.x;
		UpdateCellInfo();
	}


	//Checks the list whether the cell is already selected ? removes it : adds it
	public static void ModifySelection(CellBehaviour cell) {
		for (int i = 0; i < cellsInSelection.Count; i++) {
			if (cell == cellsInSelection[i]) {
				cellsInSelection.RemoveAt(i);
				cell.SetSelected();
				print(cellsInSelection.Count);
				return;
			}
		}
		cellsInSelection.Add(cell);
		print(cellsInSelection.Count);
		cell.SetSelected();

	}

	//Wrapper for cell atacking
	public static void AttackWrapper(CellBehaviour target, enmTeam team) {
		if (cellsInSelection.Count != 0) {
			if (team == enmTeam.ENEMY) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].AttackCell(target);
				}
			}
			else if (team == enmTeam.ALLIED) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].EmpowerCell(target);
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
		int numElements = elementCount = elementCount / 2;
		for (int i = 0; i < numElements; i++) {
			GameObject e = Instantiate(elementObj, gameObject.transform.position, Quaternion.identity);
			e.GetComponent<Element>().target = target;
			e.GetComponent<Element>().attacker = this;
		}
		UpdateCellInfo();
	}

	//Called when "attacking" your own cell
	public void EmpowerCell(CellBehaviour target) {
		int numElements = elementCount = elementCount / 2;
		for (int i = 0; i < numElements; i++) {
			GameObject e = Instantiate(elementObj, gameObject.transform.position, Quaternion.identity);
			e.GetComponent<Element>().target = target;
			e.GetComponent<Element>().attacker = this;
		}
		UpdateCellInfo();
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
	#endregion


	public void SetCellData(Vector2 position, enmTeam Team, int startingCount = 0, int maximum = 100, float regenerationRate = 2f) {
		gameObject.transform.position = position;
		elementCount = startingCount;
		maxElements = maximum;
		regenFrequency = regenerationRate;
		cellTeam = Team;
	}

	//Keeps generateing new elements for the cell
	public IEnumerator GenerateElements() {
		while (true) {
			yield return new WaitForSecondsRealtime(regenFrequency);
			if (elementCount < maxElements) {
				elementCount++;
				elementNrDisplay.text = elementCount.ToString();
				UpdateCellInfo();
			}
		}
	}

	//Selects or deselects a cell
	public void SetSelected() {
		if (isSelected) {
			isSelected = false;
		}
		else {
			isSelected = true;
			elementNrDisplay.color = new Color32(255, 0, 0, 255);
		}
	}

	//
	public void UpdateCellInfo() {

		if (elementCount >= 10 && elementCount <= maxElements) {
			float mappedValue = Map.MapFloat(elementCount, 10, maxElements, 1, 3);

			transform.localScale = new Vector3(mappedValue, mappedValue, 1);
			cellRadius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
		}
		if (elementCount > maxElements) {
			elementCount = maxElements;
		}

		elementNrDisplay.text = elementCount.ToString();
		textRenderer.sortingOrder = 2;

		if (generateCoroutine == null && cellTeam != enmTeam.NEUTRAL) {
			generateCoroutine = StartCoroutine(GenerateElements());
		}
		switch (cellTeam) {
			case enmTeam.ALLIED: {
				cellSprite.color = ally;
				return;
			}
			case enmTeam.ENEMY: {
				cellSprite.color = enemy;

				return;
			}
			case enmTeam.NEUTRAL: {
				cellSprite.color = neutral;
				return;
			}
		}
	}

	private void OnMouseOver() {
		if (!isSelected) {
			elementNrDisplay.color = new Color32(255, 0, 0, 255);
		}
		else {
			elementNrDisplay.color = new Color32(0, 255, 255, 255);
		}
		//OnMouseDown() doesn't support other mouse imputs except LMB .. lame 
		if (Input.GetMouseButtonDown(1)) {
			if (cellTeam == enmTeam.ALLIED) {
				AttackWrapper(this, enmTeam.ALLIED);
			}
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
			if (regenFrequency < 10) {
				regenFrequency += 0.5f;
				print(regenFrequency);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			if (regenFrequency > 0.5f) {
				regenFrequency -= 0.5f;
				print(regenFrequency);
			}
		}
		#endregion
	}

	//Changes the colour back to original
	private void OnMouseExit() {
		if (!isSelected) {
			elementNrDisplay.color = new Color32(255, 255, 255, 255);
		}
		else {
			elementNrDisplay.color = new Color32(255, 0, 0, 255);
		}
	}

	//Determine action depending on clicked cell's team.
	private void OnMouseDown() {
		switch (cellTeam) {
			case enmTeam.ALLIED: {
				ModifySelection(this);
				return;
			}
			case enmTeam.ENEMY: {
				AttackWrapper(this, cellTeam);
				return;
			}
			case enmTeam.NEUTRAL: {
				AttackWrapper(this, enmTeam.ENEMY);
				return;
			}
		}
	}

}
