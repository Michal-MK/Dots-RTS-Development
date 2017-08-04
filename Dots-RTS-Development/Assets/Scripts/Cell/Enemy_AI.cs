using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour {


	public bool isActive = true;
	public float decisionSpeed = 1f;

	public Cell.enmTeam _aiTeam;

	private enum Decision { EXPAND, ATTACK, HELP };

	public List<CellBehaviour> _targets = new List<CellBehaviour>();            //Cells this AI will attack
	public List<CellBehaviour> _aiCells = new List<CellBehaviour>();            //This AIs cells
	public List<CellBehaviour> _neutrals = new List<CellBehaviour>();           //Neutral cells in the scene
	public List<CellBehaviour> _allies = new List<CellBehaviour>();             //Cells this AI will not attack

	public List<Enemy_AI> alliesOfThisAI = new List<Enemy_AI>();

	private CellBehaviour selectedAiCell;                                       //Selected AI cell that will prefrom the action.
	private CellBehaviour selectedTargetCell;                                   //Selected target that can be attacked
	private CellBehaviour selectedNeutralCell;                                  //Selected cell for expansion
	private CellBehaviour selectedAiCellForAid = null;                          //Selected cell for empowering

	private float attackChoiceProbability = 0;
	private float expandChoiceProbability = 0;
	private float defendChoiceProbability = 0;


	//Event subscribers / unsubscribers
	private void OnEnable() {
		CellBehaviour.TeamChanged += RemoveCell;
	}
	private void OnDisable() {
		CellBehaviour.TeamChanged -= RemoveCell;
	}

	//Sort cells on screen to lists by their team
	private IEnumerator Start() {
		for (int i = 0; i < Control.cells.Count; i++) {

			CellBehaviour current = Control.cells[i];

			if (current.cellTeam == _aiTeam) {
				_aiCells.Add(current);
			}
			else if (current.cellTeam == Cell.enmTeam.NEUTRAL) {
				_neutrals.Add(current);
			}
			else {
				_targets.Add(current);
			}
		}
		yield return null;

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
		StartCoroutine(PreformAction());
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




	private List<DataHolder> GetCellInstances(CellBehaviour referenceCell) {
		List<DataHolder> AIs_Cells = new List<DataHolder>();
		for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
			if (Initialize_AI.AIs[i] != null) {
				Enemy_AI currentAI = Initialize_AI.AIs[i];
				DataHolder d = new DataHolder { AI = currentAI };
				AIs_Cells.Add(d);

				for (int j = 0; j < currentAI._aiCells.Count; j++) {
					if (referenceCell == currentAI._aiCells[j]) {
						d.cells[0] = referenceCell;
					}
				}
				for (int j = 0; j < currentAI._targets.Count; j++) {
					if (referenceCell == currentAI._targets[j]) {
						d.cells[1] = referenceCell;
					}
				}
				for (int j = 0; j < currentAI._allies.Count; j++) {
					if (referenceCell == currentAI._allies[j]) {
						d.cells[2] = referenceCell;
					}
				}
				for (int j = 0; j < currentAI._neutrals.Count; j++) {
					if (referenceCell == currentAI._neutrals[j]) {
						d.cells[3] = referenceCell;
					}
				}
			}
		}
		return AIs_Cells;
	}

	//Triggered when a cell changs team
	private void RemoveCell(CellBehaviour sender, Cell.enmTeam cellTeam, Cell.enmTeam elementTeam) {
		//print("Triggered " + gameObject.name + "  " + sender.gameObject.name + "  " + cellTeam + " " + elementTeam);
		List<DataHolder> data = GetCellInstances(sender);

		//DataDebugging
		//print(data.Count);
		//for (int i = 0; i < data.Count; i++) {
		//	print("AI name: " + data[i].AI.name);
		//	if (data[i].AI._aiTeam == _aiTeam) {
		//		print("The Data this Ai cell should see: ");
		//		for (int j = 0; j < data[i].cells.Length; j++) {
		//			switch (j) {
		//				case 0: {
		//					if (data[i].cells[j] != null) {
		//						print("This cell is a cell of this AI " + data[i].cells[j].gameObject.name);
		//					}
		//					else {
		//						print("No Data found for this AIs aiCell");
		//					}
		//					break;
		//				}
		//				case 1: {
		//					if (data[i].cells[j] != null) {
		//						print("This cell is a target of this AI " + data[i].cells[j].gameObject.name);
		//					}
		//					else {
		//						print("No Data found for this AIs target");
		//					}
		//					break;
		//				}
		//				case 2: {
		//					if (data[i].cells[j] != null) {
		//						print("This cell is an ally of this AI " + data[i].cells[j].gameObject.name);
		//					}
		//					else {
		//						print("No Data found for this AIs ally");
		//					}
		//					break;
		//				}
		//				case 3: {
		//					if (data[i].cells[j] != null) {
		//						print("This cell is a neutral cell of this AI " + data[i].cells[j].gameObject.name);
		//					}
		//					else {
		//						print("No Data found for this AIs neutal");
		//					}
		//					break;
		//				}
		//			}
		//		}
		//	}
		//}


		foreach (DataHolder instance in data) {

			if (instance.AI == this) {
				List<CellBehaviour> aiCells = new List<CellBehaviour>(instance.AI._aiCells);
				foreach (CellBehaviour c in aiCells) {
					for (int i = 0; i < instance.cells.Length; i++) {
						if (instance.cells[i] != null) {
							if (c == instance.cells[i]) {
								//print("If we get here, that means" + this.gameObject.name + " has " + sender.name + " in its AICELLS");
								if (elementTeam != _aiTeam) {
									if (_aiCells.Contains(sender)) {
										_aiCells.Remove(sender);
									}
									if (!_targets.Contains(sender)) {
										_targets.Add(sender);
									}
								}
							}
						}
					}
				}
				List<CellBehaviour> targets = new List<CellBehaviour>(instance.AI._targets);
				foreach (CellBehaviour c in targets) {
					for (int i = 0; i < instance.cells.Length; i++) {
						if (instance.cells[i] != null) {
							if (c == instance.cells[i]) {
								//print("If we get here, that means" + this.gameObject.name + " has " + sender.name + " in its TRAGETS");
								if (elementTeam == _aiTeam) {
									if (!_aiCells.Contains(sender)) {
										_aiCells.Add(sender);
									}
									if (_targets.Contains(sender)) {
										_targets.Remove(sender);
									}
								}
								else {
									foreach (Enemy_AI ai in instance.AI.alliesOfThisAI) {
										if (elementTeam == ai._aiTeam) {
											//print("Adding Ally because " + ai.name + " overtook " + c.name + " and " + this.gameObject.name + " is his ally");
											if (!_allies.Contains(sender)) {
												_allies.Add(sender);
											}
											if (_targets.Contains(sender)) {
												_targets.Remove(sender);
											}
										}
									}
								}
							}
						}
					}
				}
				List<CellBehaviour> allies = new List<CellBehaviour>(instance.AI._allies);
				foreach (CellBehaviour c in allies) {
					for (int i = 0; i < instance.cells.Length; i++) {
						if (instance.cells[i] != null) {
							if (c == instance.cells[i]) {
								//print("If we get here, that means" + this.gameObject.name + " has " + sender.name + " in its ALLIES");
								if (elementTeam == _aiTeam) {
									if (!_allies.Contains(sender)) {
										_allies.Remove(sender);
									}
									if (!_aiCells.Contains(sender)) {
										_aiCells.Add(sender);
									}
								}
								else {
									bool isOvertakenByAlly = false;
									foreach (Enemy_AI ai in instance.AI.alliesOfThisAI) {
										if (elementTeam == ai._aiTeam) {
											//print("Adding Ally because " + ai.name + " overtook " + c.name + " and " + this.gameObject.name + " is his ally");
											if (!_allies.Contains(sender)) {
												_allies.Add(sender);
											}
											isOvertakenByAlly = true;
										}
									}
									if (!isOvertakenByAlly) {
										if (!_targets.Contains(sender)) {
											_targets.Add(sender);
										}
										if (_allies.Contains(sender)) {
											_allies.Remove(sender);
										}
									}
								}
							}
						}
					}
				}
				List<CellBehaviour> neutralCells = new List<CellBehaviour>(instance.AI._neutrals);
				foreach (CellBehaviour c in neutralCells) {
					for (int i = 0; i < instance.cells.Length; i++) {
						if (instance.cells[i] != null) {
							if (c == instance.cells[i]) {
								//print("If we get here, that means" + this.gameObject.name + " has " + sender.name + " in its NEUTRALS");
								_neutrals.Remove(sender);

								if (elementTeam == _aiTeam) {
									_aiCells.Add(sender);
								}
								else {
									bool willBeAlly = false;
									foreach (Enemy_AI ai in instance.AI.alliesOfThisAI) {
										if (elementTeam == ai._aiTeam) {
											//print("Adding Ally because " + ai.name + " overtook " + c.name + " and " + this.gameObject.name + " is his ally");
											if (!_allies.Contains(sender)) {
												_allies.Add(sender);
											}
											willBeAlly = true;
										}
									}
									if (!willBeAlly) {
										if (!_targets.Contains(sender)) {
											_targets.Add(sender);
										}
									}
								}
							}
						}
					}
				}
			}
			//Debug.Break();
		}

		//foreach (DataHolder instance in data) {
		//	print("AAAAAAAAAAAAAA  5 * 5");
		//	for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
		//		if (Initialize_AI.AIs[i] != null) {
		//			print("BBBBBBBBBBB " + data.Count + " " + Initialize_AI.AIs.Length + " " + data.Count * Initialize_AI.AIs.Length * 5);
		//			if (instance.AI == Initialize_AI.AIs[i]) {
		//
		//				for (int j = 0; j < instance.cells.Length; j++) {
		//					if (instance.cells[j] != null) {
		//						print("Current Process = " + gameObject.name + " " + instance.AI.gameObject.name + " " + instance.cells[j].name + " " + j);
		//					}
		//					switch (j) {
		//						case 0: {
		//							if (instance.cells[j] != null) {
		//
		//								print("I know that " + instance.AI.name + " contains " + instance.cells[j].name + " as AICELL!");
		//								if (cellTeam != _aiTeam) {
		//									print("Cell " + sender.name + " of team " + cellTeam + " will be " + elementTeam + " => " + this.gameObject.name + " has to remove it from aiCells");
		//									_aiCells.Remove(sender);
		//								}
		//
		//								if (elementTeam == _aiTeam) {
		//									print("Cell " + sender.name + " of team " + cellTeam + " will be " + elementTeam + " => " + this.gameObject.name + " has to add it as a aicell.");
		//									_aiCells.Add(sender);
		//								}
		//							}
		//							continue;
		//						}
		//						case 1: {
		//							if (instance.cells[j] != null) {
		//								print("I know that " + instance.AI.name + " contains " + instance.cells[j].name + " as TARGET!");
		//								if (cellTeam == _aiTeam) {
		//									print("Cell " + sender.name + " of team " + cellTeam + " will be " + elementTeam + " => " + this.gameObject.name + " has to remove it from targets");
		//									_targets.Remove(sender);
		//								}
		//								if (elementTeam == _aiTeam) {
		//									print("Cell " + sender.name + " of team " + cellTeam + " will be " + elementTeam + " => " + this.gameObject.name + " has to add it as an aiCell.");
		//									_aiCells.Add(sender);
		//								}
		//							}
		//							continue;
		//						}
		//						case 2: {
		//							if (instance.cells[j] != null) {
		//								print("I know that " + instance.AI.name + " contains " + instance.cells[j].name + " as ALLY!");
		//								if (cellTeam == _aiTeam) {
		//									print("Cell " + sender.name + " of team " + cellTeam + " will be " + elementTeam + " => " + this.gameObject.name + " has to remove it from allies");
		//									_allies.Remove(sender);
		//								}
		//								if (elementTeam == _aiTeam) {
		//									print("Cell " + sender.name + " of team " + cellTeam + " will be " + elementTeam + " => " + this.gameObject.name + " has to add it as a ally.");
		//									_allies.Add(sender);
		//								}
		//							}
		//							continue;
		//						}
		//						case 3: {
		//							if (instance.cells[j] != null) {
		//								print("I know that " + instance.AI.name + " contains " + instance.cells[j].name + " as NEUTRAL!");
		//								print("Cell " + sender.name + " of team " + cellTeam + " will be " + elementTeam + " => " + this.gameObject.name + " has to remove it from neutrals");
		//								_neutrals.Remove(sender);
		//								if (elementTeam == _aiTeam) {
		//									print("Cell " + sender.name + " of team " + cellTeam + " will be " + elementTeam + " => " + this.gameObject.name + " has to add it as a aicell.");
		//									_aiCells.Add(sender);
		//								}
		//							}
		//							continue;
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//}

		//foreach (DataHolder instance in data) {
		//	if (instance.AI == this) {
		//		for (int i = 0; i < instance.cells.Length; i++) {
		//			if (instance.cells[i] != null) {
		//				if (i == 0) {
		//					if (elementTeam == this._aiTeam) {
		//						_aiCells.Add(sender);
		//					}
		//					else {
		//						_aiCells.Remove(sender);
		//					}
		//				}
		//				else if (i == 1) {
		//					if (elementTeam != this._aiTeam) {
		//						_targets.Add(sender);
		//					}
		//					else {
		//						_aiCells.Add(sender);
		//						_targets.Remove(sender);
		//					}
		//				}
		//				else if (i == 2) {
		//					if (elementTeam != this._aiTeam) {
		//						_allies.Add(sender);
		//					}
		//				}
		//				else {
		//					if (elementTeam != this._aiTeam) {
		//						_neutrals.Remove(sender);
		//					}
		//				}
		//			}
		//			else {
		//				if (i == 0) {
		//					if (elementTeam != this._aiTeam) {
		//						_aiCells.Remove(sender);
		//					}
		//				}
		//				else if (i == 1) {
		//					if (elementTeam == this._aiTeam) {
		//						_targets.Remove(sender);
		//					}
		//				}
		//				else if (i == 2) {
		//					if (elementTeam != this._aiTeam) {
		//						_targets.Add(sender);
		//						_allies.Remove(sender);
		//					}
		//				}
		//				else {

		//					_neutrals.Remove(sender);
		//
		//				}
		//			}
		//		}
		//	}
		//}
		//
		//Debug.Break();
		//
		//if (cellTeam == Cell.enmTeam.NEUTRAL) {
		//	_neutrals.Remove(sender);
		//	AddCell(sender, elementTeam);
		//	return;
		//}
		//
		////Cell was of team ALLIED
		//else if (cellTeam == Cell.enmTeam.ALLIED) {
		//	_targets.Remove(sender);
		//	AddCell(sender, elementTeam);
		//	return;
		//}
		////Cell was of team ENEMYX
		//else {
		//	//The AI the cell belonged to
		//	Enemy_AI homeAI = null;
		//	Enemy_AI newHomeAI = null;
		//
		//	for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
		//		if (Initialize_AI.AIs[i] != null) {
		//
		//			//If cellteam and aiteam are the same => remove from controlled
		//			if (Initialize_AI.AIs[i]._aiTeam == cellTeam && cellTeam == _aiTeam) {
		//				homeAI = Initialize_AI.AIs[i];
		//				/*Initialize_AI.AIs[i].*/
		//				_aiCells.Remove(sender);
		//			}
		//
		//			if (Initialize_AI.AIs[i]._aiTeam == elementTeam) {
		//				newHomeAI = Initialize_AI.AIs[i];
		//			}
		//		}
		//	}

		//	//Compare every AI with the AI that the cell belonged to if any AI was an ally of of THE AI remove it from their allies if not 
		//	for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
		//		if (Initialize_AI.AIs[i] != null) {
		//			for (int j = 0; j < Initialize_AI.AIs[i].alliesOfThisAI.Count; j++) {
		//				Enemy_AI allyOfAnyAI = Initialize_AI.AIs[i].alliesOfThisAI[j];
		//
		//
		//				if (allyOfAnyAI == homeAI && homeAI._aiTeam == _aiTeam) {
		//					/*allyOfAnyAI.*/
		//					_allies.Remove(sender);
		//				}
		//				else {
		//					/*allyOfAnyAI.*/
		//					_targets.Remove(sender);
		//				}
		//			}
		//		}
		//	}
		//	//AddCell(sender, future, newHomeAI);
		//	ConsiderAllies();
		//}
		//Debug.Break();
	}


	private void AddCell(CellBehaviour sender, Cell.enmTeam current, Enemy_AI defaultAI = null) {
		Enemy_AI homeAI = defaultAI;
		if (homeAI == null) {
			for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
				if (Initialize_AI.AIs[i] != null) {

					//If cellteam and aiteam are the same => remove from controlled
					if (Initialize_AI.AIs[i]._aiTeam == current) {
						homeAI = Initialize_AI.AIs[i];
					}
				}
			}
		}

		//Cell is joning team NEUTRAL
		if (current == Cell.enmTeam.NEUTRAL) {
			throw new Exception("Impossible");
		}

		//Cell is joning team ALLIED
		else if (current == Cell.enmTeam.ALLIED) {
			_targets.Add(sender);
			return;
		}

		//Cell is joning team ENEMYX
		else {
			//Compare every AI with the AI that the cell belongs to if any AI is an ally of of THE AI add it to their allies if not add it to their targets
			for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
				if (Initialize_AI.AIs[i] != null) {
					if (homeAI._aiTeam == current && current == _aiTeam) {
						//bool isMyCellAlready = false;
						//for (int k = 0; k < homeAI._allies.Count; k++) {
						//	if (sender == homeAI._allies[k]) {
						//		isMyCellAlready = true;
						//	}
						//}
						//if (!isMyCellAlready) {
						/*homeAI.*/
						_aiCells.Add(sender);
					}
					//}
					else {
						for (int j = 0; j < Initialize_AI.AIs[i].alliesOfThisAI.Count; j++) {
							Enemy_AI allyOfAnyAI = Initialize_AI.AIs[i].alliesOfThisAI[j];

							if (allyOfAnyAI == homeAI && homeAI._aiTeam == _aiTeam) {
								//bool isAllyAlredy = false;
								//for (int k = 0; k < allyOfAnyAI._allies.Count; k++) {
								//	if (sender == allyOfAnyAI._allies[k]) {
								//		isAllyAlredy = true;
								//	}
								//}
								//if (!isAllyAlredy) {
								/*allyOfAnyAI.*/
								_allies.Add(sender);
							}
							//}
							else {
								bool isTargetAlready = false;
								for (int k = 0; k < allyOfAnyAI._targets.Count; k++) {
									if (sender == allyOfAnyAI._targets[k]) {
										isTargetAlready = true;
									}
								}
								if (!isTargetAlready) {
									allyOfAnyAI._targets.Add(sender);
								}
							}
						}
					}
				}
			}
			if (_aiCells.Count == 0) {
				isActive = false;
			}
		}
	}














	//This is where the AI magic happens
	public IEnumerator PreformAction() {
		//yield return new WaitForEndOfFrame();
		//ConsiderAllies();


		while (isActive) {
			restart:
			yield return new WaitForSeconds(decisionSpeed);
			redoAction:

			bool isAlone = false;

			//AI selection
			if (_aiCells.Count != 0) {
				selectedAiCell = AiCellSelector();
				if (_aiCells.Count == 1) {
					isAlone = true;
				}
			}
			else {
				isActive = false;
				yield break;
			}
			if (!isAlone) {
				selectedAiCellForAid = AiAidSelector();
			}

			//Target selection
			if (_targets.Count != 0) {
				selectedTargetCell = TargetCellSelector();
			}
			else {
				yield break;
			}

			//Neutral cell selection
			if (_neutrals.Count != 0) {
				selectedNeutralCell = ExpandCellSelector();
			}
			else {
				selectedNeutralCell = null;
			}
			if (selectedAiCell == null) {
				//print("No cell found worthy of selection ... skipping turn.");
				goto restart;
			}


			Decision factor = CalculateBestChoice();

			//If these combinations are met the script will fail.. we have to redo the selection
			if (selectedNeutralCell == null && factor == Decision.EXPAND || isAlone == true && factor == Decision.HELP) {
				goto redoAction;
			}
			if (selectedTargetCell == null && factor == Decision.ATTACK) {
				goto restart;
			}


			if (factor == Decision.EXPAND) {

				Expand(selectedAiCell, selectedNeutralCell);

			}
			else if (factor == Decision.ATTACK) {

				Attack(selectedAiCell, selectedTargetCell);

			}
			else {

				Defend(selectedAiCell, selectedAiCellForAid);
			}
		}
	}

	//Expand from - to
	public void Expand(CellBehaviour selectedCell, CellBehaviour toTarget) {
		if (_neutrals.Count == 0) {
			//Attack(selectedCell, toTarget);
			return;
		}
		selectedCell.AttackCell(toTarget);
	}

	//Attack from - who
	public void Attack(CellBehaviour selectedCell, CellBehaviour target) {
		selectedCell.AttackCell(target);
	}

	//Defend from - who
	public void Defend(CellBehaviour selectedCell, CellBehaviour target) {
		selectedCell.EmpowerCell(target);
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
				return current;
			}
		}
		//If the biggest cell has more than "x" elements 80% return it
		if (elementRecordAI > 10) {
			if (UnityEngine.Random.Range(0, 10) < 8) {
				return _aiCells[recordIndex];
			}
			else {
				//If a target that can be overtaken exists still try to attack
				for (int i = 0; i < _targets.Count; i++) {
					for (int j = 0; j < _targets.Count; j++) {
						if (_aiCells[i].elementCount > (_targets[j].elementCount * 0.5f) - 1) {
							if (UnityEngine.Random.Range(0, 10) < 8) {
								return _aiCells[i];
							}
							else {
								return null;
							}
						}
					}
				}
				return null;
			}
		}
		else {
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
						defendChoiceProbability = 0.5f;
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
			defendChoiceProbability = 0.4f;
			return _aiCells[recordIndex];
		}
		//Helping not recommended
		else {
			defendChoiceProbability = 0f;
			return _aiCells[recordIndex];
		}
	}


	private Decision CalculateBestChoice() {

		if (attackChoiceProbability > expandChoiceProbability) {

			if ((defendChoiceProbability > expandChoiceProbability) || (defendChoiceProbability > attackChoiceProbability)) {

				if (UnityEngine.Random.Range(0, 10) < 5) {
					//print(selectedAiCell.gameObject.name + " Chose Defence with 50%");
					return Decision.HELP;
				}
				else {
					if (UnityEngine.Random.Range(0, 10) < 5) {
						//print(selectedAiCell.gameObject.name + " Chose Attack with 25%");
						return Decision.ATTACK;
					}
					else {
						//print(selectedAiCell.gameObject.name + " Chose Expand with 25%");
						return Decision.EXPAND;
					}
				}
			}
			else {
				if (UnityEngine.Random.Range(0, 10) < 7) {
					//print(selectedAiCell.gameObject.name + " Chose Attack with 70%");
					return Decision.ATTACK;
				}
				else {
					if (UnityEngine.Random.Range(0, 10) < 5) {
						//print(selectedAiCell.gameObject.name + " Chose Defence with 15%");
						return Decision.HELP;
					}
					else {
						//print(selectedAiCell.gameObject.name + " Chose Expand with 15%");
						return Decision.EXPAND;
					}
				}
			}
		}
		else {
			if (UnityEngine.Random.Range(0, 10) < 8) {
				//print(selectedAiCell.gameObject.name + " Chose Expand with 80%");
				return Decision.EXPAND;
			}
			else {
				if (UnityEngine.Random.Range(0, 10) < 5) {
					//print(selectedAiCell.gameObject.name + " Chose Attack with 10%");
					return Decision.ATTACK;
				}
				else {
					//print(selectedAiCell.gameObject.name + " Chose Defence with 10%");
					return Decision.HELP;
				}
			}
		}
	}
}

public class DataHolder {
	public Enemy_AI AI;
	public CellBehaviour[] cells = new CellBehaviour[4] { null, null, null, null };

}