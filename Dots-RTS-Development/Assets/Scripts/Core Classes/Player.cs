using UnityEngine;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour, IAlly {
	Cell.enmTeam playerTeam;
	List<IAlly> alliesOfPlayer;
	List<IAlly> targetsOfPlayer;

	public List<IAlly> listOfAlly = new List<IAlly>();
	public List<CellBehaviour> playerCells = new List<CellBehaviour>();

	

	private void Start() {
		foreach (CellBehaviour cell in PlayManager.cells) {
			if (cell.cellTeam == Cell.enmTeam.ALLIED) {
				playerCells.Add(cell);
			}
		}
	}

	public Cell.enmTeam Team {
		get {
			return playerTeam;
		}
	}

	public List<IAlly> Targets { get {
			return targetsOfPlayer;
		}
		set { targetsOfPlayer = value;
		}
	}
	public List<IAlly> Allies {
		get {
			return alliesOfPlayer;
		}
		set {
			alliesOfPlayer = value;
		}
	}

	public bool IsAllyOf(IAlly other) {
		return Allies.Contains(other);
	}

	public bool IsTargetOf(IAlly other) {
		return Targets.Contains(other);
	}
}