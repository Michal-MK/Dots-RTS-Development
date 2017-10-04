using System.Collections.Generic;
using UnityEngine;

public class Initialize_AI : MonoBehaviour {

	public bool[] initAIs = new bool[8] { false, false, false, false, false, false, false, false };

	public Cell.enmTeam[] aiTeams = new Cell.enmTeam[8] { Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE };
	public float[] decisionSpeeds = new float[8];
	public static Enemy_AI[] AIs = new Enemy_AI[8];

	private void Start() {
		//Should be necessary only for older saves and Debug scene
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == UnityEngine.SceneManagement.Scenes.DEBUG) {
			StartAiInitialization(new Dictionary<Cell.enmTeam, AIHolder>());
		}
	}

	//Goes though all the cells and creates an AI for each team.
	public void StartAiInitialization(Dictionary<Cell.enmTeam, AIHolder> clanDict) {
		foreach (Cell c in PlayManager.cells) {
			//If cell is enemy create ai for that enemy - Only once
			if ((int)c.cellTeam >= 2) {
				SetAis((int)c.cellTeam - 2, c.cellTeam);

				//print((int)c.cellTeam + "  " + c.gameObject.name);
				//switch ((int)c.cellTeam) {
				//	case 2: {
				//		SetAis(0, c.cellTeam);
				//		break;
				//	}
				//	case 3: {
				//		SetAis(1, c.cellTeam);
				//		break;
				//	}
				//	case 4: {
				//		SetAis(2, c.cellTeam);
				//		break;
				//	}
				//	case 5: {
				//		SetAis(3, c.cellTeam);
				//		break;
				//	}
				//	case 6: {
				//		SetAis(4, c.cellTeam);
				//		break;
				//	}
				//	case 7: {
				//		SetAis(5, c.cellTeam);
				//		break;
				//	}
				//	case 8: {
				//		SetAis(6, c.cellTeam);
				//		break;
				//	}
				//	case 9: {
				//		SetAis(7, c.cellTeam);
				//		break;
				//	}
				//}
			}
		}

		Dictionary<Cell.enmTeam, AIHolder>.KeyCollection keys = clanDict.Keys;
		foreach (Cell.enmTeam j in keys) {
			AIHolder temp;

			clanDict.TryGetValue(j, out temp);
			List<Cell.enmTeam> allies = temp.allies;
			List<Cell.enmTeam> targets = temp.targets;


			foreach (Cell.enmTeam team in allies) {
				AIs[(int)j - 2].AddAlly((Enemy_AI)team);
			}
			foreach (Cell.enmTeam team in targets) {
				AIs[(int)j - 2].AddTarget((Enemy_AI)team);
			}
		}

		/*Test
		foreach(KeyValuePair<Cell.enmTeam, AIHolder> kvp in clanDict) {
			AIHolder temp = kvp.Value;

			foreach (Cell.enmTeam team in kvp.Value.allies) {
				AIs[(int)kvp.Key - 2].AddAlly((Enemy_AI)team);
			}
			foreach(Cell.enmTeam team in kvp.Value.targets) {
				AIs[(int)kvp.Key - 2].AddTarget((Enemy_AI)team);
			}
		}
		*/
	}

	public void SetAis(int index, Cell.enmTeam team) {
		if (initAIs[index] == false) {
			initAIs[index] = true;
			aiTeams[index] = team;

			GameObject aiHolder = new GameObject("AI code " + index + " enemy " + (index + 1));
			//Select AI preset according to the enemy team
			AI_Behaviour ai;
			switch (index) {
				case 0: {
					ai = aiHolder.AddComponent<AI_0>();
					break;
				}
				case 1: {
					ai = aiHolder.AddComponent<AI_1>();
					break;
				}
				case 2: {
					ai = aiHolder.AddComponent<AI_2>();
					break;
				}
				case 3: {
					ai = aiHolder.AddComponent<AI_3>();
					break;
				}
				case 4: {
					ai = aiHolder.AddComponent<AI_4>();
					break;
				}
				case 5: {
					ai = aiHolder.AddComponent<AI_5>();
					break;
				}
				case 6: {
					ai = aiHolder.AddComponent<AI_6>();
					break;
				}
				case 7: {
					ai = aiHolder.AddComponent<AI_7>();
					break;
				}
				default: {
					ai = aiHolder.AddComponent<AI_Behaviour>();
					break;
				}
			}
			ai.decisionSpeed = decisionSpeeds[index];
			ai.aiTeam = team;
			ai.isActive = true;
			AIs[index] = ai;
		}
	}
}

