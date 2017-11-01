using System.Collections.Generic;
using UnityEngine;
using Conversions;

public class Initialize_AI : MonoBehaviour {

	public static List<List<Cell.enmTeam>> clanList = new List<List<Cell.enmTeam>>();
	public GameObject playerData;

	public bool[] initAIs = new bool[8] { false, false, false, false, false, false, false, false };

	public Cell.enmTeam[] aiTeams = new Cell.enmTeam[8] { Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE, Cell.enmTeam.NONE };
	public float[] decisionSpeeds = new float[8];
	public static Enemy_AI[] AIs = new Enemy_AI[8];
	
	private Player playerScript;

	private void Start() {
		//Should be necessary only for older saves and Debug scene
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == UnityEngine.SceneManagement.Scenes.DEBUG) {
			StartAiInitialization(new Dictionary<Cell.enmTeam, AIHolder>());
		}
	}

	//Goes though all the cells and creates an AI for each team.
	public void StartAiInitialization(Dictionary<Cell.enmTeam, AIHolder> clanDict) {
		clanList = BasicConversions.CDToActualClans(clanDict);

		GameObject g = Instantiate(playerData);
		g.name = "Player";
		playerScript = g.GetComponent<Player>();

		foreach (Cell c in PlayManager.cells) {
			if ((int)c.cellTeam >= 2) {
				SetAis((int)c.cellTeam - 2, c.cellTeam);
			}
		}

		Dictionary<Cell.enmTeam, AIHolder>.KeyCollection keys = clanDict.Keys;
		List<Cell.enmTeam> tl = new List<Cell.enmTeam>();
		foreach (Cell.enmTeam q in keys) {
			tl.Add (q);
		}


		List<IAlly> InterfaceList = new List<IAlly>();
		foreach (Enemy_AI ai in AIs) {

			if (ai != null && tl.Contains (ai.team)) {
				InterfaceList.Add(ai);
			}
		}
		InterfaceList.Add(playerScript);


		foreach (IAlly iAlly in InterfaceList) {
			//print(iAlly.Team);
			AIHolder temp;

			if (clanDict.TryGetValue(iAlly.Team, out temp) == false) {
				Debug.LogError("Not all IAlly|s are in the dictionary");
			}
			List<Cell.enmTeam> allies = temp.allies;
			List<IAlly> alliesI = new List<IAlly>();
			foreach (Cell.enmTeam team in allies) {
				foreach (IAlly t in InterfaceList) {
					if (t.Team == team) {
						alliesI.Add(t);
					}
				}
			}
			List<Cell.enmTeam> targets = temp.targets;
			List<IAlly> targetsI = new List<IAlly>();
			foreach (Cell.enmTeam team in targets) {
				foreach (IAlly t in InterfaceList) {
					if (t.Team == team) {
						targetsI.Add(t);
					}
				}
			}

			foreach (IAlly ally in alliesI) {
				iAlly.AddAlly(ally);
			}
			foreach (IAlly targ in targetsI) {
				iAlly.AddTarget(targ);
			}
		}

		foreach (Enemy_AI ai in AIs) {
			if (ai != null) {
				ai.FindRelationWithCells();
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
			ai.team = team;
			ai.isActive = true;
			AIs[index] = ai;
			ai.playerScript = playerScript;
		}
	}
}

