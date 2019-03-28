using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour, IAlly {

	public bool isActive = true;

	public int aICellSelectElementTreshold = 10;
	public int aICellAidElementTreshold = 10;
	public float decisionSpeed = 1f;

	public Team team;

	private enum Decision { EXPAND, ATTACK, HELP };

	private Player playerCells;

	public List<GameCell> _targets = new List<GameCell>();            //Cells this AI will attack -- aiCells of Target
	public List<GameCell> _aiCells = new List<GameCell>();            //This AIs cells
	public List<GameCell> _allies = new List<GameCell>();             //Cells this AI will not attack -- aiCells of Ally

	protected List<IAlly> alliesOfThisAI = new List<IAlly>();
	protected List<IAlly> targetsOfThisAI = new List<IAlly>();

	protected GameCell selectedAiCell;                                       //Selected AI cell that will prefrom the action.
	protected GameCell selectedTargetCell;                                   //Selected target that can be attacked
	protected GameCell selectedNeutralCell;                                  //Selected cell for expansion
	protected GameCell selectedAiCellForAid = null;                          //Selected cell for empowering

	protected float attackChoiceProbability = 0;
	protected float expandChoiceProbability = 0;
	protected float defendChoiceProbability = 0;

	private string s = "";

	//Sort cells on screen to lists by their team
	protected virtual void Start() {
		GameCell.TeamChanged += CellBehaviour_TeamChanged;

		ConsiderAllies();

		print("AI " + getCurrentAiTeam + " Initialized!");
	}

	protected virtual void OnDestroy() {
		GameCell.TeamChanged -= CellBehaviour_TeamChanged;
	}

	public void FindRelationWithCells() {
		for (int i = 0; i < PlayManager.cells.Count; i++) {

			GameCell current = PlayManager.cells[i];

			if (current.Cell.CellTeam == team) {
				_aiCells.Add(current);
			}
			else if (current.Cell.CellTeam != Team.NEUTRAL) {
				_targets.Add(current);
			}
		}
	}

	//AI List sorting logic
	private void CellBehaviour_TeamChanged(GameCell sender, Team previous, Team current) {
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
			if (previous == Team.NEUTRAL) {

				Enemy_AI currAI = (Enemy_AI)current;

				if (currAI == this) {
					print("AI took over NEUTRAL---------------------");

					currAI._aiCells.Add(sender);
					UpdateCellLists(currAI, sender, true, true);
				}

				else if (currAI == null) {
					print("PLAYER took over NEUTRAL---------------------");

					playerScript.MyCells.Add(sender);
					UpdateCellLists(playerScript, sender, true, true);
				}

				PlayManager.neutralCells.Remove(sender);
			}

			if (previous == Team.ALLIED) {

				playerScript.MyCells.Remove(sender);
				UpdateCellLists(playerScript, sender, false, false);

				Enemy_AI currAI = (Enemy_AI)current;

				if (currAI == this) {
					print("AI took over PLAYER---------------------");

					currAI._aiCells.Add(sender);
					currAI._targets.Remove(sender);
					UpdateCellLists(currAI, sender, true, true);
				}
			}

			if ((int)previous >= 2) {
				Enemy_AI prevAI = (Enemy_AI)previous;

				if (current == Team.ALLIED) {
					print("PLAYER took over AI---------------------");

					UpdateCellLists(playerScript, sender, true, true);

					if (prevAI == this) {
						prevAI._aiCells.Remove(sender);

						UpdateCellLists(prevAI, sender, false, false);

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

						UpdateCellLists(prevAI, sender, false, false);

						if(prevAI._aiCells.Count == 0) {
							DisableAI(this);
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

						UpdateCellLists(currAI, sender, true, true);

					}
				}
			}
		}
		else {
			GameCell.TeamChanged -= CellBehaviour_TeamChanged;
		}
	}

	//Update this AI's allies and targets
	private void ConsiderAllies() {  //Needs revalidtion / rewrite
		if(gameObject.name == "AI code 1 enemy 2") {
			print("");
		}

		//Loop through all allied IAllies
		for (int j = 0; j < Allies.Count; j++) {
			IAlly currentAlly = Allies[j];																					//print("My ally " + getAiAllies[j] + " has " + alliesOfThisAI[j].MyCells.Count + " cells.  " + gameObject.name);

			//Loop though all aiCells of the allied IAlly
			for (int k = 0; k < currentAlly.MyCells.Count; k++) {
				GameCell currentCellOfTheAlliedAI = currentAlly.MyCells[k];                                            //Loop though all the targets of this AI

				for (int l = 0; l < _targets.Count; l++) {
					GameCell thisAIsTarget = _targets[l];                                                              //print("Comparing " + currentCellOfTheAlliedAI + " to " + currentAlly);

					//If aiCell of the other AI and target of this AI are the same cell do Stuff
					if (currentCellOfTheAlliedAI == thisAIsTarget) {
						_allies.Add(thisAIsTarget);
						_targets.Remove(thisAIsTarget);
					}
				}
			}
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

	/// <summary>
	///	Wrapper for Updating cell lists inside an AI
	/// </summary>
	/// <param name="playerScript">The script to operate on</param>
	/// <param name="sender">The cell that activated this function call</param>
	/// <param name="addAllies">Should we add the cell or remove it?</param>
	/// <param name="addTargets">Should we add the cell or remove it?</param>
	private void UpdateCellLists(Player playerScript, GameCell sender, bool addAllies, bool addTargets) {
		foreach (Enemy_AI ally in playerScript.Allies) {
			AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(playerScript, sender), ally);
			ally.ProcessData(currData, addAllies);
		}

		foreach (Enemy_AI enemy in playerScript.Targets) {
			AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(playerScript, sender), enemy);
			enemy.ProcessData(currData, addTargets);
		}
	}
	/// <summary>
	///	Wrapper for Updating cell lists inside an AI
	/// </summary>
	/// <param name="ai">The script to operate on</param>
	/// <param name="sender">The cell that activated this function call</param>
	/// <param name="addAllies">Should we add the cell or remove it?</param>
	/// <param name="addTargets">Should we add the cell or remove it?</param>
	private void UpdateCellLists(Enemy_AI ai, GameCell sender, bool addAllies, bool addTargets) {
		foreach (Enemy_AI ally in ai.getAiAllies) {
			AI_Data_Holder currData = AI_Data_Holder.TransformForAlly(new AI_Data_Holder(ai, sender), ally);
			ally.ProcessData(currData, addAllies);
		}

		foreach (Enemy_AI enemy in ai.getAiTargets) {
			AI_Data_Holder currData = AI_Data_Holder.TransformForTarget(new AI_Data_Holder(ai, sender), enemy);
			enemy.ProcessData(currData, addTargets);
		}
	}

	private void DisableAI(Enemy_AI ai) {
		ai.isActive = false;
		print("AI " + ai.gameObject.name + " was deactivaed, No cells remain.");
	}

	#region IAlly implementation
	public Team Team {
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
	public Team getCurrentAiTeam {
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

	public List<GameCell> MyCells {
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
	public static explicit operator Enemy_AI(Team team) {
		int index = (int)team - 2;
		if (index < 0) {
			return null;
		}
		else {
			foreach (Enemy_AI ai in Initialize_AI.AIs) {
				if (ai.team == team) {
					return ai;
				}
			}
			Debug.LogError("AIs doesn't have an ai with team " + team);
			return null;
		}
	}
	#endregion

}