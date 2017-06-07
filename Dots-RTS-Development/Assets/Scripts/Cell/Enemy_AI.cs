using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour {
	public bool isActive = true;
	public float decisionSpeed = 0.4f;

	public List<CellBehaviour> _targets = new List<CellBehaviour>();
	public List<CellBehaviour> _aiCells = new List<CellBehaviour>();
	public List<CellBehaviour> _neutrals = new List<CellBehaviour>();

	private CellBehaviour selectedAiCell;                                      //Selected AI cell that will prefrom the action.
	private CellBehaviour selectedTargetCell;                                  //Selected target that can be attacked
	private CellBehaviour selectedNeutralCell;                                 //Selected cell for expansion
	private CellBehaviour selectedAiCellForAid = null;

	private float attackChoiceProbability = 0;
	private float expandChoiceProbability = 0;
	private float defendChoiceProbability = 0;


	//Event subscribers / unsubscribers
	private void OnEnable() {
		CellBehaviour.TeamChanged += Cell_TeamChanged;
	}
	private void OnDisable() {
		CellBehaviour.TeamChanged -= Cell_TeamChanged;
	}

	//Sort cells on screen to lists by their team
	void Start() {
		for (int i = 0; i < GameControll.cells.Count; i++) {
			if (GameControll.cells[i].cellTeam == Cell.enmTeam.ALLIED) {
				_targets.Add(GameControll.cells[i]);
			}
			else if (GameControll.cells[i].cellTeam == Cell.enmTeam.ENEMY) {
				_aiCells.Add(GameControll.cells[i]);
			}
			else {
				_neutrals.Add(GameControll.cells[i]);
			}
		}
		StartCoroutine(PreformAction());
	}

	//Triggered when a cell changs team
	private void Cell_TeamChanged(CellBehaviour sender, Cell.enmTeam prev, Cell.enmTeam current) {
		//Removes the cell from a team list
		switch (prev) {
			case Cell.enmTeam.ALLIED: {
				_targets.Remove(sender);
				goto addCell;
			}
			case Cell.enmTeam.ENEMY: {
				_aiCells.Remove(sender);
				goto addCell;
			}
			case Cell.enmTeam.NEUTRAL: {
				_neutrals.Remove(sender);
				goto addCell;
			}
		}

		//Adds the cell to a new list that it belongs to
		addCell:
		if (_aiCells.Count == 0) {
			GameControll.YouWon();
		}
		switch (current) {
			case Cell.enmTeam.ALLIED: {
				_targets.Add(sender);
				return;
			}
			case Cell.enmTeam.ENEMY: {
				_aiCells.Add(sender);
				return;
			}
			case Cell.enmTeam.NEUTRAL: {
				_neutrals.Add(sender);
				return;
			}
		}
	}

	//This is where the AI magic happens
	public IEnumerator PreformAction() {

		while (isActive) {
			restart:
			yield return new WaitForSecondsRealtime(decisionSpeed);
			redoAction:

			bool isAlone = false;

			//AI selection
			if (_aiCells.Count != 0) {
				//selectedAiCell = _aiCells[UnityEngine.Random.Range(0, _aiCells.Count)];
				selectedAiCell = AiCellSelector();
			}
			else {
				isActive = false;
				yield break;
			}

			selectedAiCellForAid = AiAidSelector();
			//while (selectedAiCellForAid == selectedAiCell) {
			//	if (_aiCells.Count == 1) {
			//		print("Can't help anyone... I'm alone.");
			//		isAlone = true;
			//		break;
			//	}
			//	selectedAiCellForAid = _aiCells[UnityEngine.Random.Range(0, _aiCells.Count)];
			//}

			//Target selection
			if (_targets.Count != 0) {
				//selectedTargetCell = _targets[UnityEngine.Random.Range(0, _targets.Count)];
				selectedTargetCell = TargetCellSelector();
			}
			else {
				GameControll.GameOver();
				yield break;
			}

			//Neutral cell selection
			if (_neutrals.Count != 0) {
				//selectedNeutralCell = _neutrals[UnityEngine.Random.Range(0, _neutrals.Count)];
				selectedNeutralCell = ExpandCellSelector();
			}
			else {
				//print("Can't expand anymore.");
				selectedNeutralCell = null;
			}
			if (selectedAiCell == null) {
				print("No cell found worthy of selection ... skipping turn.");
				goto restart;
			}


			int factor = CalculateBestChoice();

			//If these combinations are met the script will fail.. we have to redo the selection
			if (selectedNeutralCell == null && factor == 0 || isAlone == true && factor == 2) {
				goto redoAction;
			}
			if (selectedTargetCell == null && factor == 1) {
				goto restart;
			}


			if (factor == 0) {
				if (selectedNeutralCell == null) {
					//print("Expanding as " + selectedAiCell.gameObject.name + " to " + selectedNeutralCell.gameObject.name);
					Debug.LogError("Somehow Neutral cell is not assigned.");
				}
				else {
					Expand(selectedAiCell, selectedNeutralCell);
				}
			}
			else if (factor == 1) {
				if (selectedTargetCell == null) {
					//print("Attacking as " + selectedAiCell.gameObject.name + " to " + selectedTargetCell.gameObject.name);
					Debug.LogError("Somehow Target cell is not assigned.");
				}
				else {
					Attack(selectedAiCell, selectedTargetCell);
				}
			}
			else {
				if (selectedAiCellForAid == null) {
					//print("Defending as " + selectedAiCell.gameObject.name + " to " + selectedAiCellForAid.gameObject.name);
					Debug.LogError("Somehow Allied cell is not assigned.");
				}
				else {
					Defend(selectedAiCell, selectedAiCellForAid);
				}
			}
		}
	}

	//Expand from - to
	public void Expand(CellBehaviour selectedCell, CellBehaviour toTarget) {
		if (_neutrals.Count == 0) {
			Attack(selectedCell, toTarget);
			return;
		}
		selectedCell.AttackCell(toTarget);
		//print("Expanding as " + selectedCell.gameObject.name + " to " + toTarget.gameObject.name);
	}

	//Attack from - who
	public void Attack(CellBehaviour selectedCell, CellBehaviour target) {
		selectedCell.AttackCell(target);
		//print("Attacking as " + selectedCell.gameObject.name + " to " + target.gameObject.name);
	}
	//Defend from - who
	public void Defend(CellBehaviour selectedCell, CellBehaviour target) {
		selectedCell.EmpowerCell(target);
		//print("Defending as " + selectedCell.gameObject.name + " my " + target.gameObject.name);
	}



	public CellBehaviour AiCellSelector() {
		int elementRecordAI = -1;
		int recordIndex = -1;
		CellBehaviour current;
		for (int i = 0; i < _aiCells.Count; i++) {
			current = _aiCells[i];
			//If current element count is bigger than the previous.. overwrite it
			if (current.elementCount > elementRecordAI) {
				elementRecordAI = current.elementCount;
				recordIndex = i;
			}

			//Just return the curent cell.. lel
			if (UnityEngine.Random.Range(0, 10) < 1) {
				return current;
			}

		}
		//If the biggest cell has more than "x" elements, I'm very likely returning it.
		if (elementRecordAI > 10) {
			if (UnityEngine.Random.Range(0, 10) < 8) {
				return _aiCells[recordIndex];
			}
			//Else I'll just pass
			else {
				return null;
			}
		}
		//Else I'll pass completetly
		else {
			return null;
		}
	}

	public CellBehaviour TargetCellSelector() {
		//if I don't have an attacker .. don't even bother.
		if (selectedAiCell == null) {
			return null;
		}

		int elementRecord = 99999;
		int recordIndex = -1;
		CellBehaviour current;
		for (int i = 0; i < _targets.Count; i++) {
			current = _targets[i];

			//Just return the curent cell.. lel
			if (UnityEngine.Random.Range(0, 10) < 3) {
				attackChoiceProbability = 0.3f;
				return current;
			}
			//If current element count is smaller than the previous.. overwrite it
			if (current.elementCount < elementRecord) {
				elementRecord = current.elementCount;
				recordIndex = i;
			}
			//If curret cell's element count is smaller that the selected atacker's, likely return that one
			if (current.elementCount < selectedAiCell.elementCount) {
				if (UnityEngine.Random.Range(0, 10) < 6) {
					attackChoiceProbability = 0.5f;
					return current;
				}
			}
		}
		//50% to return the one with the lowest amount of elements
		if (UnityEngine.Random.Range(0, 10) < 5) {
			attackChoiceProbability = 0.5f;
			return _targets[recordIndex];
		}
		else {
			//Pls don't chose me lel
			defendChoiceProbability = 0f;
			return _targets[recordIndex];
		}
	}

	public CellBehaviour ExpandCellSelector() {
		CellBehaviour current;

		int elementRecord = 99999;
		int recordIndex = -1;
		for (int i = 0; i < _neutrals.Count; i++) {
			current = _neutrals[i];

			//Just return the curent cell.. lel
			if (UnityEngine.Random.Range(0, 10) < 4) {
				expandChoiceProbability = 0.4f;
				return current;
			}
			//Find a neutral cell that has the lowest element count.
			if (current.elementCount < elementRecord) {
				elementRecord = current.elementCount;
				recordIndex = i;
			}

			//If the neutral cell has less than "x" elements in it, probably return it
			if (current.elementCount < 5) {
				if (UnityEngine.Random.Range(0, 10) < 5) {
					expandChoiceProbability = 0.5f;
					return current;
				}
			}
		}
		expandChoiceProbability = 0.2f;
		return _neutrals[recordIndex];
	}

	public CellBehaviour AiAidSelector() {
		CellBehaviour current;

		int elementRecord = 99999;
		int recordIndex = -1;
		for (int i = 0; i < _aiCells.Count; i++) {
			current = _aiCells[i];

			if (current.elementCount < elementRecord) {
				if (current != selectedAiCell) {
					elementRecord = current.elementCount;
					recordIndex = i;
				}
			}
			if (current.elementCount < 5) {
				if (UnityEngine.Random.Range(0, 10) < 5) {
					if (current != selectedAiCell) {
						defendChoiceProbability = 0.5f;
						return current;
					}
				}
			}
			if (UnityEngine.Random.Range(0, 10) < 1) {
				expandChoiceProbability = 0.1f;
				return current;
			}
		}
		if (UnityEngine.Random.Range(0, 10) < 5) {
			defendChoiceProbability = 0.5f;
			return _aiCells[recordIndex];
		}
		else {
			defendChoiceProbability = 0f;
			return _aiCells[recordIndex];
		}
	}

	public int CalculateBestChoice() {

		// 0 = expand
		// 1 = attack
		// 2 = defend

		if (attackChoiceProbability > expandChoiceProbability) {
			if (defendChoiceProbability > expandChoiceProbability || defendChoiceProbability > attackChoiceProbability) {
				if (UnityEngine.Random.Range(0, 10) < 5) {
					print(selectedAiCell.gameObject.name + " Chose Defence with 50%");
					return 2;
				}
				else {
					if (UnityEngine.Random.Range(0, 10) < 5) {
						print(selectedAiCell.gameObject.name + " Chose Attack with 25%");
						return 1;
					}
					else {
						print(selectedAiCell.gameObject.name + " Chose Expand with 25%");
						return 0;
					}
				}
			}
			else {
				if (UnityEngine.Random.Range(0, 10) < 7) {
					print(selectedAiCell.gameObject.name + " Chose Attack with 70%");
					return 1;
				}
				else {
					if (UnityEngine.Random.Range(0, 10) < 5) {
						print(selectedAiCell.gameObject.name + " Chose Defence with 15%");
						return 2;
					}
					else {
						print(selectedAiCell.gameObject.name + " Chose Expand with 15%");
						return 0;
					}
				}
			}
		}
		else {
			if (UnityEngine.Random.Range(0, 10) < 8) {
				print(selectedAiCell.gameObject.name + " Chose Expand with 80%");
				return 0;
			}
			else {
				if (UnityEngine.Random.Range(0, 10) < 5) {
					print(selectedAiCell.gameObject.name + " Chose Attack with 10%");
					return 1;
				}
				else {
					print(selectedAiCell.gameObject.name + " Chose Defence with 10%");
					return 2;
				}
			}
		}
	}
}

