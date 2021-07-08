using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IAlly {

	public bool isActive = true;

	protected readonly int aICellSelectElementThreshold = 10;
	protected readonly int aICellAidElementThreshold = 10;
	public float decisionSpeed = 1f;

	public Team team;

	protected readonly List<GameCell> targets = new List<GameCell>();           //Cells this AI will attack
	protected readonly List<GameCell> aiCells = new List<GameCell>();           //Cells this AI owns
	private readonly List<GameCell> allies = new List<GameCell>();              //Cells this AI will not attack

	protected GameCell selectedAiCell;											//Selected cell that will perform the action.
	protected GameCell selectedTargetCell;										//Selected cell that can be attacked
	protected GameCell selectedNeutralCell;										//Selected cell for expansion
	protected GameCell selectedAiCellForAid = null;								//Selected cell for empowering

	protected float attackChoiceProbability = 0;
	protected float expandChoiceProbability = 0;
	protected float defendChoiceProbability = 0;

	public PlayManager playManager;

	protected virtual void Start() {
		GameCell.TeamChanged += CellBehaviour_TeamChanged;

		ConsiderAllies();
		playManager = GameObject.Find(nameof(PlayManagerBehaviour)).GetComponent<PlayManagerBehaviour>().Instance;
		print("AI " + team + " Initialized!");
	}

	protected void OnDestroy() {
		GameCell.TeamChanged -= CellBehaviour_TeamChanged;
	}

	public void FindRelationWithCells() {
		foreach (GameCell current in playManager.AllCells) {
			if (current.Cell.team == team) {
				aiCells.Add(current);
			}
			else if (current.Cell.team != Team.Neutral) {
				// TODO this assumes that no allies exist
				targets.Add(current);
			}
		}
	}

	//AI List sorting logic
	private void CellBehaviour_TeamChanged(object sender, CellTeamChangeEventArgs e /*GameCell sender, Team previous, Team current*/) {
		/*
		 * We have to cover all cases that can happen -- Only do them when the AI is active
		 * (1.) Previous was Neutral
		 *			- Update neutrals in AI - DONE GLOBALLY
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
			if (e.Previous == Team.Neutral) {

				EnemyAI currAI = playManager.InitializedActors.GetAI(e.Current);

				if (currAI == this) {
					print("AI took over NEUTRAL---------------------");

					currAI.aiCells.Add(e.Cell);
					UpdateCellLists(currAI, e.Cell, true, true);
				}

				else if (currAI == null) {
					print("PLAYER took over NEUTRAL---------------------");

					Player.MyCells.Add(e.Cell);
					UpdateCellLists(Player, e.Cell, true, true);
				}

				playManager.NeutralCells.Remove(e.Cell);
			}

			if (e.Previous == Team.Allied) {

				Player.MyCells.Remove(e.Cell);
				UpdateCellLists(Player, e.Cell, false, false);

				EnemyAI currAI = playManager.InitializedActors.GetAI(e.Current);

				if (currAI == this) {
					print("AI took over PLAYER---------------------");

					currAI.aiCells.Add(e.Cell);
					currAI.targets.Remove(e.Cell);
					UpdateCellLists(currAI, e.Cell, true, true);
				}
			}

			if (e.Previous > Team.Allied) {
				EnemyAI prevAI = playManager.InitializedActors.GetAI(e.Previous);

				if (e.Current == Team.Allied) {
					print("PLAYER took over AI---------------------");

					UpdateCellLists(Player, e.Cell, true, true);

					if (prevAI == this) {
						prevAI.aiCells.Remove(e.Cell);

						UpdateCellLists(prevAI, e.Cell, false, false);

						if (Player.IsAllyOf(prevAI)) {
							print("Y u took my cell ;.;");
						}
					}
				}

				else {
					print("AI took over AI---------------------");

					EnemyAI currAI = playManager.InitializedActors.GetAI(e.Current);

					if (prevAI == this) {
						prevAI.aiCells.Remove(e.Cell);

						UpdateCellLists(prevAI, e.Cell, false, false);

						if (prevAI.aiCells.Count == 0) {
							DisableAI(this);
						}

						if (prevAI.IsAllyOf(currAI)) {
							print("Y U Took me cell bro...?");
							print(prevAI.gameObject.name + " Will be adding as an Ally");
							currAI.allies.Remove(e.Cell);
						}
						else if (prevAI.IsTargetOf(currAI)) {
							print(prevAI.gameObject.name + " Will be adding as a Target");
						}
					}

					if (currAI == this) {
						currAI.aiCells.Add(e.Cell);
						currAI.targets.Remove(e.Cell);

						UpdateCellLists(currAI, e.Cell, true, true);
					}
				}
			}
		}
		else {
			GameCell.TeamChanged -= CellBehaviour_TeamChanged;
		}
	}

	//Update this AI's allies and targets
	private void ConsiderAllies() {
		//Needs re-validation / rewrite
		if (gameObject.name == "AI code 1 enemy 2") {
			print("");
		}

		//Loop through all allied IAllies
		foreach (IAlly currentAlly in Allies) {
			//Loop though all aiCells of the allied IAlly
			foreach (GameCell currentCellOfTheAlliedAI in currentAlly.MyCells) {
				for (int l = 0; l < targets.Count; l++) {
					GameCell thisAIsTarget = targets[l];

					//If aiCell of the other AI and target of this AI are the same cell do Stuff
					if (currentCellOfTheAlliedAI == thisAIsTarget) {
						allies.Add(thisAIsTarget);
						targets.Remove(thisAIsTarget);
					}
				}
			}
		}
	}

	/// <summary>
	/// Applies information stored in the Data format to a selected AI
	/// </summary>
	/// <param name="data">The data to process</param>
	/// <param name="isAdding">Is the selected process addition or deletion</param>
	/// <param name="ai">The Ai to operate on, default points to the one specified in DataHolder</param>
	private void ProcessData(AIDataHolder data, bool isAdding, EnemyAI ai = null) {

		if (ai == null) {
			ai = data.AI;
		}
		else if (ai != data.AI) {
			throw new Exception("AI mismatch, trying to apply data for " + ai.gameObject.name + " to " + data.AI.gameObject.name);
		}

		switch (data.Relation) {
			case AIDataHolder.RelationToAI.Self: {
				if (isAdding) {
					if (!ai.aiCells.Contains(data.Sender)) {
						ai.aiCells.Add(data.Sender);
					}
				}
				else {
					ai.aiCells.Remove(data.Sender);
				}
				break;
			}
			case AIDataHolder.RelationToAI.Target: {
				if (isAdding) {
					if (!ai.targets.Contains(data.Sender)) {
						ai.targets.Add(data.Sender);
						ai.allies.Remove(data.Sender);
					}
				}
				else {
					ai.targets.Remove(data.Sender);
				}
				break;
			}
			case AIDataHolder.RelationToAI.Ally: {
				if (isAdding) {
					if (!ai.allies.Contains(data.Sender)) {
						ai.allies.Add(data.Sender);
						ai.targets.Remove(data.Sender);
					}
				}
				else {
					ai.allies.Remove(data.Sender);
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
		foreach (EnemyAI ally in playerScript.Allies) {
			AIDataHolder currData = AIDataHolder.TransformForAlly(new AIDataHolder(sender), ally);
			ally.ProcessData(currData, addAllies);
		}

		foreach (EnemyAI enemy in playerScript.Targets) {
			AIDataHolder currData = AIDataHolder.TransformForTarget(new AIDataHolder(sender), enemy);
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
	private void UpdateCellLists(EnemyAI ai, GameCell sender, bool addAllies, bool addTargets) {
		foreach (EnemyAI ally in ai.AIAllies) {
			AIDataHolder currData = AIDataHolder.TransformForAlly(new AIDataHolder(ai, sender), ally);
			ally.ProcessData(currData, addAllies);
		}

		foreach (EnemyAI enemy in ai.AITargets) {
			AIDataHolder currData = AIDataHolder.TransformForTarget(new AIDataHolder(ai, sender), enemy);
			enemy.ProcessData(currData, addTargets);
		}
	}

	private void DisableAI(EnemyAI ai) {
		ai.isActive = false;
		print("AI " + ai.gameObject.name + " was deactivated, No cells remain.");
	}

	#region IAlly implementation

	public Team Team => team;

	public List<IAlly> Targets { get; set; } = new List<IAlly>();

	public List<IAlly> Allies { get; set; } = new List<IAlly>();

	public bool IsAllyOf(IAlly other) {
		return Allies.Contains(other);
	}

	public bool IsTargetOf(IAlly other) {
		return Targets.Contains(other);
	}

	#endregion

	#region Getters and Operators

	/// <summary>
	/// Get a list containing all allies of this AI
	/// </summary>
	private List<EnemyAI> AIAllies => Allies.OfType<EnemyAI>().ToList();

	/// <summary>
	/// Get a list containing all targets of this AI
	/// </summary>
	private List<EnemyAI> AITargets => Targets.OfType<EnemyAI>().ToList();

	public List<GameCell> MyCells => aiCells;


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

	/// <summary>
	/// Get a player, it has a list containing all cells under player's control
	/// </summary>
	public Player Player { get; set; }

	#endregion
}
