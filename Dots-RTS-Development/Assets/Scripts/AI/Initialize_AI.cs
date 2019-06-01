using System.Collections.Generic;
using UnityEngine;
using Conversions;

public class Initialize_AI : MonoBehaviour {

	public GameObject playerData;


	public static List<Enemy_AI> AIs = new List<Enemy_AI>();
	
	private Player playerScript;

	private void OnEnable() {
		AIs.Clear();
	}
	private void Start() {
		//Should be necessary only for older saves and Debug scene
		
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == UnityEngine.SceneManagement.Scenes.DEBUG) {
			StartAiInitialization(new Dictionary<Team, AIHolder>(), new Dictionary<Team, float>());
		}
	}

	//Goes though all the cells and creates an AI for each team.
	public void StartAiInitialization(Dictionary<Team, AIHolder> clanDict, Dictionary<Team, float> difficultyDict) {
		GameObject g = Instantiate(playerData);
		g.name = "Player";
		playerScript = g.GetComponent<Player>();


		//Make an ai for every team contained in the clandict
		Dictionary<Team, AIHolder>.KeyCollection teams = clanDict.Keys;
		foreach  (Team team in teams) {
			if ((int)team >= 2) {
				float diff;
				if (difficultyDict.TryGetValue(team, out diff) == false) {
					SetAis(team, 2);
				}
				else {
					SetAis(team, diff);
				}
			}
		}

		List<IAlly> InterfaceList = new List<IAlly>();
		foreach (Enemy_AI ai in AIs) {
			InterfaceList.Add(ai);
		}
		InterfaceList.Add(playerScript);


		foreach (IAlly iAlly in InterfaceList) {
			AIHolder temp;

			if (clanDict.TryGetValue(iAlly.Team, out temp) == false) {
				Debug.LogError("Not all IAlly|s are in the dictionary");
			}
			List<Team> allies = temp.allies;
			List<IAlly> alliesI = new List<IAlly>();
			foreach (Team team in allies) {
				foreach (IAlly t in InterfaceList) {
					if (t.Team == team) {
						alliesI.Add(t);
					}
				}
			}
			List<Team> targets = temp.targets;
			List<IAlly> targetsI = new List<IAlly>();
			foreach (Team team in targets) {
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

	public void SetAis(Team team, float decisionSpeed) {
		

			GameObject aiHolder = new GameObject("AI code " + (int)team + " " + team);
			//Select AI preset according to the enemy team
			AI_Behaviour ai;
			switch ((int)team - 1) {
				//case 0: {
				//	ai = aiHolder.AddComponent<AI_0>();
				//	break;
				//}
				//case 1: {
				//	ai = aiHolder.AddComponent<AI_1>();
				//	break;
				//}
				//case 2: {
				//	ai = aiHolder.AddComponent<AI_2>();
				//	break;
				//}
				//case 3: {
				//	ai = aiHolder.AddComponent<AI_3>();
				//	break;
				//}
				//case 4: {
				//	ai = aiHolder.AddComponent<AI_4>();
				//	break;
				//}
				//case 5: {
				//	ai = aiHolder.AddComponent<AI_5>();
				//	break;
				//}
				//case 6: {
				//	ai = aiHolder.AddComponent<AI_6>();
				//	break;
				//}
				//case 7: {
				//	ai = aiHolder.AddComponent<AI_7>();
				//	break;
				//}
				default: {
					ai = aiHolder.AddComponent<AI_Behaviour>();
					break;
				}
			}
			ai.decisionSpeed = decisionSpeed;
			ai.team = team;
			ai.isActive = true;
			AIs.Add(ai);
			ai.Player = playerScript;
	}
}

