using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_AI : MonoBehaviour {

	public bool[] initAIs = new bool[8] { false, false, false, false, false, false, false, false };
	public Cell.enmTeam[] aiTeams = new Cell.enmTeam[8];
	public float[] decisionSpeeds = new float[8];

	private List<Enemy_AI> AIs = new List<Enemy_AI>();

	private List<Enemy_AI> team1;
	private List<Enemy_AI> team2;
	private List<Enemy_AI> team3;
	private List<Enemy_AI> team4;


	void Start() {
		foreach (Cell c in GameControll.cells) {
			if ((int)c.cellTeam >= 2) {
				print((int)c.cellTeam + "  " + c.gameObject.name);
				switch ((int)c.cellTeam) {
					case 2: {
						SetAIs(0, c.cellTeam);
						print(GameControll.cells.Count);
						break;
					}
					case 3: {
						SetAIs(1, c.cellTeam);
						break;
					}
					case 4: {
						SetAIs(2, c.cellTeam);
						break;
					}
					case 5: {
						SetAIs(3, c.cellTeam);
						break;
					}
					case 6: {
						SetAIs(4, c.cellTeam);
						break;
					}
					case 7: {
						SetAIs(5, c.cellTeam);
						break;
					}
					case 8: {
						SetAIs(6, c.cellTeam);
						break;
					}
					case 9: {
						SetAIs(7, c.cellTeam);
						break;
					}

				}
			}
		}

	}
	private void SetAIs(int index, Cell.enmTeam team) {
		print("Attepting to set AI at " + index + " with team " + team);
		if (initAIs[index] == false) {
			initAIs[index] = true;
			aiTeams[index] = team;

			GameObject aiHolder = new GameObject("AI " + index);
			Enemy_AI ai = aiHolder.AddComponent<Enemy_AI>();
			ai.decisionSpeed = decisionSpeeds[index];
			ai._aiTeam = team;
			ai.isActive = true;
			AIs.Add(ai);
		}
	}

	public void AITeams(Enemy_AI[] teams) {
		if (team1 != null) {
			team1 = new List<Enemy_AI>();
			for (int i = 0; i < teams.Length; i++) {
				team1.Add(teams[i]);
			}
		}
		else if (team2 != null) {
			team2 = new List<Enemy_AI>();
			for (int i = 0; i < teams.Length; i++) {
				team2.Add(teams[i]);
			}
		}
		else if (team3 != null) {
			team3 = new List<Enemy_AI>();
			for (int i = 0; i < teams.Length; i++) {
				team3.Add(teams[i]);
			}
		}
		else if (team4 != null) {
			team4 = new List<Enemy_AI>();
			for (int i = 0; i < teams.Length; i++) {
				team4.Add(teams[i]);
			}
		}
	}

}
