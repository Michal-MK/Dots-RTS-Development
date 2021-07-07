using System;

public class CellTeamChangeEventArgs : EventArgs {

	public CellTeamChangeEventArgs(GameCell cell, Team prev, Team cur) {
		Cell = cell;
		Previous = prev;
		Current = cur;
	}

	public GameCell Cell { get; }
	public Team Previous { get; }
	public Team Current { get; }
}
