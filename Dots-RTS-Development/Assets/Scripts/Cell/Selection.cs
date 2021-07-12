using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour {
	[SerializeField]
	private List<GameCell> cells = new List<GameCell>();
	public IEnumerable<GameCell> SelectedCells => cells;

	public void Select(GameCell cell) {
		cells.Add(cell);
	}

	public void Deselect(GameCell cell) {
		cells.Remove(cell);
	}

	public void Clear() {
		foreach (GameCell selectedCell in SelectedCells) {
			selectedCell.SetSelected(false);
		}
		cells.Clear();
	}

	public void HandleSelection(GameCell gameCell) {
		if (gameCell.Cell.team != Team.Allied) return;

		bool isSelected = IsSelected(gameCell);
		if (isSelected) {
			Deselect(gameCell);
		}
		else {
			Select(gameCell);
		}
		
		gameCell.SetSelected(!isSelected);
	}

	public bool IsSelected(GameCell gameCell) {
		return cells.Contains(gameCell);
	}

	public void Attack(GameCell target) {
		foreach (GameCell selectedCell in SelectedCells) {
			selectedCell.AttackCell(target);
		}
	}
}
