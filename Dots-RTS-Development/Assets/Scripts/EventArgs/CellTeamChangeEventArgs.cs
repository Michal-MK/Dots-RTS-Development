using System;

public class CellTeamChangeEventArgs : EventArgs {

	public CellTeamChangeEventArgs(GameCell cell, Team prev, Team curr) {
		Cell = cell;
		Previous = prev;
		Current = curr;
	}

	public GameCell Cell { get; set; }
	public Team Previous { get; set; }
	public Team Current { get; set; }
}
