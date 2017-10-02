using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour {


	public bool isActive = true;

	public int aICellSelectElementTreshold = 10;
	public int aICellAidElementTreshold = 10;
	public float decisionSpeed = 1f;

	public Cell.enmTeam aiTeam;

	private enum Decision { EXPAND, ATTACK, HELP };

	public List<CellBehaviour> _targets = new List<CellBehaviour>();            //Cells this AI will attack
	public List<CellBehaviour> _aiCells = new List<CellBehaviour>();            //This AIs cells
	public List<CellBehaviour> _neutrals = new List<CellBehaviour>();           //Neutral cells in the scene
	public List<CellBehaviour> _allies = new List<CellBehaviour>();             //Cells this AI will not attack

	protected List<Enemy_AI> alliesOfThisAI = new List<Enemy_AI>();

	protected CellBehaviour selectedAiCell;                                       //Selected AI cell that will prefrom the action.
	protected CellBehaviour selectedTargetCell;                                   //Selected target that can be attacked
	protected CellBehaviour selectedNeutralCell;                                  //Selected cell for expansion
	protected CellBehaviour selectedAiCellForAid = null;                          //Selected cell for empowering

	protected float attackChoiceProbability = 0;
	protected float expandChoiceProbability = 0;
	protected float defendChoiceProbability = 0;

	private string s = "";

	//Event subscribers / unsubscribers
	private void OnEnable() {
		//CellBehaviour.TeamChanged += RemoveCell;
	}
	private void OnDisable() {
		//CellBehaviour.TeamChanged -= RemoveCell;
	}

	//Sort cells on screen to lists by their team
	protected virtual void Start() {
		CellBehaviour.TeamChanged += CellBehaviour_TeamChanged;

		for (int i = 0; i < PlayManager.cells.Count; i++) {

			CellBehaviour current = PlayManager.cells[i];

			if (current.cellTeam == aiTeam) {
				_aiCells.Add(current);
			}
			else if (current.cellTeam == Cell.enmTeam.NEUTRAL) {
				_neutrals.Add(current);
			}
			else {
				_targets.Add(current);
			}
		}

		ConsiderAllies();
		print("AI " + getCurrentAiTeam + " Initialized!");
	}

	protected virtual void OnDestroy() {
		CellBehaviour.TeamChanged -= CellBehaviour_TeamChanged;
	}

	private void CellBehaviour_TeamChanged(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current) {
		if (isActive) {
			Enemy_AI prevAI = (Enemy_AI)previous;
			DataHolder dataPrevAI = new DataHolder(prevAI, sender);
			Enemy_AI currAI = (Enemy_AI)current;
			DataHolder dataCurrAI = new DataHolder(currAI, sender);
			

		}
	}

	//Update this AI's allies and targets
	private void ConsiderAllies() {

		//Loop through all allied Enemy_AIs
		for (int j = 0; j < alliesOfThisAI.Count; j++) {
			Enemy_AI currentAI = alliesOfThisAI[j];                                                                                 //print("My ally has " + alliesOfThisAI[j]._aiCells.Count + " cells.  " + gameObject.name);

			//Loop though all aiCells of the allied Enemy_AI
			for (int k = 0; k < alliesOfThisAI[j]._aiCells.Count; k++) {
				CellBehaviour currentCellOfTheAlliedAI = alliesOfThisAI[j]._aiCells[k];                                       //print(currentAlliedCellOfTheAlliedAI.gameObject.name);

				//Loop though all the targets of this AI
				for (int l = 0; l < _targets.Count; l++) {
					CellBehaviour myTarget = _targets[l];                                                                         //print("Comparing " + currentAlliedCellOfTheAlliedAI + " to " + current + "ENEMY 2");

					//If aiCell of the other AI and target of this AI are the same cell do Stuff
					if (currentCellOfTheAlliedAI == myTarget) {
						this._allies.Add(myTarget);
						this._targets.Remove(myTarget);
					}
				}
			}
		}
	}

	/// <summary>
	/// Accepts a cell, returns location of that cell in every AI present.
	/// </summary>
	/// <param name="referenceCell">Cell to use in the aglorythm</param>
	/// <returns></returns>
	private List<DataHolder> GetCellInstances(CellBehaviour referenceCell) {
		List<DataHolder> AIs_Cells = new List<DataHolder>();
		for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
			if (Initialize_AI.AIs[i] != null) {
				Enemy_AI currentAI = Initialize_AI.AIs[i];
				DataHolder data = new DataHolder(currentAI, referenceCell);
				AIs_Cells.Add(data);
			}
		}
		return AIs_Cells;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="referenceCell">The cell to look for.</param>
	/// <param name="ai">The AI to look in.</param>
	/// <returns></returns>
	private DataHolder GetCellInstances(CellBehaviour referenceCell, Enemy_AI ai) {
		DataHolder data = new DataHolder(ai, referenceCell);
		return data;
	}

	//Triggered when a cell chanegs team
	//private void RemoveCell(CellBehaviour sender, Cell.enmTeam cellTeam, Cell.enmTeam elementTeam) {

	//	List<DataHolder> data = GetCellInstances(sender);
	//	#region DataDebugging
	//	//print(data.Count);
	//	//for (int i = 0; i < data.Count; i++) {
	//	//	print("AI name: " + data[i].AI.name);
	//	//	if (data[i].AI._aiTeam == _aiTeam) {
	//	//		print("The Data this Ai cell should see: ");
	//	//		for (int j = 0; j < data[i].cells.Length; j++) {
	//	//			switch (j) {
	//	//				case 0: {
	//	//					if (data[i].cells[j] != null) {
	//	//						print("This cell is a cell of this AI " + data[i].cells[j].gameObject.name);
	//	//					}
	//	//					else {
	//	//						print("No Data found for this AIs aiCell");
	//	//					}
	//	//					break;
	//	//				}
	//	//				case 1: {
	//	//					if (data[i].cells[j] != null) {
	//	//						print("This cell is a target of this AI " + data[i].cells[j].gameObject.name);
	//	//					}
	//	//					else {
	//	//						print("No Data found for this AIs target");
	//	//					}
	//	//					break;
	//	//				}
	//	//				case 2: {
	//	//					if (data[i].cells[j] != null) {
	//	//						print("This cell is an ally of this AI " + data[i].cells[j].gameObject.name);
	//	//					}
	//	//					else {
	//	//						print("No Data found for this AIs ally");
	//	//					}
	//	//					break;
	//	//				}
	//	//				case 3: {
	//	//					if (data[i].cells[j] != null) {
	//	//						print("This cell is a neutral cell of this AI " + data[i].cells[j].gameObject.name);
	//	//					}
	//	//					else {
	//	//						print("No Data found for this AIs neutal");
	//	//					}
	//	//					break;
	//	//				}
	//	//			}
	//	//		}
	//	//	}
	//	//}
	//	#endregion

	//	//Loop through all AIs and determine what to do with the removed cell in reference to the current AI
	//	//Each cell is loacted somewhere in every AI structure, be it Cell of the AI, its target, its ally, or a neutral cell.
	//	//If the cell does't exist in the AI then something went wrong.

	//	foreach (DataHolder instance in data) {
	//		if (instance.AI == this) {
	//			List<CellBehaviour> aiCells = new List<CellBehaviour>(instance.AI._aiCells);
	//			foreach (CellBehaviour c in aiCells) {
	//				for (int i = 0; i < instance.cells.Length; i++) {
	//					if (instance.cells[i] != null) {
	//						if (c == instance.cells[i]) {
	//							//print("If we get here, that means" + this.gameObject.name + " has " + sender.name + " in its AICELLS");
	//							if (elementTeam != getCurrentAiTeam) {
	//								if (_aiCells.Contains(sender)) {
	//									_aiCells.Remove(sender);
	//								}
	//								if (!_targets.Contains(sender)) {
	//									_targets.Add(sender);
	//								}
	//							}
	//						}
	//					}
	//				}
	//			}
	//			List<CellBehaviour> targets = new List<CellBehaviour>(instance.AI._targets);
	//			foreach (CellBehaviour c in targets) {
	//				for (int i = 0; i < instance.cells.Length; i++) {
	//					if (instance.cells[i] != null) {
	//						if (c == instance.cells[i]) {
	//							//print("If we get here, that means" + this.gameObject.name + " has " + sender.name + " in its TRAGETS");
	//							if (elementTeam == getCurrentAiTeam) {
	//								if (!_aiCells.Contains(sender)) {
	//									_aiCells.Add(sender);
	//								}
	//								if (_targets.Contains(sender)) {
	//									_targets.Remove(sender);
	//								}
	//							}
	//							else {
	//								foreach (Enemy_AI ai in instance.AI.alliesOfThisAI) {
	//									if (elementTeam == ai.getCurrentAiTeam) {
	//										//print("Adding Ally because " + ai.name + " overtook " + c.name + " and " + this.gameObject.name + " is his ally");
	//										if (!_allies.Contains(sender)) {
	//											_allies.Add(sender);
	//										}
	//										if (_targets.Contains(sender)) {
	//											_targets.Remove(sender);
	//										}
	//									}
	//								}
	//							}
	//						}
	//					}
	//				}
	//			}
	//			List<CellBehaviour> allies = new List<CellBehaviour>(instance.AI._allies);
	//			foreach (CellBehaviour c in allies) {
	//				for (int i = 0; i < instance.cells.Length; i++) {
	//					if (instance.cells[i] != null) {
	//						if (c == instance.cells[i]) {
	//							//print("If we get here, that means" + this.gameObject.name + " has " + sender.name + " in its ALLIES");
	//							if (elementTeam == getCurrentAiTeam) {
	//								if (!_allies.Contains(sender)) {
	//									_allies.Remove(sender);
	//								}
	//								if (!_aiCells.Contains(sender)) {
	//									_aiCells.Add(sender);
	//								}
	//							}
	//							else {
	//								bool isOvertakenByAlly = false;
	//								foreach (Enemy_AI ai in instance.AI.alliesOfThisAI) {
	//									if (elementTeam == ai.getCurrentAiTeam) {
	//										//print("Adding Ally because " + ai.name + " overtook " + c.name + " and " + this.gameObject.name + " is his ally");
	//										if (!_allies.Contains(sender)) {
	//											_allies.Add(sender);
	//										}
	//										isOvertakenByAlly = true;
	//									}
	//								}
	//								if (!isOvertakenByAlly) {
	//									if (!_targets.Contains(sender)) {
	//										_targets.Add(sender);
	//									}
	//									if (_allies.Contains(sender)) {
	//										_allies.Remove(sender);
	//									}
	//								}
	//							}
	//						}
	//					}
	//				}
	//			}
	//			List<CellBehaviour> neutralCells = new List<CellBehaviour>(instance.AI._neutrals);
	//			foreach (CellBehaviour c in neutralCells) {
	//				for (int i = 0; i < instance.cells.Length; i++) {
	//					if (instance.cells[i] != null) {
	//						if (c == instance.cells[i]) {
	//							//print("If we get here, that means" + this.gameObject.name + " has " + sender.name + " in its NEUTRALS");
	//							_neutrals.Remove(sender);

	//							if (elementTeam == getCurrentAiTeam) {
	//								_aiCells.Add(sender);
	//							}
	//							else {
	//								bool willBeAlly = false;
	//								foreach (Enemy_AI ai in instance.AI.alliesOfThisAI) {
	//									if (elementTeam == ai.getCurrentAiTeam) {
	//										//print("Adding Ally because " + ai.name + " overtook " + c.name + " and " + this.gameObject.name + " is his ally");
	//										if (!_allies.Contains(sender)) {
	//											_allies.Add(sender);
	//										}
	//										willBeAlly = true;
	//									}
	//								}
	//								if (!willBeAlly) {
	//									if (!_targets.Contains(sender)) {
	//										_targets.Add(sender);
	//									}
	//								}
	//							}
	//						}
	//					}
	//				}
	//			}
	//		}
	//	}
	//}


	public CellBehaviour AiCellSelector() {
		int elementRecordAI = -1;
		int recordIndex = -1;
		CellBehaviour current;

		for (int i = 0; i < _aiCells.Count; i++) {
			current = _aiCells[i];

			//Find the best cell to work with (the most elements)
			if (current.elementCount > elementRecordAI) {
				elementRecordAI = current.elementCount;
				recordIndex = i;
			}

			//Just return curent cell
			if (UnityEngine.Random.Range(0, 10) < 1) {
				s += "Selected 10% chance | ";
				return current;
			}
		}
		//If the biggest cell has more than "x" elements 80% return it
		if (elementRecordAI > aICellSelectElementTreshold) {
			if (UnityEngine.Random.Range(0, 10) < 6) {
				s += "Selected cell above threshold | ";
				return _aiCells[recordIndex];
			}
			else {
				s += "Cells above treshold exist, but were not selected | ";
				//If a target that can be overtaken exists still try to attack
				for (int i = 0; i < _aiCells.Count; i++) {
					for (int j = 0; j < _targets.Count; j++) {
						if ((_aiCells[i].elementCount * 0.5f) > ((_targets[j].elementCount) + 1)) {
							if (UnityEngine.Random.Range(0, 10) < 6) {
								s += "Selected Cell with guaranteed Overtake | ";
								return _aiCells[i];
							}
							else {
								s += "NO selection in second Chance | ";
								return null;
							}
						}
					}
				}
				s += "NO selection above " + aICellSelectElementTreshold + " treshold REALISM PLS | ";
				return null;
			}
		}
		else {
			s += "NO cell (highest " + elementRecordAI + ") above " + aICellSelectElementTreshold + " treshold | ";
			return null;
		}
	}

	public CellBehaviour TargetCellSelector() {
		//if I don't have an attacker .. don't even bother.
		if (selectedAiCell == null) {
			return null;
		}

		int elementRecord = 99999;
		int recordIndex = -1;

		CellBehaviour current;

		for (int i = 0; i < _targets.Count; i++) {
			current = _targets[i];

			//Just return curent cell
			if (UnityEngine.Random.Range(0, 10) < 2) {
				attackChoiceProbability = 0.3f;
				return current;
			}
			//Find the best cell to attack
			if (current.elementCount < elementRecord) {
				elementRecord = current.elementCount;
				recordIndex = i;
			}
			//If curret cell's element count is smaller than the one attacking 60% return it
			if (current.elementCount < selectedAiCell.elementCount) {
				if (UnityEngine.Random.Range(0, 10) < 6) {
					attackChoiceProbability = 0.6f;
					return current;
				}
			}
		}
		//70% return the best target
		if (UnityEngine.Random.Range(0, 10) < 7) {
			attackChoiceProbability = 0.7f;
			return _targets[recordIndex];
		}
		else {
			attackChoiceProbability = 0f;
			return _targets[recordIndex];
		}
	}

	public CellBehaviour ExpandCellSelector() {
		CellBehaviour current;

		int elementRecord = 99999;
		int recordIndex = -1;
		for (int i = 0; i < _neutrals.Count; i++) {
			current = _neutrals[i];

			//If there is still posibility to expand just return the curent cell
			if (UnityEngine.Random.Range(0, 10) < 4) {
				expandChoiceProbability = 0.4f;
				return current;
			}
			//Find a neutral cell that has the least elements
			if (current.elementCount < elementRecord) {
				elementRecord = current.elementCount;
				recordIndex = i;
			}

			//If the neutral cell has less than "x" elements in it 80% return it
			if (current.elementCount < 5) {
				if (UnityEngine.Random.Range(0, 10) < 8) {
					expandChoiceProbability = 0.8f;
					return current;
				}
			}
		}
		//20% return the best cell you found
		expandChoiceProbability = 0.2f;
		return _neutrals[recordIndex];
	}

	public CellBehaviour AiAidSelector() {

		CellBehaviour current;

		int elementRecord = 99999;
		int recordIndex = -1;
		for (int i = 0; i < _aiCells.Count; i++) {
			current = _aiCells[i];
			//Locate a cell with the lest amount of elements
			if (current.elementCount < elementRecord) {
				if (current != selectedAiCell) {
					elementRecord = current.elementCount;
					recordIndex = i;
				}
			}
			//If you get one and it has < 5 elements => 50% return it
			if (current.elementCount < 5) {
				if (UnityEngine.Random.Range(0, 10) < 5) {
					if (current != selectedAiCell) {
						if (Mathf.Abs(selectedAiCell.elementCount - current.elementCount) > aICellAidElementTreshold) {
							defendChoiceProbability = 0.5f;
						}
						else {
							defendChoiceProbability = 0.2f;
						}
						return current;
					}
				}
			}
			//Random cell for help
			if (UnityEngine.Random.Range(0, 10) < 1) {
				expandChoiceProbability = 0.1f;
				return current;
			}
		}
		// 40% return the best cell you found
		if (UnityEngine.Random.Range(0, 10) < 4) {
			if (Mathf.Abs(selectedAiCell.elementCount - _aiCells[recordIndex].elementCount) > aICellAidElementTreshold) {
				defendChoiceProbability = 0.6f;
			}
			else {
				defendChoiceProbability = 0.2f;
			}
			return _aiCells[recordIndex];
		}
		//Helping not recommended
		else {
			defendChoiceProbability = 0f;
			return _aiCells[recordIndex];
		}
	}

	/// <summary>
	/// Get current AIs Team
	/// </summary>
	public Cell.enmTeam getCurrentAiTeam {
		get { return aiTeam; }
		private set { aiTeam = value; }
	}

	public void AddAlly(Enemy_AI ai) {
		alliesOfThisAI.Add(ai);
	}

	public static explicit operator Enemy_AI(Cell.enmTeam team) {
		return Initialize_AI.AIs[(int)team - 2];
	}

}

public class DataHolder {
	/// <summary>
	/// The AI for which this configuration is valid
	/// </summary>
	private Enemy_AI _ai;
	/// <summary>
	/// Instances in this AI --> [0] = AI, [1] = target, [2] = ally, [3] = neutral
	/// </summary>
	private int[] cellIndexes = new int[4] { -1, -1, -1, -1 };

	public DataHolder(Enemy_AI AI, CellBehaviour cell) {
		_ai = AI;

		for (int i = 0; i < AI._aiCells.Count; i++) {
			if(AI._aiCells[i] == cell) {
				cellIndexes[0] = i;
			}
		}
		for (int i = 0; i < AI._targets.Count; i++) {
			if (AI._targets[i] == cell) {
				cellIndexes[1] = i;
			}
		}
		for (int i = 0; i < AI._allies.Count; i++) {
			if (AI._allies[i] == cell) {
				cellIndexes[2] = i;
			}
		}
		for (int i = 0; i < AI._neutrals.Count; i++) {
			if (AI._neutrals[i] == cell) {
				cellIndexes[3] = i;
			}
		}
	}
	public Enemy_AI getAI {
		get { return _ai; }
	}

	public int[] getCellIndexes {
		get { return cellIndexes; }
	}
}