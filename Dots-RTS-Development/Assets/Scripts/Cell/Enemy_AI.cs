using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour {
	public bool isActive = true;
	public float decisionSpeed = 2f;

	public List<CellScript> _targets = new List<CellScript>();
	public List<CellScript> _aiCells = new List<CellScript>();
	public List<CellScript> _neutrals = new List<CellScript>();

	//Event subscribers / unsubscribers
	private void OnEnable() {
		CellScript.TeamChanged += Cell_TeamChanged;
	}
	private void OnDisable() {
		CellScript.TeamChanged -= Cell_TeamChanged;
	}

	//Sort cells on screen to lists by their team
	void Start() {
		for (int i = 0; i < GameControll.cells.Count; i++) {
			if (GameControll.cells[i].team == CellScript.enmTeam.ALLIED) {
				_targets.Add(GameControll.cells[i]);
			}
			else if (GameControll.cells[i].team == CellScript.enmTeam.ENEMY) {
				_aiCells.Add(GameControll.cells[i]);
			}
			else {
				_neutrals.Add(GameControll.cells[i]);
			}
		}
		StartCoroutine(PreformAction());
	}

	//Triggered when a cell changs team
	private void Cell_TeamChanged(CellScript sender, CellScript.enmTeam prev, CellScript.enmTeam current) {
		//Removes the cell from a team list
		switch (prev) {
			case CellScript.enmTeam.ALLIED: {
				_targets.Remove(sender);
				goto addCell;
			}
			case CellScript.enmTeam.ENEMY: {
				_aiCells.Remove(sender);
				goto addCell;
			}
			case CellScript.enmTeam.NEUTRAL: {
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
			case CellScript.enmTeam.ALLIED: {
				_targets.Add(sender);
				return;
			}
			case CellScript.enmTeam.ENEMY: {
				_aiCells.Add(sender);
				return;
			}
			case CellScript.enmTeam.NEUTRAL: {
				_neutrals.Add(sender);
				return;
			}
		}
	}

	//This is where the AI magic happens
	public IEnumerator PreformAction() {

		while (isActive) {
			//print("Enemies " + _aiCells.Count + " Allies " + _targets.Count + " Neutrals " + _neutrals.Count);
			yield return new WaitForSecondsRealtime(decisionSpeed);
			redoAction:

			CellScript selectedAiCell;                                      //Selected AI cell that will prefrom the action.
			CellScript selectedTargetCell;                                  //Selected target that can be attacked
			CellScript selectedNeutralCell;                                 //Selected cell for expansion

			bool isAlone = false;

			//AI selection
			if (_aiCells.Count != 0) {
				selectedAiCell = _aiCells[UnityEngine.Random.Range(0, _aiCells.Count)];
			}
			else {
				isActive = false;
				yield break;
			}

			CellScript selectedAiCellForAid = selectedAiCell;

			while (selectedAiCellForAid == selectedAiCell) {
				if (_aiCells.Count == 1) {
					print("Can't help anyone... I'm alone.");
					isAlone = true;
					break;
				}
				selectedAiCellForAid = _aiCells[UnityEngine.Random.Range(0, _aiCells.Count)];
			}

			//Target selection
			if (_targets.Count != 0) {
				selectedTargetCell = _targets[UnityEngine.Random.Range(0, _targets.Count)];
			}
			else {
				GameControll.GameOver();
				yield break;
			}

			//Neutral cell selection
			if (_neutrals.Count != 0) {
				selectedNeutralCell = _neutrals[UnityEngine.Random.Range(0, _neutrals.Count)];
			}
			else {
				print("Can't expand anymore.");
				selectedNeutralCell = null;
			}

			int factor = UnityEngine.Random.Range(0, 3);

			//If these combinations are met the script will fail.. we have to redo the selection
			if (selectedNeutralCell == null && factor == 0 || isAlone == true && factor == 2) {
				print("Invalid Selection");
				goto redoAction;
			}



			if (factor == 0) {
				Expand(selectedAiCell, selectedNeutralCell);
			}
			else if (factor == 1) {
				Attack(selectedAiCell, selectedTargetCell);
			}
			else {
				Defend(selectedAiCell, selectedAiCellForAid);
			}
		}
	}
	//Expand from - to
	public void Expand(CellScript selectedCell, CellScript toTarget) {
		if (_neutrals.Count == 0) {
			Attack(selectedCell, toTarget);
			return;
		}
		print("Expanding as " + selectedCell.gameObject.name + " to " + toTarget.gameObject.name);

	}
	//Attack from - who
	public void Attack(CellScript selectedCell, CellScript target) {

		print("Attacking as " + selectedCell.gameObject.name + " to " + target.gameObject.name);
	}
	//Defend from - who
	public void Defend(CellScript selectedCell, CellScript target) {
		print("Defending as " + selectedCell.gameObject.name + " my " + target.gameObject.name);
	}
}
