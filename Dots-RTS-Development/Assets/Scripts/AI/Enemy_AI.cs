using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour, IAlly {

	public bool isActive = true;

	public int aICellSelectElementTreshold = 10;
	public int aICellAidElementTreshold = 10;
	public float decisionSpeed = 1f;

	public Cell.enmTeam team;

	private enum Decision { EXPAND, ATTACK, HELP };

	private Player playerCells;

	public List<CellBehaviour> _targets = new List<CellBehaviour>();            //Cells this AI will attack -- aiCells of Target
	public List<CellBehaviour> _aiCells = new List<CellBehaviour>();            //This AIs cells
	public List<CellBehaviour> _allies = new List<CellBehaviour>();             //Cells this AI will not attack -- aiCells of Ally

	protected List<IAlly> alliesOfThisAI = new List<IAlly>();
	protected List<IAlly> targetsOfThisAI = new List<IAlly>();

	protected CellBehaviour selectedAiCell;                                       //Selected AI cell that will prefrom the action.
	protected CellBehaviour selectedTargetCell;                                   //Selected target that can be attacked
	protected CellBehaviour selectedNeutralCell;                                  //Selected cell for expansion
	protected CellBehaviour selectedAiCellForAid = null;                          //Selected cell for empowering

	protected float attackChoiceProbability = 0;
	protected float expandChoiceProbability = 0;
	protected float defendChoiceProbability = 0;

	private string s = "";

	//Sort cells on screen to lists by their team
	protected virtual void Start() {
		CellBehaviour.TeamChanged += CellBehaviour_TeamChanged;


		ConsiderAllies();

		//print("AI " + getCurrentAiTeam + " Initialized!");
	}
	protected virtual void OnDestroy() {
		CellBehaviour.TeamChanged -= CellBehaviour_TeamChanged;
	}

	public void FindRelationWithCells() {
		for (int i = 0; i < PlayManager.cells.Count; i++) {

			CellBehaviour current = PlayManager.cells[i];

			if (current.cellTeam == team) {
				_aiCells.Add(current);
			}
			else if (current.cellTeam != Cell.enmTeam.NEUTRAL) {
				_targets.Add(current);
			}
		}
	}

	//AI List sorting logic
	private void CellBehaviour_TeamChanged(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current) {
		/*
		 * We have to cover all cases that can happen -- Only do them when the AI is active
		 * (1.) Previous was Neutral
		 *			- Update neutrals in AI - DONE GLOBALY
		 *		2.Current is Player
		 *			- Set targets and allies - DONE
		 *		3.Current is Enemy
		 *			- Set targets and allies - DONE
		 *	
		 * (2.) Previous was Player
		 *		- Remove self - DONE
		 *		- Update targets and allies - DONE
		 *		1. Current is Neutral - IMPOSSIBLE
		 *		2. Current is Enemy
		 *			- Update self - DONE
		 *			- Update targets and allies - DONE
		 *	
		 * (3.) Previous was Enemy
		 *		- Remove self
		 *		- Update targets and allies - TEMP
		 *		1. Current is Neutral - IMPOSSIBLE
		 *		2. Current is Player:
		 *			- Update self - DONE
		 *			- Update targets and allies - DONE
		 *			1. Player is Ally of previous
		 *			2. Player is Target of previous
		 *		3.Current is Enemy
		 *			- Update self - DONE
		 *			- Update targets and allies - DONE
		 *			1. Enemy is Ally of previous
		 *			2. Enemy is Target of previous
		 */

		if (isActive) {
			if (previous == Cell.enmTeam.NEUTRAL) {

				Enemy_AI currAI = (Enemy_AI)current;

				if (currAI == this) {
					print("AI took over NEUTRAL---------------------");
					currAI._aiCells.Add(sender);

					foreach (Enemy_AI ally in currAI.getAiAllies) {
						AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(currAI, sender), ally);
						ally.ProcessData(currData, true);

					}
					foreach (Enemy_AI target in currAI.getAiTargets) {
						AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(currAI, sender), target);
						target.ProcessData(currData, true);
					}
				}

				else if (currAI == null) {
					print("PLAYER took over NEUTRAL---------------------");

					playerScript.playerCells.Add(sender);

					foreach (Enemy_AI ally in playerScript.Allies) {
						AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(playerScript, sender), ally);
						ally.ProcessData(currData, true, ally);
					}
					foreach (Enemy_AI target in playerScript.Targets) {
						AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(playerScript, sender), target);
						target.ProcessData(currData, true, target);
					}
				}
			}

			if (previous == Cell.enmTeam.ALLIED) {

				playerScript.playerCells.Remove(sender);

				foreach (Enemy_AI ally in playerScript.Allies) {
					AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(playerScript, sender), ally);
					ally.ProcessData(currData, false);
				}
				foreach (Enemy_AI enemy in playerScript.Targets) {
					AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(playerScript, sender), enemy);
					enemy.ProcessData(currData, false);
				}

				Enemy_AI currAI = (Enemy_AI)current;

				if (currAI == this) {
					print("AI took over PLAYER---------------------");

					currAI._aiCells.Add(sender);
					currAI._targets.Remove(sender);

					foreach (Enemy_AI ally in currAI.getAiAllies) {
						AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(currAI, sender), ally);
						ally.ProcessData(currData, true);
					}
					foreach (Enemy_AI enemy in currAI.getAiTargets) {
						AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(currAI, sender), enemy);
						enemy.ProcessData(currData, true);
					}
				}
			}

			if ((int)previous >= 2) {
				Enemy_AI prevAI = (Enemy_AI)previous;

				if (current == Cell.enmTeam.ALLIED) {
					print("PLAYER took over AI---------------------");

					foreach (Enemy_AI ally in playerScript.Allies) {
						AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(playerScript, sender), ally);
						ally.ProcessData(currData, true);
					}
					foreach (Enemy_AI enemy in playerScript.Targets) {
						AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(playerScript, sender), enemy);
						enemy.ProcessData(currData, true);
					}

					if (prevAI == this) {
						prevAI._aiCells.Remove(sender);

						foreach (Enemy_AI ally in prevAI.getAiAllies) {
							AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(prevAI, sender), ally);
							ally.ProcessData(currData, false);
						}
						foreach (Enemy_AI enemy in prevAI.getAiTargets) {
							AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(prevAI, sender), enemy);
							enemy.ProcessData(currData, false);
						}

						if (playerScript.IsAllyOf(prevAI)) {
							print("Y u took my cell ;.;");
						}
					}
				}

				else {
					print("AI took over AI---------------------");

					Enemy_AI currAI = (Enemy_AI)current;

					if (prevAI == this) {
						prevAI._aiCells.Remove(sender);

						foreach (Enemy_AI ally in prevAI.getAiAllies) {
							AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(prevAI, sender), ally);
							ally.ProcessData(currData, false);
						}
						foreach (Enemy_AI target in prevAI.getAiTargets) {
							AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(prevAI, sender), target);
							target.ProcessData(currData, false);
						}

						if (prevAI.IsAllyOf(currAI)) {
							print("Y U Took me cell bro...?");
							print(prevAI.gameObject.name + " Will be adding as an Ally");
							currAI._allies.Remove(sender);
						}
						else if (prevAI.IsTargetOf(currAI)) {
							print(prevAI.gameObject.name + " Will be adding as a Target");
						}
					}

					if (currAI == this) {
						currAI._aiCells.Add(sender);
						currAI._targets.Remove(sender);

						foreach (Enemy_AI ally in currAI.getAiAllies) {
							AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(currAI, sender), ally);
							ally.ProcessData(currData, true);
						}
						foreach (Enemy_AI target in currAI.getAiTargets) {
							AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(currAI, sender), target);
							target.ProcessData(currData, true);
						}
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

		//Loop through all allied IAllies
		for (int j = 0; j < getAiAllies.Count; j++) {
			Debug.LogWarning("Ehm.. hello we are looping throug one array and using its indexes in another..");
			IAlly currentAlly = Allies[j];																					//print("My ally " + getAiAllies[j] + " has " + alliesOfThisAI[j].MyCells.Count + " cells.  " + gameObject.name);

			//Loop though all aiCells of the allied IAlly
			for (int k = 0; k < currentAlly.MyCells.Count; k++) {
				CellBehaviour currentCellOfTheAlliedAI = currentAlly.MyCells[k];                                            //Loop though all the targets of this AI

				for (int l = 0; l < this._targets.Count; l++) {
					CellBehaviour myTarget = this._targets[l];                                                              //print("Comparing " + currentCellOfTheAlliedAI + " to " + currentAlly);

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
		for (int i = 0; i < PlayManager.neutralCells.Count; i++) {
			current = PlayManager.neutralCells[i];

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
		return PlayManager.neutralCells[recordIndex];
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
	/// Applies information stored in the Data format to a selected AI
	/// </summary>
	/// <param name="data">The data to process</param>
	/// <param name="is_Adding">Is the selected process addition or deletion</param>
	/// <param name="ai">The Ai to operate on, default points to the one specified in DataHolder</param>
	private void ProcessData(AI_Data_Holder data, bool is_Adding, Enemy_AI ai = null) {

		if (ai == null) {
			ai = data.getAI;
		}
		else if (ai != data.getAI) {
			throw new Exception("AI mismatch, trying to apply data for " + ai.gameObject.name + " to " + data.getAI.gameObject.name);
		}

		switch (data.getRelation) {
			case AI_Data_Holder.RelationToAI.MYSELF: {
				if (is_Adding) {
					if (!ai._aiCells.Contains(data.getSender)) {
						ai._aiCells.Add(data.getSender);
					}
				}
				else {
					ai._aiCells.Remove(data.getSender);
				}
				break;
			}
			case AI_Data_Holder.RelationToAI.TARGET: {
				if (is_Adding) {
					if (!ai._targets.Contains(data.getSender)) {
						ai._targets.Add(data.getSender);
						ai._allies.Remove(data.getSender);
					}
				}
				else {
					ai._targets.Remove(data.getSender);
				}
				break;
			}
			case AI_Data_Holder.RelationToAI.ALLY: {
				if (is_Adding) {
					if (!ai._allies.Contains(data.getSender)) {
						ai._allies.Add(data.getSender);
						ai._targets.Remove(data.getSender);
					}
				}
				else {
					ai._allies.Remove(data.getSender);
				}
				break;
			}
		}


	}

	#region IAlly implementation
	public Cell.enmTeam Team {
		get {
			return team;
		}
	}

	public List<IAlly> Targets {
		get {
			return targetsOfThisAI;
		}
		set {
			targetsOfThisAI = value;
		}
	}
	public List<IAlly> Allies {
		get {
			return alliesOfThisAI;
		}
		set {
			targetsOfThisAI = value;
		}
	}

	public bool IsAllyOf(IAlly other) {
		return Allies.Contains(other);
	}

	public bool IsTargetOf(IAlly other) {
		return Targets.Contains(other);
	}

	#endregion

	#region Getters and Operators
	/// <summary>
	/// Get current AIs Team
	/// </summary>
	public Cell.enmTeam getCurrentAiTeam {
		get { return team; }
		private set { team = value; }
	}

	/// <summary>
	/// Get a list containing all allies of this AI
	/// </summary>
	public List<Enemy_AI> getAiAllies {
		get {
			List<Enemy_AI> ais = new List<Enemy_AI>();
			foreach (IAlly ally in alliesOfThisAI) {
				Enemy_AI ai = ally as Enemy_AI;
				if (ai != null) {
					ais.Add(ai);
				}
			}
			return ais;
		}
	}

	public List<CellBehaviour> MyCells {
		get {
			return _aiCells;
		}
	}


	/// <summary>
	/// Get a list containing all targets of this AI
	/// </summary>
	public List<Enemy_AI> getAiTargets {
		get {
			List<Enemy_AI> ais = new List<Enemy_AI>();
			foreach (IAlly target in targetsOfThisAI) {
				Enemy_AI ai = target as Enemy_AI;
				if (ai != null) {
					ais.Add(ai);
				}
			}
			return ais;
		}
	}

	public void AddAlly(IAlly ally) {
		alliesOfThisAI.Add(ally);
	}
	public void RemoveAlly(IAlly ally) {
		alliesOfThisAI.Remove(ally);
	}
	public void AddTarget(IAlly target) {
		targetsOfThisAI.Add(target);
	}
	public void RemoveTarget(IAlly target) {
		targetsOfThisAI.Remove(target);
	}

	/// <summary>
	/// Get a player, it has a list containing all cells under player's control
	/// </summary>
	public Player playerScript {
		get { return playerCells; }
		set { playerCells = value; }
	}

	/// <summary>
	/// Cast Cell.enmTeam into a Enemy_AI script
	/// </summary>
	public static explicit operator Enemy_AI(Cell.enmTeam team) {
		int index = (int)team - 2;
		if (index < 0) {
			return null;
		}
		else {
			return Initialize_AI.AIs[(int)team - 2];
		}
	}
	#endregion

}