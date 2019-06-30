using UnityEngine;
using System.Collections;

public class AI_Behaviour : Enemy_AI {

	private enum Decision { EXPAND, ATTACK, HELP };

	private string ss = "";
	public bool printOutLog = false;

	protected override void Start() {
		base.Start();
		StartCoroutine(PreformAction());
	}

	private Decision CalculateBestChoice() {

		ss += "ATK = " + attackChoiceProbability + "EXP = " + expandChoiceProbability + "DEF = " + defendChoiceProbability + " | ";
		ss += "SELECTED AI = " + selectedAiCell + " TARGET = " + selectedTargetCell + " DEFENSE =  " + selectedAiCellForAid + " EXPAND = " + selectedNeutralCell + " | ";
		if (attackChoiceProbability > expandChoiceProbability) {
			ss += "ATTACK++ ? | ";
			if ((defendChoiceProbability > expandChoiceProbability) && (defendChoiceProbability > attackChoiceProbability)) {
				ss += "defense ++ | ";
				if (Random.Range(0, 10) < 5) {
					ss += "def 50% | ";
					return Decision.HELP;
				}
				else {
					if (Random.Range(0, 10) < 5) {
						ss += "atk 25% | ";
						return Decision.ATTACK;
					}
					else {
						ss += "exp 25% | ";
						return Decision.EXPAND;
					}
				}
			}
			else {
				ss += "attack++ | ";
				if (Random.Range(0, 10) < 7.5f) {
					ss += "atk 75% | ";
					return Decision.ATTACK;
				}
				else {
					if (Random.Range(0, 10) < 5) {
						ss += "def 12.5% | ";
						return Decision.HELP;
					}
					else {
						ss += "exp 12.5% | ";
						return Decision.EXPAND;
					}
				}
			}
		}
		else {
			ss += "EXPAND++ ? | ";
			if (Random.Range(0, 10) < 8) {
				ss += "exp 80% | ";
				return Decision.EXPAND;
			}
			else {
				if (Random.Range(0, 10) < 5) {
					ss += "atk 10% | ";
					return Decision.ATTACK;
				}
				else {
					ss += "def 10% | ";
					return Decision.HELP;
				}
			}
		}
	}

	#region Function to preform ATTACKS, DEFENSES, EXPANSIONS
	//Expand from - to
	private void Expand(GameCell selectedCell, GameCell toTarget) {
		if (playManager.NeutralCells.Count == 0) {
			print("Expanding to NONE -- return;");
			return;
		}
		selectedCell.AttackCell(toTarget);
	}

	//Attack from - who
	private void Attack(GameCell selectedCell, GameCell target) {
		selectedCell.AttackCell(target);
	}

	//Defend from - who
	private void Defend(GameCell selectedCell, GameCell target) {
		selectedCell.AttackCell(target);
	}
	#endregion

	//This is where the AI magic happens
	public IEnumerator PreformAction() {

		while (isActive) {
			yield return new WaitForSeconds(decisionSpeed);

			bool isAlone = false;
			/*AI selection
			 * If this AI has cells to select from, run a procedure that will chose one cell with which the AI will work this cycle.
			 * Else this AI will shut down.
			 */
			if (_aiCells.Count != 0) {
				ss += "Selecting | ";
				selectedAiCell = AiCellSelector();
				//If we have exactly one cell in the pool, we cant select others, no halping each other is possible.
				if (_aiCells.Count == 1) {
					isAlone = true;
					ss += "Alone | ";
				}
				else {
					ss += "Not Alone | ";
				}
			}
			else {
				isActive = false;
				ss += "No AI cell Selestion, DONE!";
				if (printOutLog) {
					print(ss);
				}
				ss = "";
				yield break;
			}

			if (selectedAiCell == null) {
				ss += "Could not Select Cell";

				if (printOutLog) {
					print(ss);
				}
				ss = "";
				continue;
			}

			//Aid selection
			if (!isAlone) {
				ss += "Selecting Friend | ";
				selectedAiCellForAid = AiAidSelector();
			}

			//Target selection
			if (_targets.Count != 0) {
				ss += "Selecting Target | ";
				selectedTargetCell = TargetCellSelector();
			}
			else {
				ss += "No TARGET cell Selestion, DONE!";
				if (printOutLog) {
					print(ss);
				}
				ss = "";
				yield break;
			}

			//Neutral cell selection
			if (playManager.NeutralCells.Count != 0) {
				ss += "Selecting Neutral | ";
				selectedNeutralCell = ExpandCellSelector();
			}
			else {
				ss += "No Neutrals exist | ";
				selectedNeutralCell = null;
			}


			ss += "Calculating choices | ";
			Decision factor = CalculateBestChoice();

			//If these combinations are met the script will fail.. we have to redo the selection
			if (selectedNeutralCell == null && factor == Decision.EXPAND) {
				ss += "Expanding with no free cells, DONE!";
				if (printOutLog) {
					print(ss);
				}
				ss = "";
				continue;
			}
			if (isAlone == true && factor == Decision.HELP) {
				ss += "Aiding with no allies, DONE!";
				if (printOutLog) {
					print(ss);
				}
				ss = "";
				continue;
			}
			if (selectedTargetCell == null && factor == Decision.ATTACK) {
				ss += "Attacking with no targets, DONE!";
				if (printOutLog) {
					print(ss);
				}
				ss = "";
				continue;
			}


			if (factor == Decision.EXPAND) {
				ss += "Expanding as " + selectedAiCell + " to " + selectedNeutralCell + " END ";
				Expand(selectedAiCell, selectedNeutralCell);

			}
			else if (factor == Decision.ATTACK) {
				ss += "Attacking as " + selectedAiCell + " cell " + selectedTargetCell + " END ";
				Attack(selectedAiCell, selectedTargetCell);

			}
			else {
				ss += "Aiding as " + selectedAiCell + " teammates cell " + selectedAiCellForAid + " END ";
				Defend(selectedAiCell, selectedAiCellForAid);
			}
			if (printOutLog) {
				print(ss);
			}
			ss = "";
		}
	}

