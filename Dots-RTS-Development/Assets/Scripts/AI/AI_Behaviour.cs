using UnityEngine;
using System.Collections;

public class AI_Behaviour : Enemy_AI {

	private enum Decision { EXPAND, ATTACK, HELP };

	private string s = "";
	public bool printOutLog = false;

	protected override void Start() {
		base.Start();
		StartCoroutine(PreformAction());
	}

	private Decision CalculateBestChoice() {
		s += "ATK = " + attackChoiceProbability + "EXP = " + expandChoiceProbability + "DEF = " + defendChoiceProbability + " | ";
		s += "SELECTED AI = " + selectedAiCell + " TARGET = " + selectedTargetCell + " DEFENSE =  " + selectedAiCellForAid + " EXPAND = " + selectedNeutralCell + " | ";
		if (attackChoiceProbability > expandChoiceProbability) {
			s += "ATTACK++ ? | ";
			if ((defendChoiceProbability > expandChoiceProbability) && (defendChoiceProbability > attackChoiceProbability)) {
				s += "defense ++ | ";
				if (Random.Range(0, 10) < 5) {
					s += "def 50% | ";
					return Decision.HELP;
				}
				else {
					if (Random.Range(0, 10) < 5) {
						s += "atk 25% | ";
						return Decision.ATTACK;
					}
					else {
						s += "exp 25% | ";
						return Decision.EXPAND;
					}
				}
			}
			else {
				s += "attack++ | ";
				if (Random.Range(0, 10) < 7.5f) {
					s += "atk 75% | ";
					return Decision.ATTACK;
				}
				else {
					if (Random.Range(0, 10) < 5) {
						s += "def 12.5% | ";
						return Decision.HELP;
					}
					else {
						s += "exp 12.5% | ";
						return Decision.EXPAND;
					}
				}
			}
		}
		else {
			s += "EXPAND++ ? | ";
			if (Random.Range(0, 10) < 8) {
				s += "exp 80% | ";
				return Decision.EXPAND;
			}
			else {
				if (Random.Range(0, 10) < 5) {
					s += "atk 10% | ";
					return Decision.ATTACK;
				}
				else {
					s += "def 10% | ";
					return Decision.HELP;
				}
			}
		}
	}

	#region Function to preform ATTACKS, DEFENCES, EXPANSIONS
	//Expand from - to
	private void Expand(CellBehaviour selectedCell, CellBehaviour toTarget) {
		if (_neutrals.Count == 0) {
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
				s += "Selecting | ";
				selectedAiCell = AiCellSelector();
				//If we have exactly one cell in the pool, we cant select others, no halping each other is possible.
				if (_aiCells.Count == 1) {
					isAlone = true;
					s += "Alone | ";
				}
				s += "Not Alone | ";

			}
			else {
				isActive = false;
				s += "No AI cell Selestion, DONE!";
				if (printOutLog) {
					print(s);
				}
				s = "";
				yield break;
			}

			if (selectedAiCell == null) {
				s += "Could not Select Cell";

				if (printOutLog) {
					print(s);
				}
				s = "";
				goto restart;
			}

			/* This section will select a cell from each cathegory, only one cathergory will be chosen in the end. */

			//Aid selection
			if (!isAlone) {
				s += "Selecting Friend | ";
				selectedAiCellForAid = AiAidSelector();
			}

			//Target selection
			if (_targets.Count != 0) {
				s += "Selecting Target | ";
				selectedTargetCell = TargetCellSelector();
			}
			else {
				s += "No TARGET cell Selestion, DONE!";
				if (printOutLog) {
					print(s);
				}
				s = "";
				yield break;
			}

			//Neutral cell selection
			if (_neutrals.Count != 0) {
				s += "Selecting Neutral | ";
				selectedNeutralCell = ExpandCellSelector();
			}
			else {
				s += "No Neutrals exist | ";
				selectedNeutralCell = null;
			}


			s += "Calculating choices | ";
			Decision factor = CalculateBestChoice();

			//If these combinations are met the script will fail.. we have to redo the selection
			if (selectedNeutralCell == null && factor == Decision.EXPAND) {
				s += "Expanding with no free cells, DONE!";
				if (printOutLog) {
					print(s);
				}
				s = "";
				goto redoAction;
			}
			if (isAlone == true && factor == Decision.HELP) {
				s += "Aiding with no allies, DONE!";
				if (printOutLog) {
					print(s);
				}
				s = "";
				goto redoAction;
			}
			if (selectedTargetCell == null && factor == Decision.ATTACK) {
				s += "Attacking with no targets, DONE!";
				if (printOutLog) {
					print(s);
				}
				s = "";
				goto restart;
			}


			if (factor == Decision.EXPAND) {
				s += "Expanding as " + selectedAiCell + " to " + selectedNeutralCell + " END ";
				Expand(selectedAiCell, selectedNeutralCell);

			}
			else if (factor == Decision.ATTACK) {
				s += "Attacking as " + selectedAiCell + " cell " + selectedTargetCell + " END ";
				Attack(selectedAiCell, selectedTargetCell);

			}
			else {
				s += "Aiding as " + selectedAiCell + " teammates cell " + selectedAiCellForAid + " END ";
				Defend(selectedAiCell, selectedAiCellForAid);
			}
			if (printOutLog) {
				print(s);
			}
			s = "";
		}
	}
}
