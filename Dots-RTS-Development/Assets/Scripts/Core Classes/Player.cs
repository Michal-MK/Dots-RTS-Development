using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public List<CellBehaviour> playerCells = new List<CellBehaviour>();

	private void Start() {
		foreach (CellBehaviour cell in PlayManager.cells) {
			if(cell.cellTeam == Cell.enmTeam.ALLIED) {
				playerCells.Add(cell);
			}
		}
	}
}