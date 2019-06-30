public enum EditorMode {
	None,
	/// <summary>
	/// In this mode user can use the mouse only for placing new cells to the scene 
	/// </summary>
	PlaceCells,
	/// <summary>
	/// In this mode user can use the mouse only for moving the cell of altering its attributes
	/// </summary>
	EditCells,
	/// <summary>
	/// In this mode user can use the mouse only for removeing the cell by clicking on it
	/// </summary>
	DeleteCells
};