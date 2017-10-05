using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour {


	public bool isActive = true;

	public int aICellSelectElementTreshold = 10;
	public int aICellAidElementTreshold = 10;
	public float decisionSpeed = 1f;

	public Cell.enmTeam aiTeam;

	private enum Decision { EXPAND, ATTACK, HELP };

	private Player playerCells;
	public List<CellBehaviour> _targets = new List<CellBehaviour>();            //Cells this AI will attack
	public List<CellBehaviour> _aiCells = new List<CellBehaviour>();            //This AIs cells
	public List<CellBehaviour> _neutrals = new List<CellBehaviour>();           //Neutral cells in the scene
	public List<CellBehaviour> _allies = new List<CellBehaviour>();             //Cells this AI will not attack

	protected List<Enemy_AI> alliesOfThisAI = new List<Enemy_AI>();
	protected List<Enemy_AI> targetsOfThisAI = new List<Enemy_AI>();

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

		foreach (Enemy_AI friend in alliesOfThisAI) {
			print("ally of " + aiTeam + " is " + friend.aiTeam);
		}
		foreach (Enemy_AI target in targetsOfThisAI) {
			print("target of " + aiTeam + " is " + target.aiTeam);
		}
	}

	protected virtual void OnDestroy() {
		CellBehaviour.TeamChanged -= CellBehaviour_TeamChanged;
	}

	private void CellBehaviour_TeamChanged(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current) {

		if (isActive) {
			if (previous == Cell.enmTeam.NEUTRAL) {
				for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
					if (Initialize_AI.AIs[i] != null) {
						Initialize_AI.AIs[i]._neutrals.Remove(sender);
					}
				}
				Enemy_AI reference = (Enemy_AI)current;
				if (reference == this) {
					reference._aiCells.Add(sender);

					foreach (Enemy_AI ally in reference.getAllies) {
						DataHolder currData = DataHolder.TransformForAlly(new DataHolder(reference, sender), ally);
						ally.ProcessData(currData, true);
					}
					foreach (Enemy_AI target in reference.getTargets) {
						DataHolder currData = DataHolder.TransformForAlly(new DataHolder(reference, sender), target);
						target.ProcessData(currData, true);
					}
				}
				else if (reference == null) {
					//Cell was taken by a player --> we have no reference so we'll add sender as a target to every AI because player+enemy teams are not possible atm.
					foreach (Enemy_AI ai in Initialize_AI.AIs) {
						if (ai == this) {
							ai._targets.Add(sender);
						}
					}
				}
			}
			if (previous == Cell.enmTeam.ALLIED) {
				Enemy_AI curr = (Enemy_AI)current;
				curr._aiCells.Add(sender);
				foreach (Enemy_AI ally in curr.getAllies) {
					DataHolder currData = DataHolder.TransformForAlly(new DataHolder(curr, sender), ally);
					ally.ProcessData(currData, true);
				}
				foreach (Enemy_AI enemy in curr.getTargets) {
					DataHolder currData = DataHolder.TransformForTarget(new DataHolder(curr, sender), enemy);
					enemy.ProcessData(currData, true);
				}
			}
			if ((int)previous >= 2) {
				if (current == Cell.enmTeam.ALLIED) {
					//Cell was taken by a player --> we have no reference so we'll add sender as a target to every AI because player+enemy teams are not possible atm.
					foreach (Enemy_AI ai in Initialize_AI.AIs) {
						if (ai == this) {
							ai._targets.Add(sender);
						}
					}
					Enemy_AI prevAI = (Enemy_AI)previous;
					prevAI._aiCells.Remove(sender);
				}
				else {
					Enemy_AI prevAI = (Enemy_AI)previous;
					DataHolder dataPrevAI = new DataHolder(prevAI, sender);
					Enemy_AI currAI = (Enemy_AI)current;


					if (prevAI == this) {
						prevAI._targets.Add(sender);
						prevAI._aiCells.Remove(sender);
					}
					if (currAI == this) {
						currAI._aiCells.Add(sender);
						currAI._targets.Remove(sender);
					}

					foreach (Enemy_AI ally in prevAI.getAllies) {
						DataHolder currData = DataHolder.TransformForAlly(new DataHolder(prevAI, sender), ally);
						ally.ProcessData(currData, true);
					}
					foreach (Enemy_AI target in prevAI.getTargets) {
						DataHolder currData = DataHolder.TransformForTarget(new DataHolder(prevAI, sender), target);
						target.ProcessData(currData, true);
					}
					foreach (Enemy_AI ally in currAI.getAllies) {
						DataHolder currData = DataHolder.TransformForAlly(new DataHolder(currAI, sender), ally);
						ally.ProcessData(currData, true);
					}
					foreach (Enemy_AI target in currAI.getTargets) {
						DataHolder currData = DataHolder.TransformForTarget(new DataHolder(currAI, sender), target);
						target.ProcessData(currData, true);
					}
				}
			}
		}
		else {
			CellBehaviour.TeamChanged -= CellBehaviour_TeamChanged;
		}
	}

	//Update this AI's allies and targets
	private void ConsiderAllies() {  //Needs revalidtion / rewrite

		//Loop through all allied Enemy_AIs
		for (int j = 0; j < getAllies.Count; j++) {
			Enemy_AI currentAlly = getAllies[j];                                                                                 //print("My ally has " + alliesOfThisAI[j]._aiCells.Count + " cells.  " + gameObject.name);

			//Loop though all aiCells of the allied Enemy_AI
			for (int k = 0; k < currentAlly._aiCells.Count; k++) {
				CellBehaviour currentCellOfTheAlliedAI = currentAlly._aiCells[k];                                       //print(currentAlliedCellOfTheAlliedAI.gameObject.name);

				//Loop though all the targets of this AI
				for (int l = 0; l < this._targets.Count; l++) {
					CellBehaviour myTarget = this._targets[l];                                                                         //print("Comparing " + currentAlliedCellOfTheAlliedAI + " to " + current + "ENEMY 2");

					//If aiCell of the other AI and target of this AI are the same cell do Stuff
					if (currentCellOfTheAlliedAI == myTarget) {
						this._allies.Add(myTarget);
						this._targets.Remove(myTarget);
					}
				}
			}
		}
	}

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
	/// Accepts a cell, return its location in the AI provided
	/// </summary>
	/// <param name="referenceCell">The cell to look for.</param>
	/// <param name="ai">The AI to look in.</param>
	/// <returns></returns>
	private DataHolder GetCellInstances(CellBehaviour referenceCell, Enemy_AI ai) {
		DataHolder data = new DataHolder(ai, referenceCell);
		return data;
	}

	/// <summary>
	/// Applies information stored in the Data format to a selected AI
	/// </summary>
	/// <param name="data">The data to process</param>
	/// <param name="is_Adding">Is the selected process addition or deletion -- not implemented</param>
	/// <param name="ai">The Ai to operate on, default points to the one specified in DataHolder</param>
	private void ProcessData(DataHolder data, bool is_Adding, Enemy_AI ai = null) {
		if (ai == null) {
			ai = data.getAI;
		}

		for (int i = 0; i < data.getCellIndexes.Length; i++) {
			if (data.getCellIndexes[i] != -1) {
				switch (i) {
					case 0: {
						if (is_Adding) {
							ai._aiCells.Add(data.getCells[i]);
						}
						else {
							ai._aiCells.RemoveAt(data.getCellIndexes[i]);
						}
						break;
					}
					case 1: {
						if (is_Adding) {
							ai._targets.Add(data.getCells[i]);
						}
						else {
							ai._targets.RemoveAt(data.getCellIndexes[i]);
						}
						break;
					}
					case 2: {
						if (is_Adding) {
							ai._allies.Add(data.getCells[i]);
						}
						else {
							ai._allies.RemoveAt(data.getCellIndexes[i]);
						}
						break;
					}
					case 3: {
						if (is_Adding) {
							ai._neutrals.Add(data.getCells[i]);
						}
						else {
							ai._neutrals.RemoveAt(data.getCellIndexes[i]);
						}
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Get current AIs Team
	/// </summary>
	public Cell.enmTeam getCurrentAiTeam {
		get { return aiTeam; }
		private set { aiTeam = value; }
	}

	/// <summary>
	/// Get a list containing all allies of this AI
	/// </summary>
	public List<Enemy_AI> getAllies {
		get { return alliesOfThisAI; }
	}

	/// <summary>
	/// Get a list containing all targets of this AI
	/// </summary>
	public List<Enemy_AI> getTargets {
		get { return targetsOfThisAI; }
	}

	public Player playerScript {
		get { return playerCells; }
		set { playerCells = value; }
	}

	public void AddAlly(Enemy_AI ai) {
		alliesOfThisAI.Add(ai);
	}
	public void AddTarget(Enemy_AI ai) {
		targetsOfThisAI.Add(ai);
	}

	public static explicit operator Enemy_AI(Cell.enmTeam team) {
		int index = (int)team - 2;
		if (index < 0) {
			return null;
		}
		else {
			return Initialize_AI.AIs[(int)team - 2];
		}
	}
}

public class DataHolder {
	/// <summary>
	/// The AI for which this configuration is valid
	/// </summary>
	private Enemy_AI _ai;
	private Enemy_AI _allyAI = null;
	/// <summary>
	/// Instances in this AI --> [0] = AI, [1] = target, [2] = ally, [3] = neutral
	/// </summary>
	private int[] cellIndexes = new int[4] { -1, -1, -1, -1 };
	private CellBehaviour[] cells = new CellBehaviour[4];

	public DataHolder(Enemy_AI AI, CellBehaviour cell) {
		_ai = AI;

		for (int i = 0; i < AI._aiCells.Count; i++) {
			if (AI._aiCells[i] == cell) {
				cellIndexes[0] = i;
				cells[0] = cell;
			}
		}
		for (int i = 0; i < AI._targets.Count; i++) {
			if (AI._targets[i] == cell) {
				cellIndexes[1] = i;
				cells[0] = cell;
			}
		}
		for (int i = 0; i < AI._allies.Count; i++) {
			if (AI._allies[i] == cell) {
				cellIndexes[2] = i;
				cells[0] = cell;
			}
		}
		for (int i = 0; i < AI._neutrals.Count; i++) {
			if (AI._neutrals[i] == cell) {
				cellIndexes[3] = i;
				cells[0] = cell;
			}
		}
	}

	public static DataHolder TransformForAlly(DataHolder data, Enemy_AI ally) {

		data._ai = ally;

		data.cellIndexes[2] = data.cellIndexes[0];
		data.cellIndexes[0] = -1;

		return data;
	}

	public static DataHolder TransformForTarget(DataHolder data, Enemy_AI target) {
		data._ai = target;

		data.cellIndexes[1] = data.cellIndexes[0];
		data.cellIndexes[0] = -1;

		return data;
	}

	public Enemy_AI getAI {
		get { return _ai; }
	}

	public Enemy_AI getAllyAI {
		get { return _allyAI; }
	}

	public int[] getCellIndexes {
		get { return cellIndexes; }
	}

	public CellBehaviour[] getCells {
		get { return cells; }
	}
}