	#region Cell Selecting code  -  AI Cells, Targets, Allies, Neutrals

	public GameCell AiCellSelector() {
		int elementRecordAI = -1;
		int recordIndex = -1;
		GameCell current;

		for (int i = 0; i < _aiCells.Count; i++) {
			current = _aiCells[i];

			//Find the best cell to work with (the most elements)
			if (current.Cell.ElementCount > elementRecordAI) {
				elementRecordAI = current.Cell.ElementCount;
				recordIndex = i;
			}

			//Just return curent cell
			if (UnityEngine.Random.Range(0, 10) < 1) {
				ss += "Selected 10% chance | ";
				return current;
			}
		}
		//If the biggest cell has more than "x" elements 80% return it
		if (elementRecordAI > aICellSelectElementTreshold) {
			if (UnityEngine.Random.Range(0, 10) < 6) {
				ss += "Selected cell above threshold | ";
				return _aiCells[recordIndex];
			}
			else {
				ss += "Cells above treshold exist, but were not selected | ";
				//If a target that can be overtaken exists still try to attack
				for (int i = 0; i < _aiCells.Count; i++) {
					for (int j = 0; j < _targets.Count; j++) {
						if ((_aiCells[i].Cell.ElementCount * 0.5f) > ((_targets[j].Cell.ElementCount) + 1)) {
							if (UnityEngine.Random.Range(0, 10) < 6) {
								ss += "Selected Cell with guaranteed Overtake | ";
								return _aiCells[i];
							}
							else {
								ss += "NO selection in second Chance | ";
								return null;
							}
						}
					}
				}
				ss += "NO selection above " + aICellSelectElementTreshold + " treshold REALISM PLS | ";
				return null;
			}
		}
		else {
			ss += "NO cell (highest " + elementRecordAI + ") above " + aICellSelectElementTreshold + " treshold | ";
			return null;
		}
	}

	public GameCell TargetCellSelector() {
		//if I don't have an attacker .. don't even bother.
		if (selectedAiCell == null) {
			return null;
		}

		int elementRecord = 99999;
		int recordIndex = -1;

		GameCell current;

		for (int i = 0; i < _targets.Count; i++) {
			current = _targets[i];

			//Just return curent cell
			if (UnityEngine.Random.Range(0, 10) < 2) {
				attackChoiceProbability = 0.3f;
				return current;
			}
			//Find the best cell to attack
			if (current.Cell.ElementCount < elementRecord) {
				elementRecord = current.Cell.ElementCount;
				recordIndex = i;
			}
			//If curret cell's element count is smaller than the one attacking 60% return it
			if (current.Cell.ElementCount < selectedAiCell.Cell.ElementCount) {
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

	public GameCell ExpandCellSelector() {
		GameCell current;

		int elementRecord = 99999;
		int recordIndex = -1;
		for (int i = 0; i < playManager.NeutralCells.Count; i++) {
			current = playManager.NeutralCells[i];

			//If there is still posibility to expand just return the curent cell
			if (UnityEngine.Random.Range(0, 10) < 4) {
				expandChoiceProbability = 0.4f;
				return current;
			}
			//Find a neutral cell that has the least elements
			if (current.Cell.ElementCount < elementRecord) {
				elementRecord = current.Cell.ElementCount;
				recordIndex = i;
			}

			//If the neutral cell has less than "x" elements in it 80% return it
			if (current.Cell.ElementCount < 5) {
				if (UnityEngine.Random.Range(0, 10) < 8) {
					expandChoiceProbability = 0.8f;
					return current;
				}
			}
		}
		//20% return the best cell you found
		expandChoiceProbability = 0.2f;
		return playManager.NeutralCells[recordIndex];
	}

	public GameCell AiAidSelector() {

		GameCell current;

		int elementRecord = 99999;
		int recordIndex = -1;
		for (int i = 0; i < _aiCells.Count; i++) {
			current = _aiCells[i];
			//Locate a cell with the lest amount of elements
			if (current.Cell.ElementCount < elementRecord) {
				if (current != selectedAiCell) {
					elementRecord = current.Cell.ElementCount;
					recordIndex = i;
				}
			}
			//If you get one and it has < 5 elements => 50% return it
			if (current.Cell.ElementCount < 5) {
				if (UnityEngine.Random.Range(0, 10) < 5) {
					if (current != selectedAiCell) {
						if (Mathf.Abs(selectedAiCell.Cell.ElementCount - current.Cell.ElementCount) > aICellAidElementTreshold) {
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
			if (Mathf.Abs(selectedAiCell.Cell.ElementCount - _aiCells[recordIndex].Cell.ElementCount) > aICellAidElementTreshold) {
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

	#endregion
}
