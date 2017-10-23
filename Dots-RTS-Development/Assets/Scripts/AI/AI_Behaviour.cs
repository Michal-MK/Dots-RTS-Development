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

	#region Function to preform ATTACKS, DEFENCES, EXPANSIONS
	//Expand from - to
	private void Expand(CellBehaviour selectedCell, CellBehaviour toTarget) {
		if (PlayManager.neutralCells.Count == 0) {
			print("Expanding to NONE -- return;");
			return;
		}
		selectedCell.AttackCell(toTarget);
	}

	//Attack from - who
	private void Attack(CellBehaviour selectedCell, CellBehaviour target) {
		selectedCell.AttackCell(target);
	}

	//Defend from - who
	private void Defend(CellBehaviour selectedCell, CellBehaviour target) {
		selectedCell.EmpowerCell(target);
	}
	#endregion

	//This is where the AI magic happens
	public IEnumerator PreformAction() {


		while (isActive) {
			print("Waiting " + (decisionSpeed) + " seconds.");
			restart: //If selection fails for a known reason use goto restart;
			yield return new WaitForSeconds(decisionSpeed);
			redoAction: //If selection fails due to a flaw in the design LEL use goto redoAction;

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
				ss += "Not Alone | ";

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
				goto restart;
			}

			/* This section will select a cell from each cathegory, only one cathergory will be chosen in the end. */

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
			if (PlayManager.neutralCells.Count != 0) {
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
				goto redoAction;
			}
			if (isAlone == true && factor == Decision.HELP) {
				ss += "Aiding with no allies, DONE!";
				if (printOutLog) {
					print(ss);
				}
				ss = "";
				goto redoAction;
			}
			if (selectedTargetCell == null && factor == Decision.ATTACK) {
				ss += "Attacking with no targets, DONE!";
				if (printOutLog) {
					print(ss);
				}
				ss = "";
				goto restart;
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
}
