using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_AI : MonoBehaviour {

	public bool[] initAIs = new bool[8] { false, false, false, false, false, false, false, false };
	public Cell.enmTeam[] aiTeams = new Cell.enmTeam[8] { Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE };
	public float[] decisionSpeeds = new float[8];

	public Ally[] alliesOfAi = new Ally[8];

	public static Enemy_AI[] AIs = new Enemy_AI[8];

	private List<Enemy_AI> team1;
	private List<Enemy_AI> team2;
	private List<Enemy_AI> team3;
	private List<Enemy_AI> team4;

	private void Start() {
		StartAiInitialization();
	}

	public void StartAiInitialization() {
		foreach (Cell c in GameControll.cells) {
			//If cell is enemy create ai for that enemy - Only once
			if ((int)c.cellTeam >= 2) {
				//print((int)c.cellTeam + "  " + c.gameObject.name);
				switch ((int)c.cellTeam) {
					case 2: {
						SetAis(0, c.cellTeam);
						break;
					}
					case 3: {
						SetAis(1, c.cellTeam);
						break;
					}
					case 4: {
						SetAis(2, c.cellTeam);
						break;
					}
					case 5: {
						SetAis(3, c.cellTeam);
						break;
					}
					case 6: {
						SetAis(4, c.cellTeam);
						break;
					}
					case 7: {
						SetAis(5, c.cellTeam);
						break;
					}
					case 8: {
						SetAis(6, c.cellTeam);
						break;
					}
					case 9: {
						SetAis(7, c.cellTeam);
						break;
					}

				}
			}
		}
		CreateAlly(0, new int[] { 1, 2 });
		CreateAlly(1, new int[] { 2 });
		CreateAlly(7, new int[] { 3 });
		//CreateAlly(3, new int[] { 1, 2, 0 });

		print("forming");
		FormTeams();

	}
	private void SetAis(int index, Cell.enmTeam team) {
		//print("Attepting to set AI at " + index + " with team " + team);
		if (initAIs[index] == false) {
			initAIs[index] = true;
			aiTeams[index] = team;

			GameObject aiHolder = new GameObject("AI " + "code " + index + " enemy " + (index + 1));
			Enemy_AI ai = aiHolder.AddComponent<Enemy_AI>();
			ai.decisionSpeed = decisionSpeeds[index];
			ai._aiTeam = team;
			ai.isActive = true;
			AIs[index] = ai;
		}

	}

	public void FormTeams() {

		//Loop though all the AIs
		for (int i = 0; i < AIs.Length; i++) {
			//If allies for this AI exist
			if (AIs[i] != null) {
				//Loop though all of them
				for (int j = 0; j < alliesOfAi.Length; j++) {
					//If this enemy AI exists
					if (alliesOfAi[j] != null) {
						//If the AI index matches with the one in ally
						if (i == alliesOfAi[j].index) {
							//Loop though the associated allies and add them if they exist
							for (int k = 0; k < alliesOfAi[j].allies.Length; k++) {
								if (alliesOfAi[j].allies[k] != null) {
									AIs[i].alliesOfThisAI.Add(alliesOfAi[j].allies[k]);
									AIs[k].alliesOfThisAI.Add(AIs[i]);
								}
							}
						}
					}
				}
			}
		}
		print("formed");
	}


	private void CreateAlly(int enemyIndex, int[] allyIndexes) {
		foreach (int i in allyIndexes) {
			if (enemyIndex == i) {
				throw new System.Exception("Can't assign myself as my ally!");
			}
		}

		alliesOfAi[enemyIndex] = new Ally();

		alliesOfAi[enemyIndex].index = enemyIndex;
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < allyIndexes.Length; j++) {
				if (i == allyIndexes[j]) {
					if (AIs[i] != null) {
						alliesOfAi[enemyIndex].allies[i] = AIs[i];
						//alliesOfAi[enemyIndex].allies[j] = AIs[i];
					}
				}
			}
		}
	}
}

