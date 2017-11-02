using UnityEngine;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour, IAlly {

	public Cell.enmTeam playerTeam = Cell.enmTeam.ALLIED;
	private List<IAlly> alliesOfPlayer = new List<IAlly>();
	private List<IAlly> targetsOfPlayer = new List<IAlly>();

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

	public List<IAlly> Targets {
		get {
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

	public List<CellBehaviour> MyCells {
		get {
			return playerCells;
		}
	}

	public bool IsAllyOf(IAlly other) {
		return Allies.Contains(other);
	}

	public bool IsTargetOf(IAlly other) {
		return Targets.Contains(other);
	}

	public void AddAlly(IAlly ally) {
		alliesOfPlayer.Add(ally);
	}

	public void RemoveAlly(IAlly ally) {
		alliesOfPlayer.Remove(ally);
	}

	public void AddTarget(IAlly target) {
		targetsOfPlayer.Add(target);
	}

	public void RemoveTarget(IAlly target) {
		targetsOfPlayer.Remove(target);
	}
}