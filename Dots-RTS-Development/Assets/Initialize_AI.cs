using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_AI : MonoBehaviour {

	public bool[] initAIs = new bool[8] { false, false, false, false, false, false, false, false };
	public Cell.enmTeam[] aiTeams = new Cell.enmTeam[8];
	public float[] decisionSpeeds = new float[8];



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

			Enemy_AI ai = gameObject.AddComponent<Enemy_AI>();
			ai.decisionSpeed = decisionSpeeds[index];
			ai._aiTeam = team;
			ai.isActive = true;
		}
	}

}
