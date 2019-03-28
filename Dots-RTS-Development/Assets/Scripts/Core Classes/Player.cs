using UnityEngine;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour, IAlly {

	public Team Team { get; set; } = Team.ALLIED;

	private void Start() {
		foreach (GameCell cell in PlayManager.cells) {
			if (cell.Cell.CellTeam == Team.ALLIED) {
				MyCells.Add(cell);
			}
		}
	}

	public List<IAlly> Targets { get; set; } = new List<IAlly>();
	public List<IAlly> Allies { get; set; } = new List<IAlly>();

	public List<GameCell> MyCells { get; } = new List<GameCell>();

	#region Ally/Target manipulation
	public bool IsAllyOf(IAlly other) {
		return Allies.Contains(other);
	}

	public bool IsTargetOf(IAlly other) {
		return Targets.Contains(other);
	}

	public void AddAlly(IAlly ally) {
		Allies.Add(ally);
	}

	public void RemoveAlly(IAlly ally) {
		Allies.Remove(ally);
	}

	public void AddTarget(IAlly target) {
		Targets.Add(target);
	}

	public void RemoveTarget(IAlly target) {
		Targets.Remove(target);
	} 
	#endregion
}