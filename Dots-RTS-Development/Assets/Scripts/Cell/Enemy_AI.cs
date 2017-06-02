using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour {
	public bool isActive = true;
	public float decisionSpeed = 2f;

	private int _elements;
	private List<CellScript> _targets = new List<CellScript>();
	private List<CellScript> _aiCells = new List<CellScript>();
	private List<CellScript> _neutrals = new List<CellScript>();

	public CellScript cell;

	private void OnEnable() {
		cell.TeamChanged += Cell_TeamChanged;
	}
	private void OnDisable() {
		cell.TeamChanged -= Cell_TeamChanged;
	}

	void Start() {
		_elements = cell.GetElements;

		for (int i = 0; i < GameControll.cells.Count; i++) {
			if (GameControll.cells[i].team == CellScript.enmTeam.ALLIED) {
				_targets.Add(GameControll.cells[i]);
		}else if (GameControll.cells[i].team == CellScript.enmTeam.ENEMY) {
			_aiCells.Add(GameControll.cells[i]);
			}
			else {
				_neutrals.Add(GameControll.cells[i]);
			}
		}
	}

	private void Cell_TeamChanged() {
		Debug.Log(gameObject.name);
	}


	public IEnumerator PreformAction() {

		while (cell.team == CellScript.enmTeam.ENEMY && isActive) {
			yield return new WaitForSecondsRealtime(decisionSpeed);

		}
	}

	public void Expand(CellScript toTarget) {
		if(_neutrals.Count == 0) {
			Attack(toTarget);
			return;
		}


	}

	public void Attack(CellScript target) {
		if(_targets.Count == 0) {
			GameControll.GameOver();
			isActive = false;
			return;
		}
	}

	public void Defend(CellScript tareget) {

	}
}
